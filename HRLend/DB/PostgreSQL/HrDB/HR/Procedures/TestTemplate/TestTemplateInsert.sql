CREATE OR REPLACE PROCEDURE hr.test_template__insert(
		_cabinet_id integer,
        _title text,
		out _id_template integer,
		_competence_ids integer[] default null,
		_competence_need_ids integer[] default null
	) AS
	$$
		BEGIN
			INSERT INTO hr.test_template 
			(
				cabinet_id,
				title
			) 
			VALUES 
			(
				_cabinet_id,
				_title
			)
			RETURNING id INTO _id_template;
				
			IF  NOT (_competence_ids IS NULL) THEN
				INSERT INTO hr.test_template_and_competence(
					test_template_id,
					competence_id,
					competence_need_id
				)
				SELECT _id_template, unnest(_competence_ids), unnest(_competence_need_ids);
			END IF;
		END;
    $$
    LANGUAGE plpgsql;