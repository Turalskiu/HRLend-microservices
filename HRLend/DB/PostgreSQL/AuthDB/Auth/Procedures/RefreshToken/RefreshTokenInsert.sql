CREATE OR REPLACE PROCEDURE auth.refresh_token__insert(
        _user_id integer,
        _token text,
        _expires timestamp(6) with time zone,
        _created timestamp(6) with time zone,
        _created_by_ip text,
		out _is_insert boolean
	) AS
	$$
		BEGIN

			IF EXISTS (SELECT true FROM auth.refresh_token WHERE token = _token) THEN
				_is_insert := false;
			ELSE
				INSERT INTO auth.refresh_token (
					user_id,
					token,
					expires,
                    created,
					created_by_ip
				) 
				VALUES (
					_user_id,
					_token,
					_expires,
                    _created,
					_created_by_ip
				);
				_is_insert := true;
			END IF;

		END;
    $$
    LANGUAGE plpgsql;