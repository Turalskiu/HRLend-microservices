create table assistant.document
(
    id integer primary key generated by default as identity,
    cabinet_id integer not null,
    title text not null,
    elasticsearch_index text not null,

    unique(cabinet_id, title)
);