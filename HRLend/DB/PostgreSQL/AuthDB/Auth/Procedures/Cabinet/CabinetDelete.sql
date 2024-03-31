CREATE OR REPLACE PROCEDURE auth.cabinet__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM auth.cabinet
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;