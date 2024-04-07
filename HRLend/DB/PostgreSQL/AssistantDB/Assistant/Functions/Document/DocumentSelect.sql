CREATE OR REPLACE FUNCTION assistant.document__select(
		_cabinet_id integer
	)RETURNS TABLE (
		id integer,
		title text ,
		elasticsearch_index text
	) AS 
	$$
		BEGIN

			RETURN QUERY
				SELECT 
					a.id AS id,
					a.title AS title,
					a.elasticsearch_index AS elasticsearch_index
				FROM assistant.document AS a
				WHERE _cabinet_id = a.cabinet_id;	
		END;	
	$$
	LANGUAGE plpgsql;


