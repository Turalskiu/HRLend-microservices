INSERT INTO auth.role (id, title)
VALUES (1, 'user'),
       (2, 'admin'),
       (3, 'cabinet_admin'),
       (4, 'cabinet_hr'),
       (5, 'cabinet_employee'),
       (6, 'cabinet_candidate');


INSERT INTO auth.user_status (id, title)
VALUES (1, 'activated'),
       (2, 'blocked'),
       (3, 'deleted');


INSERT INTO auth.cabinet_status (id, title)
VALUES (1, 'activated'),
       (2, 'blocked'),
       (3, 'deleted');


INSERT INTO auth.group_type (id, title)
VALUES (1, 'candidate'),
       (2, 'employee');


INSERT INTO auth.cabinet ( status_id, title, date_create)
VALUES ( 1, 'cab1', '10-06-2023'),  -- 1
       ( 1, 'cab2', '10-06-2023');  -- 2



--pasword = test1
INSERT INTO auth.user ( status_id, cabinet_id, username, email, password_hash, date_create, date_activation)
VALUES (1, 1, 'pavel', 'ibishov.tural20@mail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),  --1
       (1, 1, 'oleg', 'ibishov.tural22@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --2
       (1, 1, 'lena', 'ibishov.tural23@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --3
       (1, 1, 'vasiliu', 'ibishov.tural24@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --4
       (1, 1, 'maxim', 'ibishov.tural25@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --5
       (1, 1, 'ivan', 'ibishov.tural26@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --6
       (1, 1, 'petr', 'ibishov.tural27@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --7
       (1, 1, 'tom', 'ibishov.tural28@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --8
       (1, 2, 'jon', 'ibishov.tural30@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023');   --9


INSERT INTO auth.user_info ( user_id)
VALUES (1),  --1
       (2),   --2
       (3),   --3
       (4),   --4
       (5),   --5
       (6),   --6
       (7),   --7
       (8),   --8
       (9);   --9




INSERT INTO auth.user_and_role ( user_id, role_id)
VALUES ( 1, 1),
       ( 1, 2), 
       (1, 3),
       (2, 1),
       (2, 4), 
       (3, 1),
       (3, 5),
       (4,1),
       (4,5),
       (5,1),
       (5,5),
       (6, 1),
       (6, 6),
       (7,1),
       (7,6),
       (8,1),
       (8,6),
       (9, 1),
       (9, 3);

INSERT INTO auth.group ( cabinet_id, type_id, title)
VALUES ( 1, 2, 'for_employee'),  -- 1
       ( 1, 1, 'for_candidate');  -- 2

INSERT INTO auth.group_and_user (group_id, user_id)
VALUES (1, 3),
       (1, 4),
       (2, 6),
       (2, 7);
