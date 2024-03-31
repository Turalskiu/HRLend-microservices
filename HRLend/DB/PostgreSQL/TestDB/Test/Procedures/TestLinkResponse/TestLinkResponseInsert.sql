CREATE OR REPLACE PROCEDURE test.test_link_response__insert(
		_test_link_id integer,
        _status_id integer,
		_user_id integer,
		_number_attempt integer,
		_test_generated_link text,
		_date_create timestamp(6) with time zone,
		out _id_test_link_response integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test_link_response
			(
				test_link_id,
				status_id,
				user_id,
				number_attempt,
				test_generated_link,
				date_create
			) VALUES 
			(
				_test_link_id,
				_status_id,
				_user_id,
				_number_attempt,
				_test_generated_link,
				_date_create
			)
			RETURNING id INTO _id_test_link_response;	
		END;
    $$
    LANGUAGE plpgsql;