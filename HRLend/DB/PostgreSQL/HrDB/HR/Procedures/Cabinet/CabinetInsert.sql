CREATE OR REPLACE PROCEDURE hr.cabinet__insert(
        _id integer
	) AS
	$$		
		BEGIN
			INSERT INTO hr.cabinet (
					id
				) 
				VALUES (
					_id
				);
		END;
    $$
    LANGUAGE plpgsql;