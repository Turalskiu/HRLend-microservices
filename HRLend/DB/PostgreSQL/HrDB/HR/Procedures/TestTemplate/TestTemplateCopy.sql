CREATE OR REPLACE PROCEDURE hr.test_template__copy(
		_cabinet_id integer,
		out _id_test_template integer,
		_template_copy_json jsonb
	) AS
	$$
		DECLARE competence_json jsonb;

		BEGIN

			/*
			Если в кабинете имеются аналогичный шаблон, то узнаем его id,
			и не дублируем.
			*/
			SELECT t.id INTO _id_test_template
			FROM hr.test_template t
			WHERE 
				t.cabinet_id = _cabinet_id AND
				t.title = SUBSTRING((_template_copy_json->'Title')::text, 2, LENGTH((_template_copy_json->'Title')::text) - 2);

			IF _id_test_template IS NULL THEN
				INSERT INTO hr.test_template 
				(
					cabinet_id,
					title
				) 
				VALUES 
				(
					_cabinet_id,
					SUBSTRING((_template_copy_json->'Title')::text, 2, LENGTH((_template_copy_json->'Title')::text) - 2)
				)
				RETURNING id INTO _id_test_template;

				FOR competence_json IN 
					SELECT value FROM jsonb_array_elements(_template_copy_json->'Competencies')
				LOOP
					call hr.hellper__copy_competence(
						_cabinet_id,
						_id_test_template,
						competence_json
					);
				END LOOP;
			END IF;

		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE hr.hellper__copy_competence(
		_cabinet_id integer,
		_id_test_template integer,
		_competence_copy_json jsonb
	) AS
	$$
		DECLARE 
			skill_json jsonb;
			competence_id integer;
		BEGIN

			/*
			Если в кабинете имеются аналогичная компетенция, то узнаем его id,
			и не дублируем.
			*/
			SELECT c.id INTO competence_id
			FROM hr.competence c
			WHERE 
				c.cabinet_id = _cabinet_id AND
				c.title = SUBSTRING((_competence_copy_json->'Title')::text, 2, LENGTH((_competence_copy_json->'Title')::text) - 2);

			IF competence_id IS NULL THEN
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
				RETURNING id INTO competence_id;

				FOR skill_json IN 
					SELECT value FROM jsonb_array_elements(_competence_copy_json->'Skills')
				LOOP
					call hr.hellper__copy_skill(
						_cabinet_id,
						competence_id,
						skill_json
					);
				END LOOP;
			END IF;

			/*
			Соединяем шаблон с компетенцией
			*/
			INSERT INTO hr.test_template_and_competence
			(
				test_template_id,
				competence_id,
				competence_need_id
			) VALUES(
				_id_test_template,
				competence_id,
				(_competence_copy_json->>'CompetenceNeed')::integer
			);


		END;
    $$
    LANGUAGE plpgsql;

