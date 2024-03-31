CREATE OR REPLACE PROCEDURE auth.user__update_username(
		_id integer,
		_username text
	) AS
	$$		
		BEGIN
			UPDATE auth.user SET 
				username = _username
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__update_photo(
		_id integer,
		_photo text
	) AS
	$$		
		BEGIN
			UPDATE auth.user SET 
				photo = _photo
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__update_password(
		_id integer,
		_password_hash text
	) AS
	$$		
		BEGIN
			UPDATE auth.user SET 
				password_hash = _password_hash
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__update_role(
		_id integer,
		_role_id integer[]
	) AS
	$$		
		BEGIN
			DELETE FROM auth.user_and_role
			WHERE user_id = _id;

			INSERT INTO auth.user_and_role(
			  user_id,
			  role_id
			)
			SELECT _id, unnest(_role_id);	
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__update_status_delete(
        _id integer,
        _status_id integer,
        _date_delete timestamp(6) with time zone
	) AS
    $$		
		BEGIN
			UPDATE auth.user SET 
				status_id = _status_id,
				date_delete = _date_delete
			WHERE id = _id; 
		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__update_status_blocked(
        _id integer,
        _status_id integer,
        _date_blocked timestamp(6) with time zone,
		_date_unblocked timestamp(6) with time zone,
		_reason_blocked text
	) AS
    $$		
		BEGIN
			UPDATE auth.user SET 
				status_id = _status_id,
				date_blocked = _date_blocked,
				date_unblocked = _date_unblocked,
				reason_blocked = _reason_blocked
			WHERE id = _id; 
		END;
    $$
    LANGUAGE plpgsql;


	CREATE OR REPLACE PROCEDURE auth.user__update_status_unblocked(
        _id integer,
        _status_id integer,
		_date_unblocked timestamp(6) with time zone
	) AS
    $$		
		BEGIN
			UPDATE auth.user SET 
				status_id = _status_id,
				date_unblocked = _date_unblocked
			WHERE id = _id; 
		END;
    $$
    LANGUAGE plpgsql;