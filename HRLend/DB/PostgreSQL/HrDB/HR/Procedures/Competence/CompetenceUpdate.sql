CREATE OR REPLACE PROCEDURE hr.competence__update(
		_id integer,
		_title text,
		_skill_ids integer[] default null,
		_skill_need_ids integer[] default null
	) AS
	$$		
		BEGIN
			UPDATE hr.competence SET title = _title
			WHERE id = _id;
				
			IF  NOT (_skill_ids IS NULL) THEN
				DELETE FROM hr.competence_and_skill
				WHERE competence_id = _id;
			
				INSERT INTO hr.competence_and_skill(
					competence_id,
					skill_id,
					skill_need_id
				)
				SELECT _id, unnest(_skill_ids), unnest(_skill_need_ids);
			END IF;
		END;
	$$
	LANGUAGE plpgsql;



CREATE OR REPLACE PROCEDURE hr.competence__update_constructor(
		_competence_json jsonb
	) AS
	$$
		DECLARE skill_json jsonb;

		BEGIN

			IF (_competence_json->'IsUpdateBody')::boolean = true THEN
				UPDATE hr.competence SET
					title = SUBSTRING((_competence_json->'Title')::text, 2, LENGTH((_competence_json->'Title')::text) - 2)
				WHERE id = (_competence_json->>'Id')::integer;
			END IF;

			DELETE FROM hr.competence_and_skill
			WHERE competence_id = (_competence_json->>'Id')::integer;

			FOR skill_json IN 
				SELECT value FROM jsonb_array_elements(_competence_json->'Skills')
			LOOP

				INSERT INTO hr.competence_and_skill
				(
					competence_id,
					skill_id,
					skill_need_id
				) VALUES(
					(_competence_json->>'Id')::integer,
					(skill_json->>'Id')::integer,
					(skill_json->>'SkillNeed')::integer
				);

				call hr.hellper__update_skill_constructor(
					skill_json
				);
			END LOOP;

		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE hr.hellper__update_skill_constructor(
		_skill_json jsonb
	) AS
	$$

		BEGIN

			IF (_skill_json->'IsUpdateBody')::boolean = true THEN
				UPDATE hr.skill SET
					title = SUBSTRING((_skill_json->'Title')::text, 2, LENGTH((_skill_json->'Title')::text) - 2)
				WHERE id = (_skill_json->>'Id')::integer;
			END IF;
				
		END;
    $$
    LANGUAGE plpgsql;

