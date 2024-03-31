CREATE OR REPLACE PROCEDURE auth.group__update(
		_group_id integer,
		_cabinet_id integer,
		_title text
	) AS
	$$		
		BEGIN
			UPDATE auth.group SET 
				title = _title
			WHERE _group_id = id AND _cabinet_id = cabinet_id; 
		END;
	$$
	LANGUAGE plpgsql;
