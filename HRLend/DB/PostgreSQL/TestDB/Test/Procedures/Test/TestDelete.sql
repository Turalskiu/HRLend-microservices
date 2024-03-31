CREATE OR REPLACE PROCEDURE test.test__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.test
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;