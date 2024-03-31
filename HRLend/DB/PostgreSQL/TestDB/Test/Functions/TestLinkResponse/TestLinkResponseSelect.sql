CREATE OR REPLACE FUNCTION test.test_link_response_and_user__select(
		_ref_test_link_response refcursor,
		_ref_page_info refcursor,
		_test_link_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test_link_response FOR
				SELECT 
					t_l_r.id AS id,
					t_l_r.date_create AS date_create,
					t_l_r.status_id AS status_id,
					t_l_r.number_attempt AS number_attempt,
					t_l_r_s.title AS status_title,
					a_u.id AS anonymous_user_id,
					a_u.first_name AS anonymous_user_first_name,
					a_u.email AS anonymous_user_email,
					t_l_r.user_id AS user_id,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo
				FROM test.test_link_response AS t_l_r
				JOIN test.test_link_response_status AS t_l_r_s ON t_l_r.status_id = t_l_r_s.id
				LEFT JOIN test.anonymous_user AS a_u ON a_u.test_link_response_id = t_l_r.id
				LEFT JOIN test.user AS u ON u.id = t_l_r.user_id
				WHERE t_l_r.test_link_id = _test_link_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t_l_r.date_create END ASC,
					CASE WHEN _sort = 'desc' THEN t_l_r.date_create END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test_link_response;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM test.test_link_response AS t_l_r
						WHERE t_l_r.test_link_id = _test_link_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;