CREATE OR REPLACE FUNCTION test.test_link__select(
		_ref_test_link refcursor,
		_ref_page_info refcursor,
		_hr_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test_link FOR
				SELECT 
					t_l.id AS id,
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title
				FROM test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				JOIN test.test AS t ON t.id = t_l.test_id
				WHERE t.hr_id = _hr_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t_l.title END ASC,
					CASE WHEN _sort = 'desc' THEN t_l.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test_link;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM test.test_link AS t_l
						JOIN test.test AS t ON t.id = t_l.test_id
						WHERE t.hr_id = _hr_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;