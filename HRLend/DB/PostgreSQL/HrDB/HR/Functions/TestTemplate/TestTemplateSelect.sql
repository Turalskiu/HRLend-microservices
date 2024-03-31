CREATE OR REPLACE FUNCTION hr.test_template__select(
		_ref_test_template refcursor,
		_ref_page_info refcursor,
		_cabinet_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test_template FOR
				SELECT 
					t_t.id AS id,
					t_t.title AS title
				FROM hr.test_template AS t_t
				WHERE t_t.cabinet_id = _cabinet_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t_t.title END ASC,
					CASE WHEN _sort = 'desc' THEN t_t.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test_template;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM hr.test_template
						WHERE cabinet_id = _cabinet_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;