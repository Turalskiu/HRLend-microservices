create table auth.registration_token
(
    id integer primary key generated by default as identity,
    user_id integer not null,
    token text not null,
    expires timestamp(6) with time zone not null,
    created timestamp(6) with time zone not null,
    created_by_ip text not null,

    cabinet integer,
    cabinet_role integer,

	foreign key (user_id) references auth.user(id) on delete cascade
);