CREATE OR REPLACE FUNCTION auth.user_and_role__get(
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
					u.password_hash AS password_hash,
					u.date_create AS date_create,
					u.date_delete AS date_delete,
					u.date_activation AS date_activation,
					u.date_blocked AS date_blocked,
					u.date_unblocked AS date_unblocked,
					u.reason_blocked AS reason_blocked
				FROM  auth.user AS u
                JOIN auth.user_status AS u_s ON u_s.id = u.status_id
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


CREATE OR REPLACE FUNCTION auth.user_and_role__get(
		_ref_user refcursor,
		_ref_role refcursor,
		_ref_refresh_token refcursor,
		_username text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN			
			OPEN _ref_user FOR
				SELECT
                    u.id AS id,
					u.cabinet_id AS cabinet_id,
					u.status_id AS status_id,
                    u_s.title AS status_title,
					u.email AS email,
					u.photo AS photo,
					u.password_hash AS password_hash,
					u.date_create AS date_create,
					u.date_delete AS date_delete,
					u.date_activation AS date_activation,
					u.date_blocked AS date_blocked,
					u.date_unblocked AS date_unblocked,
					u.reason_blocked AS reason_blocked
				FROM  auth.user AS u
                JOIN auth.user_status AS u_s ON u_s.id = u.status_id
				WHERE u.username = _username;
			RETURN NEXT _ref_user;
			
            OPEN _ref_role FOR
				SELECT
					r.id AS id,
					r.title AS title
				FROM  auth.role AS r
                JOIN auth.user_and_role AS u_a_r ON u_a_r.role_id = r.id 
				JOIN auth.user AS u ON u_a_r.user_id = u.id
				WHERE u.username = _username;
			RETURN NEXT _ref_role;
			
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user_and_role_and_refresh_token__get(
		_ref_user refcursor,
		_ref_role refcursor,
		_ref_refresh_token refcursor,
		_username text
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN			
			OPEN _ref_user FOR
				SELECT
                    u.id AS id,
					u.cabinet_id AS cabinet_id,
					u.status_id AS status_id,
                    u_s.title AS status_title,
					u.email AS email,
					u.photo AS photo,
					u.password_hash AS password_hash,
					u.date_create AS date_create,
					u.date_delete AS date_delete,
					u.date_activation AS date_activation,
					u.date_blocked AS date_blocked,
					u.date_unblocked AS date_unblocked,
					u.reason_blocked AS reason_blocked
				FROM  auth.user AS u
                JOIN auth.user_status AS u_s ON u_s.id = u.status_id
				WHERE u.username = _username;
			RETURN NEXT _ref_user;
			
            OPEN _ref_role FOR
				SELECT
					r.id AS id,
					r.title AS title
				FROM  auth.role AS r
                JOIN auth.user_and_role AS u_a_r ON u_a_r.role_id = r.id 
				JOIN auth.user AS u ON u_a_r.user_id = u.id
				WHERE u.username = _username;
			RETURN NEXT _ref_role;
			
			OPEN _ref_refresh_token FOR
				SELECT
                    r_t.id AS id,
					r_t.user_id AS user_id,
					r_t.token AS token,
					r_t.expires AS expires,
					r_t.created AS created,
					r_t.created_by_ip AS created_by_ip,
					r_t.revoked AS revoked,
					r_t.revoked_by_ip AS revoked_by_ip,
					r_t.replaced_by_token AS replaced_by_token,
					r_t.reason_revoked AS reason_revoked
				FROM  auth.refresh_token AS r_t
                JOIN auth.user AS u ON u.id = r_t.user_id 
				WHERE u.username = _username;
			RETURN NEXT _ref_refresh_token;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user_and_role_and_refresh_token__get_by_refresh_token(
		_ref_user refcursor,
		_ref_role refcursor,
		_ref_refresh_token refcursor,
		_token text
	) RETURNS SETOF refcursor AS 
	$$
		DECLARE
			us_id integer;

		BEGIN	
				
			SELECT r_t.user_id INTO us_id
			FROM auth.refresh_token AS r_t
			WHERE r_t.token = _token;
			
			OPEN _ref_user FOR
				SELECT
                    u.id AS id,
					u.cabinet_id AS cabinet_id,
					u.username AS username,
					u.status_id AS status_id,
                    u_s.title AS status_title,
					u.email AS email,
					u.photo AS photo,
					u.password_hash AS password_hash,
					u.date_create AS date_create,
					u.date_delete AS date_delete,
					u.date_activation AS date_activation,
					u.date_blocked AS date_blocked,
					u.date_unblocked AS date_unblocked,
					u.reason_blocked AS reason_blocked
				FROM  auth.user AS u
                JOIN auth.user_status AS u_s ON u_s.id = u.status_id
				WHERE u.id = us_id;
			RETURN NEXT _ref_user;
			
            OPEN _ref_role FOR
				SELECT
                    r.id AS id,
					r.title AS title
				FROM  auth.role AS r
                JOIN auth.user_and_role AS u_a_r ON u_a_r.role_id = r.id 
				WHERE u_a_r.user_id = us_id;
			RETURN NEXT _ref_role;
			
			OPEN _ref_refresh_token FOR
				SELECT
                    r_t.id AS id,
					r_t.user_id AS user_id,
					r_t.token AS token,
					r_t.expires AS expires,
					r_t.created AS created,
					r_t.created_by_ip AS created_by_ip,
					r_t.revoked AS revoked,
					r_t.revoked_by_ip AS revoked_by_ip,
					r_t.replaced_by_token AS replaced_by_token,
					r_t.reason_revoked AS reason_revoked
				FROM  auth.refresh_token AS r_t
				WHERE r_t.user_id = us_id;
			RETURN NEXT _ref_refresh_token;
		END;	
	$$
	LANGUAGE plpgsql;