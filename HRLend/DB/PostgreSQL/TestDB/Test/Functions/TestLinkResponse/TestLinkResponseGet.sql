CREATE OR REPLACE FUNCTION test.test_link_and_user_response__get(
		_ref_test_link_response refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			
			OPEN _ref_test_link_response FOR
				SELECT 
					t_l_r.date_create AS date_create,
					t_l_r.test_link_id AS test_link_id,
					t_l_r.test_generated_link AS test_generated_link,
					t_l_r.user_id AS user_id,
					t_l_r.number_attempt AS number_attempt,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					a_u.id AS anonymous_user_id,
					a_u.first_name AS anonymous_user_first_name,
					a_u.last_name AS anonymous_user_last_name,
					a_u.middle_name AS anonymous_user_middle_name,
					a_u.email AS anonymous_user_email,
					t_l_r_s.id AS status_id,
					t_l_r_s.title AS status_title,
					t_r.id AS test_result_id,
					t_r.test_result_link AS test_result_link,
					t_r.test_template_statistics_link AS test_template_statistics_link,
					t_r.is_passed AS test_is_passed
				FROM test.test_link_response AS t_l_r
				JOIN test.test_link_response_status AS t_l_r_s ON t_l_r.status_id = t_l_r_s.id
				LEFT JOIN test.test_result AS t_r ON t_r.test_link_response_id = t_l_r.id
				LEFT JOIN test.anonymous_user AS a_u ON a_u.test_link_response_id = _id
				LEFT JOIN test.user AS u ON u.id = t_l_r.user_id
				WHERE t_l_r.id = _id;
			RETURN NEXT _ref_test_link_response;
			
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test_link_response__get_test_result_link(
		_id integer
	) RETURNS TEXT AS
	$$
		DECLARE
    		result TEXT;
		BEGIN
			
			SELECT 
				t_r.test_result_link INTO result
			FROM test.test_link_response AS t_l_r
			JOIN test.test_result AS t_r ON t_r.test_link_response_id = _id
			WHERE t_l_r.id = _id;

			IF result IS NULL THEN
				result := '';
			END IF;

			RETURN result;
		END;	
	$$
	LANGUAGE plpgsql;