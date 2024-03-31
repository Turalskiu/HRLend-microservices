create table auth.user_and_role
(
    user_id integer not null,
    role_id integer not null,

	foreign key (user_id) references auth.user(id) on delete cascade,
    foreign key (role_id) references auth.role(id) on delete cascade
);