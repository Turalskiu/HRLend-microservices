CREATE OR REPLACE PROCEDURE auth.user_info__update(
		_id integer,
		_first_name text,
		_middle_name text,
		_last_name text,
		_age integer
	) AS
	$$		
		BEGIN
			UPDATE auth.user_info SET 
				first_name = _first_name,
				last_name = _last_name,
				middle_name = _middle_name,
				age = _age
			WHERE user_id = _id;
		END;
	$$
	LANGUAGE plpgsql;

