CREATE OR REPLACE PROCEDURE auth.registration_token__delete(
        _id integer
	) AS
	$$	
		BEGIN
			DELETE FROM auth.registration_token
			WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE auth.registration_token__delete(
        _registration_token_id integer[]
	) AS
	$$	
        DECLARE
			i integer;
		BEGIN
            FOREACH i IN ARRAY _registration_token_id
			LOOP
				DELETE FROM auth.registration_token
				WHERE id = i;
			END LOOP;
		END;
    $$
    LANGUAGE plpgsql;