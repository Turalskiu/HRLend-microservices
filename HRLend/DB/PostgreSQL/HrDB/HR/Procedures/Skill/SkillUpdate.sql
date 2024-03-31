CREATE OR REPLACE PROCEDURE hr.skill__update(
		_id integer,
		_title text,
		_test_module_link text
	) AS
	$$		
		BEGIN
			UPDATE hr.skill 
			SET 
				title = _title,
				test_module_link = _test_module_link
			WHERE id = _id;
				
		END;
	$$
	LANGUAGE plpgsql;

