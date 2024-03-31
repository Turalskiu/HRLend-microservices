CREATE OR REPLACE FUNCTION hr.skill__select(
		_ref_skill refcursor,
		_ref_page_info refcursor,
		_cabinet_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			
			OPEN _ref_skill FOR
				SELECT 
					s.id AS id,
					s.title AS title,
					s.test_module_link AS test_module_link
				FROM hr.skill AS s
				WHERE s.cabinet_id = _cabinet_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN s.title END ASC,
					CASE WHEN _sort = 'desc' THEN s.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_skill;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM hr.skill
						WHERE cabinet_id = _cabinet_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;