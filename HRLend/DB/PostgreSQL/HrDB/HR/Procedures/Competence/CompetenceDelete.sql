CREATE OR REPLACE PROCEDURE hr.competence__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM hr.competence
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;