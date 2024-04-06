CREATE OR REPLACE FUNCTION assistant.document__get(
		_id integer
	) RETURNS TABLE (
		cabinet_id integer,
		title text,
		elasticsearch_index text
	) AS 
	$$
		BEGIN
			RETURN QUERY 
			SELECT 
				d.cabinet_id AS cabinet_id,
				d.title AS title,
				d.elasticsearch_index AS elasticsearch_index
			FROM assistant.document AS d
			WHERE d.id = _id;
		END;
    $$
	LANGUAGE plpgsql;