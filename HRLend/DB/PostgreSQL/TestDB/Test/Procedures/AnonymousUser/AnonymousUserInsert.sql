CREATE OR REPLACE PROCEDURE test.anonymous_user__insert(
		_test_link_response_id integer,
		_first_name text,
		_last_name text,
		_middle_name text,
		_email text,
		out _id_anonymous_user integer
	) AS
	$$
		BEGIN
			INSERT INTO test.anonymous_user
			(
				test_link_response_id,
				first_name,
				last_name,
				middle_name,
				email
			) VALUES 
			(
				_test_link_response_id,
				_first_name,
				_last_name,
				_middle_name,
				_email
			)
			RETURNING id INTO _id_anonymous_user;	
		END;
    $$
    LANGUAGE plpgsql;