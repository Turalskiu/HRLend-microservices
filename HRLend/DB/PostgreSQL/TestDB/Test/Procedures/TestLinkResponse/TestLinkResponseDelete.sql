CREATE OR REPLACE PROCEDURE test.test_link_response__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.test_link_response
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;