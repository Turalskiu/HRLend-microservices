CREATE OR REPLACE PROCEDURE auth.cabinet__update(
		_id integer,
		_title text,
		_description text
	) AS
	$$		
		BEGIN
			UPDATE auth.cabinet SET 
				title = _title,
				description = _description
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.cabinet__update_status_delete(
        _id integer,
        _status_id integer,
        _date_delete timestamp(6) with time zone
	) AS
    $$		
		BEGIN
			UPDATE auth.cabinet SET 
				status_id = _status_id,
				date_delete = _date_delete
			WHERE id = _id; 
		END;
    $$
    LANGUAGE plpgsql;