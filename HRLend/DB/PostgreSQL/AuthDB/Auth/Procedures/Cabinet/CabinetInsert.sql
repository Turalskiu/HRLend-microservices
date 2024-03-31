CREATE OR REPLACE PROCEDURE auth.cabinet__insert(
        _status_id integer,
        _title text,
		_description text,
		_date_create timestamp(6) with time zone,
		out _id_cabinet integer
	) AS
	$$		
		BEGIN
			INSERT INTO auth.cabinet (
					status_id,
					title,
					description,
					date_create
				) 
				VALUES (
					_status_id,
					_title,
					_description,
					_date_create
				)
			RETURNING id INTO _id_cabinet;
		END;
    $$
    LANGUAGE plpgsql;