CREATE OR REPLACE PROCEDURE test.test__insert(
		_hr_id integer,
        _title text,
		_description text,
		_test_template_link text,
		out _id_test integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test 
			(
				hr_id,
				title, 
				description,
				test_template_link
			) VALUES 
			(
				_hr_id,
				_title, 
				_description,
				_test_template_link
			)
			RETURNING id INTO _id_test;
				
		END;
    $$
    LANGUAGE plpgsql;