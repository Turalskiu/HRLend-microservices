CREATE OR REPLACE FUNCTION test.test__get(
		_ref_test refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test FOR
				SELECT 
					t.title AS title,
					t.description AS description,
					t.test_template_link AS test_template_link
				FROM test.test AS t
				WHERE t.id = _id;		
			RETURN NEXT _ref_test;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test_and_test_link__get(
		_ref_test refcursor,
		_ref_test_link refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test FOR
				SELECT 
					t.title AS title,
					t.description AS description,
					t.test_template_link AS test_template_link
				FROM test.test AS t
				WHERE t.id = _id;		
			RETURN NEXT _ref_test;
				
			OPEN _ref_test_link FOR
				SELECT 
					t_l.id AS id,
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title
				FROM  test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				where t_l.test_id = _id;
			RETURN NEXT _ref_test_link;
			
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test__get_test_template_link(
		_id integer
	) RETURNS TEXT AS
	$$
		DECLARE
    		result TEXT;
		BEGIN
			
			SELECT 
				t.test_template_link INTO result
			FROM test.test AS t
			WHERE t.id = _id;

			IF result IS NULL THEN
				result := '';
			END IF;

			RETURN result;
		END;	
	$$
	LANGUAGE plpgsql;