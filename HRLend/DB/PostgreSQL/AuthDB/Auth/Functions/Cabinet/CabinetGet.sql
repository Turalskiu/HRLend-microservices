CREATE OR REPLACE FUNCTION auth.cabinet__get(
		_id integer
	) RETURNS TABLE (
		status_id integer,
		status_title text,
		title text,
		description text,
		date_create timestamp(6) with time zone,
		date_delete timestamp(6) with time zone
	) AS 
	$$
		BEGIN
			RETURN QUERY 
			SELECT 
				c.status_id AS status_id,
				c_s.title AS status_title,
				c.title AS title,
				c.description AS description,
				c.date_create AS date_create,
				c.date_delete AS date_delete
			FROM auth.cabinet AS c
			JOIN auth.cabinet_status AS c_s ON c_s.id = c.status_id
			WHERE c.id = _id;
		END;
    $$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.cabinet_and_group_and_user__get(
		_ref_cabinet refcursor,
		_ref_group refcursor,
		_ref_user refcursor,
		_cabinet_id integer
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN	

			OPEN _ref_cabinet FOR
				SELECT 
					c.status_id AS status_id,
					c_s.title AS status_title,
					c.title AS title,
					c.description AS description,
					c.date_create AS date_create,
					c.date_delete AS date_delete
				FROM auth.cabinet AS c
				JOIN auth.cabinet_status AS c_s ON c_s.id = c.status_id
				WHERE c.id = _cabinet_id;
			RETURN NEXT _ref_cabinet;

			OPEN _ref_group FOR
				SELECT
					g.id AS id,
					g.title AS title,
					g.type_id AS type_id,
					g_t.title AS type_title
				FROM  auth.group AS g
                JOIN auth.group_type AS g_t ON g.type_id = g_t.id
				WHERE g.cabinet_id = _cabinet_id;
			RETURN NEXT _ref_user;
			
			OPEN _ref_user FOR
				SELECT 
					u.id AS id,
					u.username AS username,
					u.email AS email,
					u.photo AS photo
				FROM auth.user AS u
				WHERE u.cabinet_id = _cabinet_id;
			RETURN NEXT _ref_user;
		END;	
	$$
	LANGUAGE plpgsql;