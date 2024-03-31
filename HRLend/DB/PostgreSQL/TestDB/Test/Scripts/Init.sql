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
       (8,'tom', 'ibishov.tural28@yandex.ru'),   --8
       (9,'jon', 'ibishov.tural30@yandex.ru');   --9

INSERT INTO test.group (id, title)
VALUES (1, 'for_employee'),  -- 1
       (2, 'for_candidate');  -- 2
