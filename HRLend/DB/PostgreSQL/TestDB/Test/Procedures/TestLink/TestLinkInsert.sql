CREATE OR REPLACE PROCEDURE test.test_link__insert(
		_test_id integer,
        _status_id integer,
		_type_id integer,
		_user_id integer,
		_group_id integer,
		_limit_candidate_count integer,
		_limit_attempt integer,
		_title text,
		_link text,
		_date_create timestamp(6) with time zone,
		_date_expired timestamp(6) with time zone,
		out _id_test_link integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test_link
			(
				test_id,
				status_id,
				type_id,
				user_id,
				group_id,
				limit_candidate_count,
				limit_attempt,
				candidate_count,
				title,
				link,
				date_create,
				date_expired
			) VALUES 
			(
				_test_id,
				_status_id,
				_type_id,
				_user_id,
				_group_id,
				_limit_candidate_count,
				_limit_attempt,
				0,
				_title,
				_link,
				_date_create,
				_date_expired
			)
			RETURNING id INTO _id_test_link;
				
		END;
    $$
    LANGUAGE plpgsql;