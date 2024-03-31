CREATE OR REPLACE PROCEDURE hr.competence__copy(
		_cabinet_id integer,
		out _id_competence integer,
		_competence_copy_json jsonb
	) AS
	$$
		DECLARE skill_json jsonb;

		BEGIN

			/*
			Если в кабинете имеются аналогичная компетенция, то узнаем его id,
			и не дублируем.
			*/
			SELECT c.id INTO _id_competence
			FROM hr.competence c
			WHERE 
				c.cabinet_id = _cabinet_id AND
				c.title = SUBSTRING((_competence_copy_json->'Title')::text, 2, LENGTH((_competence_copy_json->'Title')::text) - 2);

			IF _id_competence IS NULL THEN
				INSERT INTO hr.competence 
				(
					cabinet_id,
					title
				) 
				VALUES 
				(
					_cabinet_id,
					SUBSTRING((_competence_copy_json->'Title')::text, 2, LENGTH((_competence_copy_json->'Title')::text) - 2)
				)
				RETURNING id INTO _id_competence;

				FOR skill_json IN 
					SELECT value FROM jsonb_array_elements(_competence_copy_json->'Skills')
				LOOP
					call hr.hellper__copy_skill(
						_cabinet_id,
						_id_competence,
						skill_json
					);
				END LOOP;
			END IF;

		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE hr.hellper__copy_skill(
		_cabinet_id integer,
		_id_competence integer,
		_skill_copy_json jsonb
	) AS
	$$
		DECLARE skill_id integer;

		BEGIN
			/*
			Если в кабинете имеются аналогичный навык, то узнаем его id,
			и не дублируем.
			*/
			SELECT s.id INTO skill_id
			FROM hr.skill s
			WHERE 
				s.cabinet_id = _cabinet_id AND
				s.title = SUBSTRING((_skill_copy_json->'Title')::text, 2, LENGTH((_skill_copy_json->'Title')::text) - 2);

			IF skill_id IS NULL THEN
				INSERT INTO hr.skill 
				(
					cabinet_id,
					title, 
					test_module_link
				) VALUES 
				(
					_cabinet_id,
					 SUBSTRING((_skill_copy_json->'Title')::text, 2, LENGTH((_skill_copy_json->'Title')::text) - 2),
					 SUBSTRING((_skill_copy_json->'TestModuleLink')::text, 2, LENGTH((_skill_copy_json->'TestModuleLink')::text) - 2)
				)
				RETURNING id INTO skill_id;
			END IF;

			/*
			Соединяем компетенцию с навыком
			*/
			INSERT INTO hr.competence_and_skill
			(
				competence_id,
				skill_id,
				skill_need_id
			) VALUES(
				_id_competence,
				skill_id,
				(_skill_copy_json->>'SkillNeed')::integer
			);
				
		END;
    $$
    LANGUAGE plpgsql;