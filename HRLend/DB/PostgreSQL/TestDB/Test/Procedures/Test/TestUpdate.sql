CREATE OR REPLACE PROCEDURE test.test__update(
		_id integer,
		_title text,
		_description text
	) AS
	$$		
		BEGIN
			UPDATE test.test
			SET 
				title = _title,
				description = _description
			WHERE id = _id;
				
		END;
	$$
	LANGUAGE plpgsql;

