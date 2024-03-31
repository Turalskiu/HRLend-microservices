CREATE OR REPLACE PROCEDURE test.user__insert(
		_id integer,
		_username text,
		_photo text,
		_email text
	) AS
	$$
		BEGIN
			INSERT INTO test.user
			(
				id,
				username,
				photo,
				email
			) VALUES 
			(
				_id,
				_username,
				_photo,
				_email
			);
		END;
    $$
    LANGUAGE plpgsql;