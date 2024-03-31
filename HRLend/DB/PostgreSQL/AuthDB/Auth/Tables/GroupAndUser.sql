create table auth.group_and_user
(
    group_id integer not null,
    user_id integer not null,

	foreign key (user_id) references auth.user(id) on delete cascade,
    foreign key (group_id) references auth.group(id) on delete cascade
);