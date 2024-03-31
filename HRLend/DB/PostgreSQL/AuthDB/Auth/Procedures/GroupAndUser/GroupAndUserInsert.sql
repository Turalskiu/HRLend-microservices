CREATE OR REPLACE PROCEDURE auth.group_and_user__insert(
        _group_id integer,
		_user_id integer
	) AS
	$$		
		BEGIN
			INSERT INTO auth.group_and_user (
					group_id,
					user_id
				) 
				VALUES (
					_group_id,
					_user_id
				);
		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.group_and_user__insert(
        _group_id integer,
		_user_id integer[]
	) AS
	$$
		DECLARE
			i integer;		
		BEGIN
			FOREACH i IN ARRAY _user_id
			LOOP
				INSERT INTO auth.group_and_user (
						group_id,
						user_id
					) 
					VALUES (
						_group_id,
						i
					);
			END LOOP;

		END;
    $$
    LANGUAGE plpgsql;