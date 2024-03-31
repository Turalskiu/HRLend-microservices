CREATE OR REPLACE PROCEDURE auth.user__insert(
		_cabinet_id integer,
        _username text,
        _email text,
        _password_hash text,
        _date_create timestamp(6) with time zone,
		_date_activation timestamp(6) with time zone,
        _status_id integer,
		out _id_user integer,
        _role_id integer[] default null
	) AS
	$$
			
		BEGIN
			INSERT INTO auth.user (
					cabinet_id,
					username,
					email,
					password_hash,
					date_create,
					date_activation,
                    status_id
				) 
				VALUES (
					_cabinet_id,
					_username,
					_email,
					_password_hash,
					_date_create,
					_date_activation,
                    _status_id
				)	
			RETURNING id INTO _id_user;
				
			IF  NOT (_role_id IS NULL) THEN
				INSERT INTO auth.user_and_role(
					user_id,
					role_id
				)
				SELECT _id_user, unnest(_role_id);	
			END IF;

			INSERT INTO auth.user_info (
					user_id
				) 
				VALUES (
					_id_user
				);
		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__add_role(
		_user_id integer,
		_role_id integer
	) AS
	$$		
		BEGIN

			INSERT INTO auth.user_and_role(
			  user_id,
			  role_id
			)
			VALUES(
				_user_id,
				_role_id
			);	
		END;
	$$
	LANGUAGE plpgsql;