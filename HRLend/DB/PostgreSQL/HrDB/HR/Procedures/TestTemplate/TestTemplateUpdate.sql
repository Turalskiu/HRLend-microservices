CREATE OR REPLACE PROCEDURE hr.test_template__update(
		_id integer,
		_title text,
		_competence_ids integer[] default null,
		_competence_need_ids integer[] default null
	) AS
	$$		
		BEGIN
			UPDATE hr.test_template SET title = _title
			WHERE id = _id;
				
			IF  NOT (_competence_ids IS NULL) THEN
				DELETE FROM hr.test_template_and_competence
				WHERE test_template_id = _id;
			
				INSERT INTO hr.test_template_and_competence(
					test_template_id,
					competence_id,
					competence_need_id
				)
				SELECT _id, unnest(_competence_ids), unnest(_competence_need_ids);
			END IF;
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE hr.template__update_constructor(
		_template_json jsonb
	) AS
	$$
		DECLARE competence_json jsonb;

		BEGIN

			IF (_template_json->'IsUpdateBody')::boolean = true THEN
				UPDATE hr.test_template SET
					title = SUBSTRING((_template_json->'Title')::text, 2, LENGTH((_template_json->'Title')::text) - 2)
				WHERE id = (_template_json->>'Id')::integer;
			END IF;

			DELETE FROM hr.test_template_and_competence
			WHERE test_template_id = (_template_json->>'Id')::integer;

			FOR competence_json IN 
				SELECT value FROM jsonb_array_elements(_template_json->'Competencies')
			LOOP

				INSERT INTO hr.test_template_and_competence
				(
					test_template_id,
					competence_id,
					competence_need_id
				) VALUES(
					(_template_json->>'Id')::integer,
					(competence_json->>'Id')::integer,
					(competence_json->>'CompetenceNeed')::integer
				);

				call hr.competence__update_constructor(
					competence_json
				);
			END LOOP;

		END;
    $$
    LANGUAGE plpgsql;

