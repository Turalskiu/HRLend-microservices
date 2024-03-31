#!/bin/bash

cat /dev/null > init-docker/script.sql


cat 'Test\Test.sql' >> init-docker/script.sql
cat 'Test\Tables\User.sql' >> init-docker/script.sql
cat 'Test\Tables\Group.sql' >> init-docker/script.sql
cat 'Test\Tables\TestLinkStatus.sql' >> init-docker/script.sql
cat 'Test\Tables\TestLinkType.sql' >> init-docker/script.sql
cat 'Test\Tables\TestLinkResponseStatus.sql' >> init-docker/script.sql
cat 'Test\Tables\Test.sql' >> init-docker/script.sql
cat 'Test\Tables\TestLink.sql' >> init-docker/script.sql
cat 'Test\Tables\TestLinkResponse.sql' >> init-docker/script.sql
cat 'Test\Tables\AnonumousUser.sql' >> init-docker/script.sql
cat 'Test\Tables\TestResult.sql' >> init-docker/script.sql



for file in $(find './Test/Functions' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

for file in $(find './Test/Procedures' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

cat 'Test\Scripts\Init.sql' >> init-docker/script.sql