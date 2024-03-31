CREATE OR REPLACE PROCEDURE auth.refresh_token__delete(
        _refresh_token_id integer[]
	) AS
	$$	
        DECLARE
			i integer;
		BEGIN
            FOREACH i IN ARRAY _refresh_token_id
			LOOP
				DELETE FROM auth.refresh_token
				WHERE id = i;
			END LOOP;
		END;
    $$
    LANGUAGE plpgsql;