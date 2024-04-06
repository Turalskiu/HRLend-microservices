CREATE OR REPLACE PROCEDURE assistant.document__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM assistant.document
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;