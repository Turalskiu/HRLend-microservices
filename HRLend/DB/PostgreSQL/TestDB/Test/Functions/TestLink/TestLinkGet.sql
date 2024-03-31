CREATE OR REPLACE FUNCTION test.test_link__get_by_link(
		_ref_test_link refcursor,
		_link text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test_link FOR
				SELECT 
					t_l.id AS id,
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title,
					t_l.type_id AS type_id,
					t_l_t.title AS type_title,
					t_l.user_id AS user_id,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					t_l.group_id AS group_id,
					t_l.test_id AS test_id,
					g.title AS group_title,
					t_l.limit_candidate_count AS limit_candidate_count,
					t_l.limit_attempt AS limit_attempt,
					t_l.candidate_count AS candidate_count,
					t_l.date_create AS date_create,
					t_l.date_closed AS date_closed,
					t_l.date_expired AS date_expired
				FROM test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				JOIN test.test_link_type AS t_l_t ON t_l_t.id = t_l.type_id
				LEFT JOIN test.user AS u ON u.id = t_l.user_id
				LEFT JOIN test.group AS g ON g.id = t_l.group_id
				WHERE t_l.link = _link;
			RETURN NEXT _ref_test_link;	
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test_link__get(
		_ref_test_link refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test_link FOR
				SELECT 
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title,
					t_l.type_id AS type_id,
					t_l_t.title AS type_title,
					t_l.user_id AS user_id,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					t_l.group_id AS group_id,
					t_l.test_id AS test_id,
					g.title AS group_title,
					t_l.limit_candidate_count AS limit_candidate_count,
					t_l.limit_attempt AS limit_attempt,
					t_l.candidate_count AS candidate_count,
					t_l.link AS link,
					t_l.date_create AS date_create,
					t_l.date_closed AS date_closed,
					t_l.date_expired AS date_expired
				FROM test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				JOIN test.test_link_type AS t_l_t ON t_l_t.id = t_l.type_id
				LEFT JOIN test.user AS u ON u.id = t_l.user_id
				LEFT JOIN test.group AS g ON g.id = t_l.group_id
				WHERE t_l.id = _id;
			RETURN NEXT _ref_test_link;	
		END;	
	$$
	LANGUAGE plpgsql;