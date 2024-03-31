CREATE OR REPLACE PROCEDURE test.group__delete(
        _group_id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.group
      		WHERE id = _group_id;
		END;
    $$
    LANGUAGE plpgsql;