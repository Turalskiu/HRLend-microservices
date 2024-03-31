#!/bin/bash

cat /dev/null > db-docker-script.sql


cat 'AuthDB\init-docker\script.sql' >> db-docker-script.sql
cat 'HrDB\init-docker\script.sql' >> db-docker-script.sql
cat 'TestDB\init-docker\script.sql' >> db-docker-script.sql
