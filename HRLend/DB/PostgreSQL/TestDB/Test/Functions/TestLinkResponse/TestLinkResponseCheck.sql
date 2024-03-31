CREATE OR REPLACE FUNCTION test.test_link_response__count(
    _link_id integer,
    _user_id integer
)
  RETURNS integer AS $$
    DECLARE count_passing integer;
    BEGIN

        SELECT COUNT(*) INTO count_passing
        FROM test.test_link_response AS t_l_r
        WHERE t_l_r.user_id = _user_id AND t_l_r.test_link_id = _link_id;

        RETURN count_passing;
    END;
$$
LANGUAGE plpgsql;
