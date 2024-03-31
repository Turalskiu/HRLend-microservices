CREATE OR REPLACE FUNCTION auth.registration_token__get(
		_id integer
	) RETURNS TABLE (
		user_id integer,
		token text,
		expires timestamp(6) with time zone,
		created timestamp(6) with time zone,
		created_by_ip text,
		cabinet integer,
		cabinet_role integer
	) AS  
	$$
		BEGIN			
			RETURN QUERY
				SELECT
					r_t.user_id AS user_id,
					r_t.token AS token,
					r_t.expires AS expires,
					r_t.created AS created,
					r_t.created_by_ip AS created_by_ip,
					r_t.cabinet AS cabinet,
					r_t.cabinet_role
				FROM  auth.registration_token AS r_t
				WHERE r_t.id = _id;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.registration_token__get(
		_token text
	) RETURNS TABLE (
		id integer,
		expires timestamp(6) with time zone,
		created timestamp(6) with time zone,
		created_by_ip text,
		cabinet integer,
		cabinet_role integer
	) AS  
	$$
		BEGIN			
			RETURN QUERY
				SELECT
					r_t.id AS id,
					r_t.expires AS expires,
					r_t.created AS created,
					r_t.created_by_ip AS created_by_ip,
					r_t.cabinet AS cabinet,
					r_t.cabinet_role AS cabinet_role
				FROM  auth.registration_token AS r_t
				WHERE r_t.token = _token;
		END;	
	$$
	LANGUAGE plpgsql;