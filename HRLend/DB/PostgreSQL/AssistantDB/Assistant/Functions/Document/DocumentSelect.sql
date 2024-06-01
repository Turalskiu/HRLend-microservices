CREATE OR REPLACE FUNCTION assistant.document__select(
		_cabinet_id integer
	)RETURNS TABLE (
		id integer,
		title text ,
		type_id integer,
		type_title text,
		elasticsearch_index text
	) AS 
	$$
		BEGIN

			RETURN QUERY
				SELECT 
					d.id AS id,
					d.title AS title,
					d_t.id AS type_id,
					d_t.title AS type_title,
					d.elasticsearch_index AS elasticsearch_index
				FROM assistant.document AS d
				JOIN assistant.document_type AS d_t ON d_t.id = d.type_id
				WHERE _cabinet_id = d.cabinet_id;	
		END;	
	$$
	LANGUAGE plpgsql;


