CREATE OR REPLACE PROCEDURE auth.group__insert(
		_cabinet_id integer,
        _type_id integer,
        _title text,
		out _id_group integer
	) AS
	$$		
		BEGIN
			INSERT INTO auth.group (
					cabinet_id,
					type_id,
					title
				) 
				VALUES (
					_cabinet_id,
					_type_id,
					_title
				)
			RETURNING id INTO _id_group;
		END;
    $$
    LANGUAGE plpgsql;