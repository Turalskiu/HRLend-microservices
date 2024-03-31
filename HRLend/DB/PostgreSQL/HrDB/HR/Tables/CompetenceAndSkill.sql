create table hr.competence_and_skill
(
    skill_id integer not null,
	competence_id integer not null,
	skill_need_id integer not null,

	foreign key (skill_id) references hr.skill (id) on delete cascade,
	foreign key (competence_id) references hr.competence (id) on delete cascade,
	foreign key (skill_need_id) references hr.skill_need (id) on delete cascade,
	primary key(skill_id, competence_id)
);
