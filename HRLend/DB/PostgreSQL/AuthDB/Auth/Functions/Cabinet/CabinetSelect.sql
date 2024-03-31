CREATE OR REPLACE FUNCTION auth.cabinet__select(
		_ref_cabinet refcursor,
		_ref_page_info refcursor,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN	

			OPEN _ref_cabinet FOR
				SELECT 
					c.id AS id,
					c.status_id AS status_id,
					c_s.title AS status_title,
					c.title AS title,
					c.description AS description,
					c.date_create AS date_create,
					c.date_delete AS date_delete
				FROM auth.cabinet AS c
				JOIN auth.cabinet_status AS c_s ON c_s.id = c.status_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN c.date_create END ASC,
					CASE WHEN _sort = 'desc' THEN c.date_create END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			RETURN NEXT _ref_cabinet;

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.cabinet
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;