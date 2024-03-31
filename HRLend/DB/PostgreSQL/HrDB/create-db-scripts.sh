#!/bin/bash

cat /dev/null > init-docker/script.sql

cat 'HR\HR.sql' >> init-docker/script.sql
cat 'HR\Tables\Cabinet.sql' >> init-docker/script.sql
cat 'HR\Tables\SkillNeed.sql' >> init-docker/script.sql
cat 'HR\Tables\CompetenceNeed.sql' >> init-docker/script.sql
cat 'HR\Tables\TestTemplate.sql' >> init-docker/script.sql
cat 'HR\Tables\Competence.sql' >> init-docker/script.sql
cat 'HR\Tables\Skill.sql' >> init-docker/script.sql
cat 'HR\Tables\TestTemplateAndCompetence.sql' >> init-docker/script.sql
cat 'HR\Tables\CompetenceAndSkill.sql' >> init-docker/script.sql



for file in $(find './HR/Functions' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

for file in $(find './HR/Procedures' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

cat 'HR\Scripts\Init.sql' >> init-docker/script.sql