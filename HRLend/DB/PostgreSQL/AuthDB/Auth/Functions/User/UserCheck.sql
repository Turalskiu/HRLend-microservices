CREATE OR REPLACE FUNCTION auth.user__is_included_cabinet(
    _user_id integer,
    _cabinet_id integer
)
  RETURNS BOOLEAN AS $$
    DECLARE result boolean;
    DECLARE user_cab_id integer;
    BEGIN

        SELECT u.cabinet_id INTO user_cab_id
        FROM auth.user AS u
        WHERE u.id = _user_id;

        IF user_cab_id = _cabinet_id THEN
            result := true;
        ELSE
            result := false;
        END IF;

        RETURN result;
    END;
$$
LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user__is_included_cabinet(
    _username text,
    _cabinet_id integer
)
  RETURNS BOOLEAN AS $$
    DECLARE result boolean;
    DECLARE user_cab_id integer;
    BEGIN

        SELECT u.cabinet_id INTO user_cab_id
        FROM auth.user AS u
        WHERE u.username = _username;

        IF user_cab_id = _cabinet_id THEN
            result := true;
        ELSE
            result := false;
        END IF;

        RETURN result;
    END;
$$
LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION auth.user__is_exists_username(
    _username text
)
  RETURNS BOOLEAN AS $$
    DECLARE result boolean;
    DECLARE is_username text;

    BEGIN

        SELECT u.username INTO is_username
        FROM auth.user AS u
        WHERE u.username = _username;

        IF is_username IS NULL THEN
            result := false;
        ELSE
            result := true;
        END IF;

        RETURN result;
    END;
$$
LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION auth.user__is_exists_email(
    _email text
)
  RETURNS BOOLEAN AS $$
    DECLARE result boolean;
    DECLARE is_email text;

    BEGIN

        SELECT u.email INTO is_email
        FROM auth.user AS u
        WHERE u.email = _email;

        IF is_email IS NULL  THEN
            result := false;
        ELSE
            result := true;
        END IF;

        RETURN result;
    END;
$$
LANGUAGE plpgsql;
