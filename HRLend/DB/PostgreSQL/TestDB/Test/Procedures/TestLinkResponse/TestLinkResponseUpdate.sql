CREATE OR REPLACE PROCEDURE test.test_link_response__update(
		_id integer,
		_status_id integer
	) AS
	$$		
		BEGIN
			UPDATE test.test_link_response
			SET 
				status_id = _status_id
			WHERE id = _id;		
		END;
	$$
	LANGUAGE plpgsql;



