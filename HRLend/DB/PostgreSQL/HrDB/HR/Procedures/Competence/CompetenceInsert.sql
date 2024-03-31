CREATE OR REPLACE PROCEDURE hr.competence__insert(
		_cabinet_id integer,
        _title text,
		out _id_competence integer,
		_skill_ids integer[] default null,
		_skill_need_ids integer[] default null
	) AS
	$$
		BEGIN
			INSERT INTO hr.competence 
			(
				cabinet_id,
				title
			) 
			VALUES 
			(
				_cabinet_id,
				_title
			)
			RETURNING id INTO _id_competence;
				
			IF NOT (_skill_ids IS NULL) THEN
				INSERT INTO hr.competence_and_skill(
					competence_id,
					skill_id,
					skill_need_id
				)
				SELECT _id_competence, unnest(_skill_ids), unnest(_skill_need_ids);
			END IF;
		END;
    $$
    LANGUAGE plpgsql;