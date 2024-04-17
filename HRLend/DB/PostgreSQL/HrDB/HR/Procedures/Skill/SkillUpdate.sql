CREATE OR REPLACE PROCEDURE hr.skill__update(
		_id integer,
		_title text
	) AS
	$$		
		BEGIN
			UPDATE hr.skill 
			SET 
				title = _title
			WHERE id = _id;
				
		END;
	$$
	LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE hr.skill__update_test_module_link(
		_id integer,
		_test_module_link text
	) AS
	$$		
		BEGIN
			UPDATE hr.skill 
			SET 
				test_module_link = _test_module_link
			WHERE id = _id;
				
		END;
	$$
	LANGUAGE plpgsql;

