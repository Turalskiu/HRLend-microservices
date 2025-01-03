create table auth.group
(
    id integer primary key generated by default as identity,
    cabinet_id integer not null,
    type_id integer not null,
    title text not null,

    foreign key (cabinet_id) references auth.cabinet(id),
    foreign key (type_id) references auth.group_type(id)
);