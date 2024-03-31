CREATE OR REPLACE PROCEDURE test.test_result__insert(
		_test_link_response_id integer,
        _is_passed boolean,
		_test_result_link text,
		_test_template_statistics_link text,
		out _id_test_result integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test_result
			(
				test_link_response_id,
				is_passed,
				test_result_link,
				test_template_statistics_link
			) VALUES 
			(
				_test_link_response_id,
				_is_passed,
				_test_result_link,
				_test_template_statistics_link
			)
			RETURNING id INTO _id_test_result;
				
		END;
    $$
    LANGUAGE plpgsql;