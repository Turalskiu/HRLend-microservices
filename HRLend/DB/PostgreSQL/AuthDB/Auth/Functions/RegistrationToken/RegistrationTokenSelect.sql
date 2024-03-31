CREATE OR REPLACE FUNCTION auth.registration_token__select(
		_ref_registration_token refcursor,
		_ref_page_info refcursor,
		_user_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN
			OPEN _ref_registration_token FOR
				SELECT
					r_t.id AS id,
					r_t.token AS token,
					r_t.expires AS expires,
					r_t.created AS created,
					r_t.created_by_ip AS created_by_ip,
					r_t.cabinet AS cabinet,
					r_t.cabinet_role
				FROM  auth.registration_token AS r_t
				WHERE r_t.user_id = _user_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN r_t.created END ASC,
					CASE WHEN _sort = 'desc' THEN r_t.created END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;
			RETURN NEXT _ref_registration_token;

			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM auth.registration_token
						WHERE user_id = _user_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
		END;	
	$$
	LANGUAGE plpgsql;