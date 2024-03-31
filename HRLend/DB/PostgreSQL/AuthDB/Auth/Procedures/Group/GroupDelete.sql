CREATE OR REPLACE PROCEDURE auth.group__delete(
        _group_id integer,
		_cabinet_id integer
	) AS
	$$		
		BEGIN
			DELETE FROM auth.group
      		WHERE id = _group_id AND cabinet_id = _cabinet_id;
		END;
    $$
    LANGUAGE plpgsql;