CREATE OR REPLACE PROCEDURE auth.registration_token__insert(
        _user_id integer,
        _token text,
        _expires timestamp(6) with time zone,
        _created timestamp(6) with time zone,
        _created_by_ip text,
		_cabinet integer,
		_cabinet_role integer,
		out _is_insert boolean
	) AS
	$$
		BEGIN

			IF EXISTS (SELECT true FROM auth.registration_token WHERE token = _token) THEN
				_is_insert := false;
			ELSE
				INSERT INTO auth.registration_token (
					user_id,
					token,
					expires,
                    created,
					created_by_ip,
					cabinet,
					cabinet_role
				) 
				VALUES (
					_user_id,
					_token,
					_expires,
                    _created,
					_created_by_ip,
					_cabinet,
					_cabinet_role
				);
				_is_insert := true;
			END IF;

		END;
    $$
    LANGUAGE plpgsql;