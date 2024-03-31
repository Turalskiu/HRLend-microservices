CREATE OR REPLACE FUNCTION hr.competence_and_skill__get(
		_ref_competence refcursor,
		_ref_skill refcursor,
		_id integer,
		_cabinet_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_competence  FOR
				SELECT
					c.title AS title
				FROM hr.competence  c
				WHERE c.id = _id AND c.cabinet_id = _cabinet_id;
			RETURN NEXT _ref_competence;
				
			OPEN _ref_skill FOR
				SELECT 
					s.id AS skill_id,
					s.title AS skill_title,
					s_n.title AS skill_need_title,
					s_n.id AS skill_need_id
				FROM  hr.skill AS s
				JOIN hr.competence_and_skill AS c_a_s ON c_a_s.skill_id = s.id
				JOIN hr.skill_need AS s_n ON s_n.id = c_a_s.skill_need_id
				where c_a_s.competence_id = _id AND s.cabinet_id = _cabinet_id;
			RETURN NEXT _ref_skill;
			
		END;	
	$$
	LANGUAGE plpgsql;