create schema test;create table test.user
(
    id integer primary key,
    username text not null,
    email text not null,
    photo text
);create table test.group
(
    id integer primary key,
    title text not null
);create table test.test_link_status
(
    id integer primary key generated by default as identity,
	title text not null
);create table test.test_link_type
(
    id integer primary key generated by default as identity,
	title text not null
);create table test.test_link_response_status
(
    id integer primary key generated by default as identity,
	title text not null
);create table test.test
(
    id integer primary key generated by default as identity,
	hr_id integer not null,
	title text,
	description text,
	test_template_link text not null,

	foreign key (hr_id) references test.user(id) on delete cascade
);create table test.test_link
(
    id integer primary key generated by default as identity,
    test_id integer not null,
    status_id integer not null,
	type_id integer not null,
    user_id integer,
    group_id integer,
    limit_candidate_count integer,
    limit_attempt integer not null,
    candidate_count integer not null,
    title text not null,
	link text not null,
	date_create timestamp(6) with time zone not null,
    date_expired timestamp(6) with time zone not null,
    date_closed timestamp(6) with time zone,
    
	
	foreign key (test_id) references test.test(id) on delete cascade,
	foreign key (status_id) references test.test_link_status(id),
    foreign key (type_id) references test.test_link_type(id),
    foreign key (user_id) references test.user(id) on delete cascade,
    foreign key (group_id) references test.group(id) on delete cascade
);create table test.test_link_response
(
    id integer primary key generated by default as identity,
    test_link_id integer not null,
    status_id integer not null,
    number_attempt integer not null,
    user_id integer,
	date_create timestamp(6) with time zone not null,
    test_generated_link text,
	
	foreign key (test_link_id) references test.test_link(id) on delete cascade,
	foreign key (status_id) references test.test_link_response_status(id),
    foreign key (user_id) references test.user(id) on delete cascade
);create table test.anonymous_user
(
    id integer primary key generated by default as identity,
    test_link_response_id integer not null,
    first_name text not null,
    middle_name text not null,
    last_name text not null,
    email text not null,

    foreign key (test_link_response_id) references test.test_link_response(id) on delete cascade
);create table test.test_result
(
    id integer primary key generated by default as identity,
    test_link_response_id integer not null,
    is_passed boolean,
	test_result_link text not null,
    test_template_statistics_link text not null,

    foreign key (test_link_response_id) references test.test_link_response(id) on delete cascade
);CREATE OR REPLACE FUNCTION test.test__get(
		_ref_test refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test FOR
				SELECT 
					t.title AS title,
					t.description AS description,
					t.test_template_link AS test_template_link
				FROM test.test AS t
				WHERE t.id = _id;		
			RETURN NEXT _ref_test;
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test_and_test_link__get(
		_ref_test refcursor,
		_ref_test_link refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test FOR
				SELECT 
					t.title AS title,
					t.description AS description,
					t.test_template_link AS test_template_link
				FROM test.test AS t
				WHERE t.id = _id;		
			RETURN NEXT _ref_test;
				
			OPEN _ref_test_link FOR
				SELECT 
					t_l.id AS id,
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title
				FROM  test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				where t_l.test_id = _id;
			RETURN NEXT _ref_test_link;
			
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test__get_test_template_link(
		_id integer
	) RETURNS TEXT AS
	$$
		DECLARE
    		result TEXT;
		BEGIN
			
			SELECT 
				t.test_template_link INTO result
			FROM test.test AS t
			WHERE t.id = _id;

			IF result IS NULL THEN
				result := '';
			END IF;

			RETURN result;
		END;	
	$$
	LANGUAGE plpgsql;CREATE OR REPLACE FUNCTION test.test__select(
		_ref_test refcursor,
		_ref_page_info refcursor,
		_hr_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test FOR
				SELECT 
					t.id AS id,
					t.title AS title
				FROM test.test AS t
				WHERE t.hr_id = _hr_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t.title END ASC,
					CASE WHEN _sort = 'desc' THEN t.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM test.test
						WHERE hr_id = _hr_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;CREATE OR REPLACE FUNCTION test.test_link__is_check_passing_user(
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
CREATE OR REPLACE FUNCTION test.test_link__get_by_link(
		_ref_test_link refcursor,
		_link text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test_link FOR
				SELECT 
					t_l.id AS id,
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title,
					t_l.type_id AS type_id,
					t_l_t.title AS type_title,
					t_l.user_id AS user_id,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					t_l.group_id AS group_id,
					t_l.test_id AS test_id,
					g.title AS group_title,
					t_l.limit_candidate_count AS limit_candidate_count,
					t_l.limit_attempt AS limit_attempt,
					t_l.candidate_count AS candidate_count,
					t_l.date_create AS date_create,
					t_l.date_closed AS date_closed,
					t_l.date_expired AS date_expired
				FROM test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				JOIN test.test_link_type AS t_l_t ON t_l_t.id = t_l.type_id
				LEFT JOIN test.user AS u ON u.id = t_l.user_id
				LEFT JOIN test.group AS g ON g.id = t_l.group_id
				WHERE t_l.link = _link;
			RETURN NEXT _ref_test_link;	
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test_link__get(
		_ref_test_link refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			OPEN _ref_test_link FOR
				SELECT 
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title,
					t_l.type_id AS type_id,
					t_l_t.title AS type_title,
					t_l.user_id AS user_id,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					t_l.group_id AS group_id,
					t_l.test_id AS test_id,
					g.title AS group_title,
					t_l.limit_candidate_count AS limit_candidate_count,
					t_l.limit_attempt AS limit_attempt,
					t_l.candidate_count AS candidate_count,
					t_l.link AS link,
					t_l.date_create AS date_create,
					t_l.date_closed AS date_closed,
					t_l.date_expired AS date_expired
				FROM test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				JOIN test.test_link_type AS t_l_t ON t_l_t.id = t_l.type_id
				LEFT JOIN test.user AS u ON u.id = t_l.user_id
				LEFT JOIN test.group AS g ON g.id = t_l.group_id
				WHERE t_l.id = _id;
			RETURN NEXT _ref_test_link;	
		END;	
	$$
	LANGUAGE plpgsql;CREATE OR REPLACE FUNCTION test.test_link__select(
		_ref_test_link refcursor,
		_ref_page_info refcursor,
		_hr_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test_link FOR
				SELECT 
					t_l.id AS id,
					t_l.title AS title,
					t_l.status_id AS status_id,
					t_l_s.title AS status_title
				FROM test.test_link AS t_l
				JOIN test.test_link_status AS t_l_s ON t_l_s.id = t_l.status_id
				JOIN test.test AS t ON t.id = t_l.test_id
				WHERE t.hr_id = _hr_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t_l.title END ASC,
					CASE WHEN _sort = 'desc' THEN t_l.title END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test_link;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM test.test_link AS t_l
						JOIN test.test AS t ON t.id = t_l.test_id
						WHERE t.hr_id = _hr_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;CREATE OR REPLACE FUNCTION test.test_link_response__count(
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
CREATE OR REPLACE FUNCTION test.test_link_and_user_response__get(
		_ref_test_link_response refcursor,
		_id integer
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
			
			OPEN _ref_test_link_response FOR
				SELECT 
					t_l_r.date_create AS date_create,
					t_l_r.test_link_id AS test_link_id,
					t_l_r.test_generated_link AS test_generated_link,
					t_l_r.user_id AS user_id,
					t_l_r.number_attempt AS number_attempt,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					a_u.id AS anonymous_user_id,
					a_u.first_name AS anonymous_user_first_name,
					a_u.last_name AS anonymous_user_last_name,
					a_u.middle_name AS anonymous_user_middle_name,
					a_u.email AS anonymous_user_email,
					t_l_r_s.id AS status_id,
					t_l_r_s.title AS status_title,
					t_r.id AS test_result_id,
					t_r.test_result_link AS test_result_link,
					t_r.test_template_statistics_link AS test_template_statistics_link,
					t_r.is_passed AS test_is_passed
				FROM test.test_link_response AS t_l_r
				JOIN test.test_link_response_status AS t_l_r_s ON t_l_r.status_id = t_l_r_s.id
				LEFT JOIN test.test_result AS t_r ON t_r.test_link_response_id = t_l_r.id
				LEFT JOIN test.anonymous_user AS a_u ON a_u.test_link_response_id = _id
				LEFT JOIN test.user AS u ON u.id = t_l_r.user_id
				WHERE t_l_r.id = _id;
			RETURN NEXT _ref_test_link_response;
			
		END;	
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION test.test_link_response__get_test_result_link(
		_id integer
	) RETURNS TEXT AS
	$$
		DECLARE
    		result TEXT;
		BEGIN
			
			SELECT 
				t_r.test_result_link INTO result
			FROM test.test_link_response AS t_l_r
			JOIN test.test_result AS t_r ON t_r.test_link_response_id = _id
			WHERE t_l_r.id = _id;

			IF result IS NULL THEN
				result := '';
			END IF;

			RETURN result;
		END;	
	$$
	LANGUAGE plpgsql;CREATE OR REPLACE FUNCTION test.test_link_response_and_user__select(
		_ref_test_link_response refcursor,
		_ref_page_info refcursor,
		_test_link_id integer,
		_page_number integer,
		_page_size integer,
		_sort text
	) RETURNS SETOF refcursor AS
	$$
		BEGIN
		
			OPEN _ref_test_link_response FOR
				SELECT 
					t_l_r.id AS id,
					t_l_r.date_create AS date_create,
					t_l_r.status_id AS status_id,
					t_l_r.number_attempt AS number_attempt,
					t_l_r_s.title AS status_title,
					a_u.id AS anonymous_user_id,
					a_u.first_name AS anonymous_user_first_name,
					a_u.email AS anonymous_user_email,
					a_u.middle_name AS anonymous_user_middle_name,
					a_u.last_name AS anonymous_user_last_name,
					t_l_r.user_id AS user_id,
					u.username AS username,
					u.email AS user_email,
					u.photo AS user_photo,
					t_r.id AS test_result_id,
					t_r.test_result_link AS test_result_link,
					t_r.test_template_statistics_link AS test_template_statistics_link,
					t_r.is_passed AS test_is_passed
				FROM test.test_link_response AS t_l_r
				JOIN test.test_link_response_status AS t_l_r_s ON t_l_r.status_id = t_l_r_s.id
				LEFT JOIN test.anonymous_user AS a_u ON a_u.test_link_response_id = t_l_r.id
				LEFT JOIN test.user AS u ON u.id = t_l_r.user_id
				LEFT JOIN test.test_result AS t_r ON t_r.test_link_response_id = t_l_r.id
				WHERE t_l_r.test_link_id = _test_link_id
				ORDER BY
					CASE WHEN _sort = 'asc' THEN t_l_r.date_create END ASC,
					CASE WHEN _sort = 'desc' THEN t_l_r.date_create END DESC
				LIMIT _page_size OFFSET (_page_number-1)*_page_size;		
			RETURN NEXT _ref_test_link_response;
			
			OPEN _ref_page_info FOR
				SELECT
					(
						SELECT COUNT(*)::integer FROM test.test_link_response AS t_l_r
						WHERE t_l_r.test_link_id = _test_link_id
					) AS "TotalRows",
					_page_number AS "PageNo",
					_page_size AS "PageSize",
					_sort AS "Sort";	
			RETURN NEXT _ref_page_info;
			
		END;
    $$
	LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.anonymous_user__insert(
		_test_link_response_id integer,
		_first_name text,
		_last_name text,
		_middle_name text,
		_email text,
		out _id_anonymous_user integer
	) AS
	$$
		BEGIN
			INSERT INTO test.anonymous_user
			(
				test_link_response_id,
				first_name,
				last_name,
				middle_name,
				email
			) VALUES 
			(
				_test_link_response_id,
				_first_name,
				_last_name,
				_middle_name,
				_email
			)
			RETURNING id INTO _id_anonymous_user;	
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.group__delete(
        _group_id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.group
      		WHERE id = _group_id;
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.group__insert(
		_id integer,
        _title text
	) AS
	$$		
		BEGIN
			INSERT INTO test.group (
					id,
					title
				) 
				VALUES (
					_id,
					_title
				);
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.group__update(
		_id integer,
		_title text
	) AS
	$$		
		BEGIN
			UPDATE test.group SET 
				title = _title
			WHERE _id = id; 
		END;
	$$
	LANGUAGE plpgsql;
CREATE OR REPLACE PROCEDURE test.test__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.test
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.test__insert(
		_hr_id integer,
        _title text,
		_description text,
		_test_template_link text,
		out _id_test integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test 
			(
				hr_id,
				title, 
				description,
				test_template_link
			) VALUES 
			(
				_hr_id,
				_title, 
				_description,
				_test_template_link
			)
			RETURNING id INTO _id_test;
				
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.test__update(
		_id integer,
		_title text,
		_description text
	) AS
	$$		
		BEGIN
			UPDATE test.test
			SET 
				title = _title,
				description = _description
			WHERE id = _id;
				
		END;
	$$
	LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE test.test_link__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.test_link
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.test_link__insert(
		_test_id integer,
        _status_id integer,
		_type_id integer,
		_user_id integer,
		_group_id integer,
		_limit_candidate_count integer,
		_limit_attempt integer,
		_title text,
		_link text,
		_date_create timestamp(6) with time zone,
		_date_expired timestamp(6) with time zone,
		out _id_test_link integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test_link
			(
				test_id,
				status_id,
				type_id,
				user_id,
				group_id,
				limit_candidate_count,
				limit_attempt,
				candidate_count,
				title,
				link,
				date_create,
				date_expired
			) VALUES 
			(
				_test_id,
				_status_id,
				_type_id,
				_user_id,
				_group_id,
				_limit_candidate_count,
				_limit_attempt,
				0,
				_title,
				_link,
				_date_create,
				_date_expired
			)
			RETURNING id INTO _id_test_link;
				
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.test_link__update(
		_id integer,
		_status_id integer
	) AS
	$$		
		BEGIN
			UPDATE test.test_link
			SET 
				status_id = _status_id
			WHERE id = _id;		
		END;
	$$
	LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE test.test_link__closed(
		_id integer,
		_status_id integer,
		_date_closed timestamp(6) with time zone
	) AS
	$$		
		BEGIN
			UPDATE test.test_link
			SET 
				status_id = _status_id,
				date_closed = _date_closed
			WHERE id = _id;		
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE test.test_link__candidate_count_increase(
		_id integer
	) AS
	$$		
		BEGIN
			UPDATE test.test_link 
			SET
				candidate_count = (candidate_count + 1)
			WHERE id =_id;
		END;
	$$
	LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE test.test_link_response__delete(
        _id integer
	) AS
	$$		
		BEGIN
			DELETE FROM test.test_link_response
      		WHERE id = _id;
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.test_link_response__insert(
		_test_link_id integer,
        _status_id integer,
		_user_id integer,
		_number_attempt integer,
		_test_generated_link text,
		_date_create timestamp(6) with time zone,
		out _id_test_link_response integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test_link_response
			(
				test_link_id,
				status_id,
				user_id,
				number_attempt,
				test_generated_link,
				date_create
			) VALUES 
			(
				_test_link_id,
				_status_id,
				_user_id,
				_number_attempt,
				_test_generated_link,
				_date_create
			)
			RETURNING id INTO _id_test_link_response;	
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.test_link_response__update(
		_id integer,
		_status_id integer
	) AS
	$$		
		BEGIN
			UPDATE test.test_link_response
			SET 
				status_id = _status_id
			WHERE id = _id;		
		END;
	$$
	LANGUAGE plpgsql;



CREATE OR REPLACE PROCEDURE test.test_result__insert(
		_test_link_response_id integer,
        _is_passed boolean,
		_test_result_link text,
		_test_template_statistics_link text,
		out _id_test_result integer
	) AS
	$$
		BEGIN
			INSERT INTO test.test_result
			(
				test_link_response_id,
				is_passed,
				test_result_link,
				test_template_statistics_link
			) VALUES 
			(
				_test_link_response_id,
				_is_passed,
				_test_result_link,
				_test_template_statistics_link
			)
			RETURNING id INTO _id_test_result;
				
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.user__insert(
		_id integer,
		_username text,
		_photo text,
		_email text
	) AS
	$$
		BEGIN
			INSERT INTO test.user
			(
				id,
				username,
				photo,
				email
			) VALUES 
			(
				_id,
				_username,
				_photo,
				_email
			);
		END;
    $$
    LANGUAGE plpgsql;CREATE OR REPLACE PROCEDURE test.user__update_username(
		_id integer,
		_username text
	) AS
	$$		
		BEGIN
			UPDATE test.user SET 
				username = _username
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE test.user__update_photo(
		_id integer,
		_photo text
	) AS
	$$		
		BEGIN
			UPDATE test.user SET 
				photo = _photo
			WHERE id = _id; 
		END;
	$$
	LANGUAGE plpgsql;


INSERT INTO test.test_link_type (id, title)
VALUES (1, 'for_user'),
       (2, 'for_group'),
       (3, 'for_anonymous_user'),
       (4, 'for_anonymous_group');

INSERT INTO test.test_link_status (id, title)
VALUES (1, 'open'),
       (2, 'closed'),
       (3, 'expired'),
       (4, 'limit');


INSERT INTO test.test_link_response_status (id, title)
VALUES (1, 'respond'),
       (2, 'start_test'),
       (3, 'end_test'),
       (4, 'overdue_test');

INSERT INTO test.user (id, username, email)
VALUES (1, 'pavel', 'ibishov.tural20@mail.ru'),  --1
       (2, 'oleg', 'ibishov.tural22@yandex.ru'),   --2
       (3, 'lena', 'ibishov.tural23@yandex.ru'),   --3
       (4, 'vasiliu', 'ibishov.tural24@yandex.ru'),   --4
       (5, 'maxim', 'ibishov.tural25@yandex.ru'),   --5
       (6, 'ivan', 'ibishov.tural26@yandex.ru'),   --6
       (7,'petr', 'ibishov.tural27@yandex.ru'),   --7
       (8,'tom', 'ibishov.tural28@yandex.ru');   --8

INSERT INTO test.user ( id, username, email)
VALUES (9, 'админ', 'ibishov.tural30@mail.ru'),  --9
       (10, 'hr', 'atomiccrot@gmail.com'),   --10
       (11, 'сотрудник 1', 'sochinskiyartom@gmail.com'),   --11
       (12, 'сотрудник 2', 'sochinskiyartem@gmail.com'),   --12
       (13, 'сотрудник 3', 'nhczkxl595@1secmail.ru'),   --13
       (14, 'сотрудник 4', 'nhczkxl596@1secmail.ru'),   --14
       (15, 'кандидат 1', 'nhczkxl597@1secmail.ru'),   --15
       (16, 'кандидат 2', 'nhczkxl598@1secmail.ru'),   --16
       (17, 'кандидат 3', 'nhczkxl599@1secmail.ru'),   --17
       (18, 'кандидат 4', 'nhczkxl591@1secmail.ru');   --18


INSERT INTO test.group (id, title)
VALUES (1, 'for_employee'),  -- 1
       (2, 'for_candidate'), -- 2
       ( 3, 'for_employee'),  -- 3
       ( 4, 'for_candidate');  -- 4; 
