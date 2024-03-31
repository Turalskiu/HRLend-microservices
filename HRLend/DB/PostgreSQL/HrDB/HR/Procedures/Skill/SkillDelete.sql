CREATE OR REPLACE PROCEDURE hr.skill__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM hr.skill
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;