CREATE OR REPLACE PROCEDURE hr.cabinet__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM hr.cabinet
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;