#!/bin/bash

cat /dev/null > init-docker/script.sql

cat 'Assistant\Assistant.sql' >> init-docker/script.sql
cat 'Assistant\Tables\Document.sql' >> init-docker/script.sql



for file in $(find './Assistant/Functions' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

for file in $(find './Assistant/Procedures' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

cat 'Assistant\Scripts\Init.sql' >> init-docker/script.sql