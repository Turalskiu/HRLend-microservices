CREATE OR REPLACE PROCEDURE test.group__update(
		_id integer,
		_title text
	) AS
	$$		
		BEGIN
			UPDATE test.group SET 
				title = _title
			WHERE _id = id; 
		END;
	$$
	LANGUAGE plpgsql;
