CREATE OR REPLACE FUNCTION auth.group__get(
		_cabinet_id integer,
		_group_id integer
	) RETURNS TABLE (
		title integer,
		type_id text,
		type_title text
	) AS  
	$$
		BEGIN			
			RETURN QUERY
				SELECT
					g.title AS title,
					g.type_id AS type_id,
					g_t.title AS type_title
				FROM  auth.group AS g
				JOIN auth.group_type AS g_t ON g.type_id = g_t.id
				WHERE g.id = _group_id AND g.cabinet_id = _cabinet_id;
		END;	
	$$
	LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.group_and_user__get(
		_ref_group refcursor,
		_ref_user refcursor,
		_cabinet_id integer,
		_group_id integer
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN			
			OPEN _ref_group FOR
				SELECT
					g.title AS title,
					g.type_id AS type_id,
					g_t.title AS type_title
				FROM  auth.group AS g
                JOIN auth.group_type AS g_t ON g.type_id = g_t.id
				WHERE g.id = _group_id AND g.cabinet_id = _cabinet_id;
			RETURN NEXT _ref_user;
			
			OPEN _ref_user FOR
				SELECT 
					u.id AS id,
					u.username AS username,
					u.email AS email,
					u.photo AS photo
				FROM auth.user AS u
				JOIN auth.group_and_user AS g_a_u ON g_a_u.user_id = u.id
				WHERE g_a_u.group_id = _group_id;
			RETURN NEXT _ref_user;
		END;	
	$$
	LANGUAGE plpgsql;