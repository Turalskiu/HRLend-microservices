CREATE OR REPLACE FUNCTION hr.skill__get(
		_id integer,
		_cabinet_id integer
	) RETURNS TABLE (
		title text,
		test_module_link text
	) AS
	$$
		BEGIN
			RETURN QUERY
				SELECT 
					s.title AS title,
					s.test_module_link AS test_module_link
				FROM hr.skill AS s
				WHERE s.id = _id and s.cabinet_id = _cabinet_id;
		END;
	$$
	LANGUAGE plpgsql;