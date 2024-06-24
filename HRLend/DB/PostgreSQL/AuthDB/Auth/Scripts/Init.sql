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
       ( 1, 'Дельта', '10-06-2023');  -- 2



--pasword = test1
INSERT INTO auth.user ( status_id, cabinet_id, username, email, password_hash, date_create, date_activation)
VALUES (1, 1, 'pavel', 'ibishov.tural20@mail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),  --1
       (1, 1, 'oleg', 'ibishov.tural22@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --2
       (1, 1, 'lena', 'ibishov.tural23@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --3
       (1, 1, 'vasiliu', 'ibishov.tural24@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --4
       (1, 1, 'maxim', 'ibishov.tural25@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --5
       (1, 1, 'ivan', 'ibishov.tural26@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --6
       (1, 1, 'petr', 'ibishov.tural27@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --7
       (1, 1, 'tom', 'ibishov.tural28@yandex.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023');   --8


INSERT INTO auth.user ( status_id, cabinet_id, username, email, password_hash, date_create, date_activation)
VALUES (1, 2, 'админ', 'ibishov.tural30@mail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),  --9
       (1, 2, 'hr', 'atomiccrot@gmail.com', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --10
       (1, 2, 'сотрудник 1', 'sochinskiyartom@gmail.com', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --11
       (1, 2, 'сотрудник 2', 'sochinskiyartem@gmail.com', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --12
       (1, 2, 'сотрудник 3', 'nhczkxl595@1secmail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --13
       (1, 2, 'сотрудник 4', 'nhczkxl596@1secmail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --14
       (1, 2, 'кандидат 1', 'nhczkxl597@1secmail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --15
       (1, 2, 'кандидат 2', 'nhczkxl598@1secmail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --16
       (1, 2, 'кандидат 3', 'nhczkxl599@1secmail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023'),   --17
       (1, 2, 'кандидат 4', 'nhczkxl591@1secmail.ru', '$2a$11$7jh9KCNmLmYjftp8Nxt8AeIUqjCl/012zwTY3QoFyNea4IUG4nX/u', '10-06-2023', '10-06-2023');   --18

INSERT INTO auth.user_info ( user_id)
VALUES (1),  --1
       (2),   --2
       (3),   --3
       (4),   --4
       (5),   --5
       (6),   --6
       (7),   --7
       (8),   --8
       (9),   --9
       (10),   --10
       (11),   --11
       (12),   --12
       (13),   --13
       (14),   --14
       (15),   --15
       (16),   --16
       (17),   --17
       (18);   --18




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
       (9, 3),
       (10, 1),
       (10, 4),
       (11, 1),
       (11, 5),
       (12, 1),
       (12, 5),
       (13, 1),
       (13, 5),
       (14, 1),
       (14, 5),
       (15, 1),
       (15, 6),
       (16, 1),
       (16, 6),
       (17, 1),
       (17, 6),
       (18, 1),
       (18, 6);

INSERT INTO auth.group ( cabinet_id, type_id, title)
VALUES ( 1, 2, 'for_employee'),  -- 1
       ( 1, 1, 'for_candidate'),
       ( 2, 2, 'for_employee'),  -- 3
       ( 2, 1, 'for_candidate');  -- 4

INSERT INTO auth.group_and_user (group_id, user_id)
VALUES (1, 3),
       (1, 4),
       (2, 6),
       (2, 7),
       (3, 11),
       (3, 12),
       (3, 13),
       (3, 14),
       (3, 15),
       (3, 16),
       (3, 17),
       (3, 18);
