CREATE OR REPLACE PROCEDURE auth.user__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM auth.user
      		WHERE id = _id;

			DELETE FROM auth.user_info
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE auth.user__delete_role(
		_user_id integer,
		_role_id integer
	) AS
	$$		
		BEGIN

			DELETE FROM auth.user_and_role
      		WHERE user_id = _user_id AND role_id = _role_id;

		END;
	$$
	LANGUAGE plpgsql;