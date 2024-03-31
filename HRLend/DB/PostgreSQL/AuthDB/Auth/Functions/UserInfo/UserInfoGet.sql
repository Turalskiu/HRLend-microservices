CREATE OR REPLACE FUNCTION auth.user_info_and_role__get(
		_ref_user refcursor,
		_ref_role refcursor,
		_id integer
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN			
			OPEN _ref_user FOR
				SELECT
                    u.username AS username,
					u.cabinet_id AS cabinet_id,
					u.status_id AS status_id,
                    u_s.title AS status_title,
					u.email AS email,
					u.photo AS photo,
					u.date_create AS date_create,
					u.date_delete AS date_delete,
					u.date_blocked AS date_blocked,
					u.date_activation AS date_activation,
					u.date_unblocked AS date_unblocked,
					u.reason_blocked AS reason_blocked,
					u_i.first_name AS first_name,
					u_i.middle_name AS middle_name,
					u_i.last_name AS last_name,
					u_i.age AS age
				FROM  auth.user AS u
                JOIN auth.user_status AS u_s ON u_s.id = u.status_id
				JOIN auth.user_info AS u_i ON u_i.user_id = _id
				WHERE u.id = _id;
			RETURN NEXT _ref_user; 
			
            OPEN _ref_role FOR
				SELECT
                    r.id AS id,
					r.title AS title
				FROM  auth.role AS r
                JOIN auth.user_and_role AS u_a_r ON u_a_r.role_id = r.id 
				WHERE u_a_r.user_id = _id;
			RETURN NEXT _ref_role;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user_info_and_role__get(
		_ref_user refcursor,
		_ref_role refcursor,
		_cabinet_id integer,
		_user_id integer
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN			
			OPEN _ref_user FOR
				SELECT
                    u.username AS username,
					u.status_id AS status_id,
                    u_s.title AS status_title,
					u.email AS email,
					u.photo AS photo,
					u_i.first_name AS first_name,
					u_i.middle_name AS middle_name,
					u_i.last_name AS last_name,
					u_i.age AS age
				FROM  auth.user AS u
                JOIN auth.user_status AS u_s ON u_s.id = u.status_id
				JOIN auth.user_info AS u_i ON u_i.user_id = _user_id
				WHERE u.id = _user_id AND u.cabinet_id = _cabinet_id; 
			RETURN NEXT _ref_user; 
			
            OPEN _ref_role FOR
				SELECT
                    r.id AS id,
					r.title AS title
				FROM  auth.role AS r
                JOIN auth.user_and_role AS u_a_r ON u_a_r.role_id = r.id 
				WHERE u_a_r.user_id = _user_id;
			RETURN NEXT _ref_role;
		END;	
	$$
	LANGUAGE plpgsql;
