CREATE OR REPLACE FUNCTION hr.test_template_and_competence_and_skill__get(
		_ref_test_template refcursor, 
		_ref_competence refcursor, 
		_ref_skill refcursor,
		_id integer,
		_cabinet_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN

			OPEN _ref_test_template FOR
				SELECT 
					t_t.title AS title
				FROM hr.test_template  t_t
				WHERE id = _id AND t_t.cabinet_id = _cabinet_id;
			RETURN NEXT _ref_test_template ;
			
			CREATE TEMP TABLE temp_competence ON COMMIT DROP AS
      		SELECT * FROM hr.competence AS c
            	JOIN hr.test_template_and_competence AS t_t_a_c ON c.id = t_t_a_c.competence_id AND c.cabinet_id = _cabinet_id
			WHERE t_t_a_c.test_template_id = _id;
   
   			OPEN _ref_competence FOR
				SELECT 
					c.id AS competence_id,
					c.title AS competence_title,
					c.competence_need_id AS competence_need_id,
					c_n.title AS competence_need_title
				FROM temp_competence AS c
				JOIN hr.competence_need AS c_n ON c_n.id = c.competence_need_id;
   			RETURN NEXT _ref_competence;
				
			OPEN _ref_skill FOR
				SELECT 
					c.id AS competence_id,
					s.id AS skill_id,
					s.title AS skill_title,
					s.test_module_link AS skill_test_module_link,
					s_n.id AS skill_need_id,
					s_n.title AS skill_need_title
				FROM  temp_competence AS c
				JOIN hr.competence_and_skill AS c_a_s ON c_a_s.competence_id = c.id
				JOIN hr.skill AS s ON c_a_s.skill_id = s.id
				join hr.skill_need AS s_n ON s_n.id = c_a_s.skill_need_id;
			RETURN NEXT _ref_skill;
			
		END;
	$$
	LANGUAGE plpgsql;