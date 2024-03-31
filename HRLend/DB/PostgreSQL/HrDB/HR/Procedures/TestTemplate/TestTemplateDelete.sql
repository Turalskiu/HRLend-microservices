CREATE OR REPLACE PROCEDURE hr.test_template__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM hr.test_template
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;