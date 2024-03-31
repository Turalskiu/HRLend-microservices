create table hr.test_template_and_competence
(
    test_template_id integer not null,
	competence_id integer not null,
	competence_need_id integer not null,

	foreign key (test_template_id) references hr.test_template (id) on delete cascade,
	foreign key (competence_id) references hr.competence (id) on delete cascade,
	foreign key (competence_need_id) references hr.competence_need (id) on delete cascade,
	primary key(test_template_id, competence_id)
);