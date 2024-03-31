CREATE OR REPLACE PROCEDURE auth.group_and_user__delete(
        _group_id integer,
		_user_id integer
	) AS
	$$		
		BEGIN
			DELETE FROM auth.group_and_user
      		WHERE group_id = _group_id AND user_id = _user_id;
		END;
    $$
    LANGUAGE plpgsql;