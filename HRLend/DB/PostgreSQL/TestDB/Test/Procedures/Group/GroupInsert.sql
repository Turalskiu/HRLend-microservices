CREATE OR REPLACE PROCEDURE test.group__insert(
		_id integer,
        _title text
	) AS
	$$		
		BEGIN
			INSERT INTO test.group (
					id,
					title
				) 
				VALUES (
					_id,
					_title
				);
		END;
    $$
    LANGUAGE plpgsql;