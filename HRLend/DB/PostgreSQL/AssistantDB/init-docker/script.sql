create schema assistant;create table assistant.document_type
(
    id integer primary key generated by default as identity,
    title text not null
);create table assistant.document
(
    id integer primary key generated by default as identity,
    type_id integer not null,
    cabinet_id integer not null,
    title text not null,

    elasticsearch_index text not null,
    foreign key (type_id) references assistant.document_type(id),

    unique(cabinet_id, title)
);CREATE OR REPLACE FUNCTION assistant.document__get(
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
	LANGUAGE plpgsql;CREATE OR REPLACE FUNCTION assistant.document__select(
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


CREATE OR REPLACE PROCEDURE assistant.document__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM assistant.document
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE assistant.document__insert(
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
    LANGUAGE plpgsql;INSERT INTO assistant.document_type (id, title)
VALUES (1, 'for_test'),
       (2, 'for_company_info');