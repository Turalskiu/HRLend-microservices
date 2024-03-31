
CREATE OR REPLACE FUNCTION auth.user__select(
		_ref_user refcursor,
		_ref_page_info refcursor,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN

			OPEN _ref_user FOR
				SELECT 
					u.id AS id,
					u.username AS username,
					u.email AS email,
					u.photo AS photo
				FROM auth.user AS u
				ORDER BY
					CASE WHEN _sort = 'asc' THEN u.date_create END ASC,
					CASE WHEN _sort = 'desc' THEN u.date_create END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			RETURN NEXT _ref_user;		

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.user
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user_and_role__select(
		_ref_user refcursor,
		_ref_role refcursor,
		_ref_page_info refcursor,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN

			CREATE TEMP TABLE temp_user ON COMMIT DROP AS
      		SELECT 
				u.id AS id,
				u.username AS username,
				u.cabinet_id AS cabinet_id,
				u.status_id AS status_id,
				u_s.title AS status_title,
				u.email AS email,
				u.photo AS photo
			FROM auth.user AS u
			JOIN auth.user_status AS u_s ON u.status_id = u_s.id
			ORDER BY
				CASE WHEN _sort = 'asc' THEN u.date_create END ASC,
				CASE WHEN _sort = 'desc' THEN u.date_create END DESC
			LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			
			OPEN _ref_user FOR
				SELECT *
				FROM temp_user;
			RETURN NEXT _ref_user;		
			
            OPEN _ref_role FOR
				SELECT
                    u_a_r.user_id AS user_id,
                    r.id AS role_id,
					r.title AS role_title
				FROM temp_user AS u
                JOIN auth.user_and_role AS u_a_r ON u_a_r.user_id = u.id
				JOIN auth.role AS r ON u_a_r.role_id = r.id;
			RETURN NEXT _ref_role;

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.user
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user_and_role__select_by_cabinet(
		_ref_user refcursor,
		_ref_role refcursor,
		_ref_page_info refcursor,
		_cabinet_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN

			CREATE TEMP TABLE temp_user ON COMMIT DROP AS
      		SELECT 
				u.id AS id,
				u.username AS username,
				u.email AS email,
				u.photo AS photo
			FROM auth.user AS u
			WHERE u.cabinet_id = _cabinet_id
			ORDER BY
				CASE WHEN _sort = 'asc' THEN u.date_create END ASC,
				CASE WHEN _sort = 'desc' THEN u.date_create END DESC
			LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			
			OPEN _ref_user FOR
				SELECT *
				FROM temp_user;
			RETURN NEXT _ref_user;		
			
            OPEN _ref_role FOR
				SELECT
                    u_a_r.user_id AS user_id,
                    r.id AS role_id,
					r.title AS role_title
				FROM temp_user AS u
                JOIN auth.user_and_role AS u_a_r ON u_a_r.user_id = u.id
				JOIN auth.role AS r ON u_a_r.role_id = r.id;
			RETURN NEXT _ref_role;

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.user
						WHERE cabinet_id = _cabinet_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user__select_by_group(
		_ref_user refcursor,
		_ref_page_info refcursor,
		_cabinet_id integer,
		_group_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN
			
			OPEN _ref_user FOR
				SELECT 
					u.id AS id,
					u.username AS username,
					u.email AS email,
					u.photo AS photo
				FROM auth.user AS u
				JOIN auth.group_and_user AS g_a_u ON u.id = g_a_u.user_id
				WHERE g_a_u.group_id = _group_id AND _cabinet_id = u.cabinet_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN u.date_create END ASC,
					CASE WHEN _sort = 'desc' THEN u.date_create END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			RETURN NEXT _ref_user;		
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.user AS u
						JOIN auth.group_and_user AS g_a_u ON u.id = g_a_u.user_id
						WHERE g_a_u.group_id = _group_id AND _cabinet_id = u.cabinet_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;