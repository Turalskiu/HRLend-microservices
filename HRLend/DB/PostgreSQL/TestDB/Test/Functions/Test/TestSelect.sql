CREATE OR REPLACE FUNCTION test.test__select(
		_ref_test refcursor,
		_ref_page_info refcursor,
		_hr_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test FOR
				SELECT 
					t.id AS id,
					t.title AS title
				FROM test.test AS t
				WHERE t.hr_id = _hr_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t.title END ASC,
					CASE WHEN _sort = 'desc' THEN t.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM test.test
						WHERE hr_id = _hr_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;