#!/bin/bash

cat /dev/null > script.sql

cat 'Auth\Auth.sql' >> init-docker/script.sql
cat 'Auth\Tables\CabinetStatus.sql' >> init-docker/script.sql
cat 'Auth\Tables\GroupType.sql' >> init-docker/script.sql
cat 'Auth\Tables\Cabinet.sql' >> init-docker/script.sql
cat 'Auth\Tables\Group.sql' >> init-docker/script.sql
cat 'Auth\Tables\Role.sql' >> init-docker/script.sql
cat 'Auth\Tables\UserStatus.sql' >> init-docker/script.sql
cat 'Auth\Tables\User.sql' >> init-docker/script.sql
cat 'Auth\Tables\UserInfo.sql' >> init-docker/script.sql
cat 'Auth\Tables\RefreshToken.sql' >> init-docker/script.sql
cat 'Auth\Tables\RegistrationToken.sql' >> init-docker/script.sql
cat 'Auth\Tables\UserAndRole.sql' >> init-docker/script.sql
cat 'Auth\Tables\GroupAndUser.sql' >> init-docker/script.sql


for file in $(find './Auth/Functions' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

for file in $(find './Auth/Procedures' -type f -name "*.sql" | sort -n); do
  cat $file >> init-docker/script.sql
done

cat 'Auth\Scripts\Init.sql' >> init-docker/script.sql