CREATE OR REPLACE PROCEDURE test.test_link__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.test_link
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;