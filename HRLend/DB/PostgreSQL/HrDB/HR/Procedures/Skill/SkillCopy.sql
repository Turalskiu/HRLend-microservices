CREATE OR REPLACE PROCEDURE hr.skill__copy(
		_cabinet_id integer,
		out _id_skill integer,
		_skill_copy_json jsonb
	) AS
	$$
		BEGIN

			/*
			Если в кабинете имеются аналогичный навык, то узнаем его id,
			и не дублируем одинаковые навыки.
			*/
			SELECT s.id INTO _id_skill
			FROM hr.skill s
			WHERE 
				s.cabinet_id = _cabinet_id AND
				s.title = SUBSTRING((_skill_copy_json->'Title')::text, 2, LENGTH((_skill_copy_json->'Title')::text) - 2);

			IF _id_skill IS NULL THEN
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
				RETURNING id INTO _id_skill;
			END IF;
				
		END;
    $$
    LANGUAGE plpgsql;

