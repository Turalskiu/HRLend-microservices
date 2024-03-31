CREATE OR REPLACE PROCEDURE test.user__update_username(
		_id integer,
		_username text
	) AS
	$$		
		BEGIN
			UPDATE test.user SET 
				username = _username
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE test.user__update_photo(
		_id integer,
		_photo text
	) AS
	$$		
		BEGIN
			UPDATE test.user SET 
				photo = _photo
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


