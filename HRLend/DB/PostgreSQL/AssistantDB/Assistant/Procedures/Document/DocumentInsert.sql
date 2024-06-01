CREATE OR REPLACE PROCEDURE assistant.document__insert(
		_cabinet_id integer,
		_title text,
		_type_id integer,
		_elasticsearch_index text,
		out _id_document integer
	) AS
	$$		
		BEGIN
			INSERT INTO assistant.document (
					cabinet_id,
					type_id,
					title,
					elasticsearch_index
				) 
				VALUES (
					_cabinet_id,
					_type_id,
					_title,
					_elasticsearch_index
				)
			RETURNING id INTO _id_document;
		END;
    $$
    LANGUAGE plpgsql;