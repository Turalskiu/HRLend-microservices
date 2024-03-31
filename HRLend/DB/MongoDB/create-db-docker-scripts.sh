#!/bin/bash

cat /dev/null > db-docker-script.js


cat 'TestDB\Scripts\Init.js' >> db-docker-script.js
cat 'TestGeneratorDB\Scripts\Init.js' >> db-docker-script.js
cat 'StatisticDB\Scripts\Init.js' >> db-docker-script.js
cat 'KnowledgeBaseDB\Scripts\Init.js' >> db-docker-script.js
