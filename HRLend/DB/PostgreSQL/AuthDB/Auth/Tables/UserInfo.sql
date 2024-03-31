create table auth.user_info
(
    user_id integer not null unique,

    first_name text,
    last_name text,
    middle_name text,
    age integer
);