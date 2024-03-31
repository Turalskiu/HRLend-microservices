
CREATE OR REPLACE FUNCTION auth.group__select(
		_ref_group refcursor,
		_ref_page_info refcursor,
		_cabinet_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN

			OPEN _ref_group FOR
				SELECT
					g.id AS id,
					g.title AS title,
					g.type_id AS type_id,
					g_t.title AS type_title
				FROM  auth.group AS g
                JOIN auth.group_type AS g_t ON g.type_id = g_t.id
				WHERE g.cabinet_id = _cabinet_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN g.title END ASC,
					CASE WHEN _sort = 'desc' THEN g.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			RETURN NEXT _ref_group;		

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.group
						WHERE cabinet_id = _cabinet_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.group__select_by_user(
		_ref_group refcursor,
		_ref_page_info refcursor,
		_user_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN

			OPEN _ref_group FOR
				SELECT
					g.id AS id,
					g.title AS title,
					g.type_id AS type_id,
					g_t.title AS type_title
				FROM  auth.group AS g
                JOIN auth.group_type AS g_t ON g.type_id = g_t.id
				JOIN auth.group_and_user AS g_a_u ON g_a_u.group_id = g.id
				WHERE g_a_u.user_id = _user_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN g.title END ASC,
					CASE WHEN _sort = 'desc' THEN g.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			RETURN NEXT _ref_group;		

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.group AS g
						JOIN auth.group_and_user AS g_a_u ON g_a_u.group_id = g.id
				        WHERE g_a_u.user_id = _user_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;



