CREATE OR REPLACE PROCEDURE auth.refresh_token__update(
		_refresh_token_id integer[],
        _revoked timestamp(6) with time zone,
        _revoked_by_ip text,
        _replaced_by_token text,
        _reason_revoked text
	) AS
	$$	
		DECLARE
			i integer;
		BEGIN
			FOREACH i IN ARRAY _refresh_token_id
			LOOP
				UPDATE auth.refresh_token SET 
					revoked = _revoked,
					revoked_by_ip = _revoked_by_ip,
					replaced_by_token = _replaced_by_token,
					reason_revoked = _reason_revoked
				WHERE id = i;
			END LOOP;
		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.refresh_token__update(
        _id integer,
        _revoked timestamp(6) with time zone,
        _revoked_by_ip text,
        _replaced_by_token text,
        _reason_revoked text
	) AS
	$$	
		BEGIN
			UPDATE auth.refresh_token SET 
				revoked = _revoked,
				revoked_by_ip = _revoked_by_ip,
				replaced_by_token = _replaced_by_token,
				reason_revoked = _reason_revoked
			WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;