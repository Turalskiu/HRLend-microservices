CREATE OR REPLACE PROCEDURE hr.skill__insert(
		_cabinet_id integer,
        _title text,
		_test_module_link text,
		out _id_skill integer
	) AS
	$$
		BEGIN
			INSERT INTO hr.skill 
			(
				cabinet_id,
				title, 
				test_module_link
			) VALUES 
			(
				_cabinet_id,
				_title,
				_test_module_link
			)
			RETURNING id INTO _id_skill;
				
		END;
    $$
    LANGUAGE plpgsql;