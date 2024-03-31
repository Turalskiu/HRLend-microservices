CREATE OR REPLACE FUNCTION test.test_link__is_check_passing_user(
    _link_id integer,
    _user_email text
)
  RETURNS BOOLEAN AS $$
    DECLARE result boolean;
    DECLARE link_id integer;
    BEGIN

        SELECT t_l.id INTO link_id
        FROM test.test_link AS t_l
		JOIN test.test_link_response AS t_l_r ON t_l_r.test_link_id = t_l.id
		JOIN test.anonymous_user AS a_u ON a_u.test_link_response_id = t_l_r.id
        WHERE t_l.id = _link_id AND a_u.email = _user_email;

        IF link_id IS NULL THEN
            result := false;
        ELSE
            result := true;
        END IF;

        RETURN result;
    END;
$$
LANGUAGE plpgsql;
