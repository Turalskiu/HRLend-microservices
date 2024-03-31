create table test.test_result
(
    id integer primary key generated by default as identity,
    test_link_response_id integer not null,
    is_passed boolean,
	test_result_link text not null,
    test_template_statistics_link text not null,

    foreign key (test_link_response_id) references test.test_link_response(id) on delete cascade
);