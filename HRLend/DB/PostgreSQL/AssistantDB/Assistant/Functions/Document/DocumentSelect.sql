CREATE OR REPLACE FUNCTION assistant.document__select(
		_ref_document refcursor,
		_cabinet_id integer
	) RETURNS SETOF refcursor AS 
	$$
		BEGIN

			OPEN _ref_document FOR
				SELECT 
					a.id AS id,
					a.title AS title,
					a.elasticsearch_index AS elasticsearch_index
				FROM assistant.document AS a
				WHERE _cabinet_id = a.cabinet_id;
			RETURN NEXT _ref_document;		

		END;	
	$$
	LANGUAGE plpgsql;


