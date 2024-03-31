CREATE OR REPLACE FUNCTION hr.competence__select(
		_ref_competence refcursor,
		_ref_skill refcursor,
		_cabinet_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN

			CREATE TEMP TABLE temp_competence ON COMMIT DROP AS
      		SELECT * FROM hr.competence AS c
			WHERE c.cabinet_id = _cabinet_id;
		
			OPEN _ref_competence FOR
				SELECT 
					c.id AS id,
					c.title AS title
				FROM temp_competence AS c;
			RETURN NEXT _ref_competence;
			
			OPEN _ref_skill FOR
				SELECT 
					c.id AS competence_id,
					s.id AS skill_id,
					s.title AS skill_title,
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

CREATE OR REPLACE FUNCTION hr.competence__select(
		_ref_competence refcursor,
		_ref_page_info refcursor,
		_cabinet_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_competence FOR
				SELECT 
					c.id AS id,
					c.title AS title
				FROM hr.competence AS c
				WHERE c.cabinet_id = _cabinet_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN c.title END ASC,
					CASE WHEN _sort = 'desc' THEN c.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_competence;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM hr.competence
						WHERE cabinet_id = _cabinet_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;