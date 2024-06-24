// init.js
db = db.getSiblingDB('HRLend');
db.createCollection('test_module');
db.test_module.insertMany([
  {
    "_id": ObjectId("659d2b771585c698877dafdd"),
    "title": "тест модуль для навыка стессоустойчивость",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "ea03ec8d-4c94-43b3-1943-41d6b041d758",
        "text": "Какие методы анализа данных вы используете для определения потребностей клиентов?",
        "description": "c",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "7a03ec8d-4c94-43b3-1943-41d6b041d758",
            "is_true": true,
            "text": "Анализ истории покупок"
          },
          {
            "id": "ea73ec8d-4c94-43b3-1943-41d6b041d758",
            "is_true": false,
            "text": "Анализ предпочтений клиентов"
          }
        ]
      },
      {
        "id": "ea03ec8d-4c94-43b3-1943-41d6b041d756",
        "text": "Какие инструменты вы используете для прогнозирования будущих потребностей клиентов?",
        "description": "c",
        "type": "checkbox",
        "max_value": 1,
        "options": [
          {
            "id": "ea03ec8d-4c04-43b3-1943-41d6b041d758",
            "is_true": false,
            "text": "Машинное обучение"
          },
          {
            "id": "ea03ec8d-4c94-43b3-1943-51d6b041d758",
            "is_true": true,
            "text": "Аналитические платформы"
          },
          {
            "id": "ea03ec8d-4994-43b3-1943-41d6b041d758",
            "is_true": true,
            "text": "Инструменты бизнес-аналитики"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d2bf81585c698877dafdf"),
    "title": "Способен определять потребность клиента",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "8a03ec8d-4c94-43b3-1943-41d6b041d759",
        "text": "Какие стратегии вы используете для обеспечения удовлетворенности клиентов вашими продуктами или услугами?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d760",
            "is_true": true,
            "text": "Постоянное улучшение качества продуктов"
          },
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d761",
            "is_true": false,
            "text": "Быстрая и эффективная служба поддержки клиентов"
          }
        ]
      },
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d762",
        "text": "Какие инструменты вы используете для измерения удовлетворенности клиентов?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d763",
            "is_true": true,
            "text": "Опросы клиентов"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d764",
            "is_true": false,
            "text": "Мониторинг социальных сетей"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d2c771585c698877dafe1"),
    "title": "Осведомлен о стандартах обслуживания клиентов",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "8a03ec8d-4c94-43b3-1943-41d6b041d759",
        "text": "Какие стандарты обслуживания клиентов применяются в вашей компании для обеспечения высокого уровня удовлетворенности?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d760",
            "is_true": true,
            "text": "Стандарты времени ответа на запросы"
          },
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d761",
            "is_true": false,
            "text": "Стандарты качества обслуживания"
          }
        ]
      },
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d762",
        "text": "Какие инструменты и методы вы используете для мониторинга соблюдения стандартов обслуживания клиентов?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d763",
            "is_true": true,
            "text": "Регулярные аудиты и проверки"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d764",
            "is_true": false,
            "text": "Сбор и анализ обратной связи от клиентов"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d2d011585c698877dafe3"),
    "title": "Способен быстро наладить контакт с другими людьми",
    "options": {
      "is_default": true,
      "count_questions": 1,
      "take_questions": 1,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d762",
        "text": "Какие техники вы используете для быстрого установления контакта с новыми людьми?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d763",
            "is_true": true,
            "text": "Активное слушание"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d764",
            "is_true": false,
            "text": "Использование общих интересов"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d2dc01585c698877dafe5"),
    "title": "Способен активно слушать",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "8a03ec8d-4c94-43b3-1943-41d6b041d765",
        "text": "Какие техники активного слушания вы применяете в разговоре?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d766",
            "is_true": true,
            "text": "Задавание уточняющих вопросов"
          },
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d767",
            "is_true": false,
            "text": "Парафраз (пересказ сказанного)"
          }
        ]
      },
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d768",
        "text": "Как вы демонстрируете собеседнику, что вы его внимательно слушаете?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d769",
            "is_true": true,
            "text": "Установление зрительного контакта"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d770",
            "is_true": false,
            "text": "Подтверждающие реплики (например, 'да', 'понятно')"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d2f561585c698877dafe7"),
    "title": "тест 1",
    "options": {
      "is_default": true,
      "count_questions": 1,
      "take_questions": 1,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какие методы вы используете, чтобы ясно и четко выражать свои мысли в разговоре?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Использование простых и понятных слов"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Структурирование своих мыслей перед началом разговора"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d2faf1585c698877dafe9"),
    "title": "тест 2/3/4/5/6/7/8",
    "options": {
      "is_default": true,
      "count_questions": 1,
      "take_questions": 1,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какие методы вы используете, чтобы лучше понять чувства и эмоции других людей?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Активное слушание и задавание вопросов"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Наблюдение за невербальными сигналами"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы демонстрируете свою эмпатию к другим людям?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "3",
            "is_true": true,
            "text": "Слушаю внимательно, не перебивая"
          },
          {
            "id": "4",
            "is_true": false,
            "text": "Использую выражения, показывающие сочувствие (например, 'Я понимаю, как вам трудно')"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d30461585c698877dafeb"),
    "title": "тест о космосе",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Каким образом вы планируете свое ежедневное расписание?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Использую ежедневные списки дел"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Планирую задачи в календаре"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Ставлю приоритеты для выполнения задач"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы обеспечиваете баланс между работой и личной жизнью при планировании своего расписания?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Ставлю конкретные временные рамки для работы и отдыха"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Регулярно делаю перерывы и планирую время для семьи и хобби"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Использую методы эффективного временного управления"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d30d81585c698877dafed"),
    "title": "тест по фильму Терминатор 2",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какие критерии вы учитываете при делегировании задач другим сотрудникам?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Уровень компетенции и опыта сотрудника в данной области"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Личные предпочтения и отношения с сотрудником"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Сложность задачи и сроки выполнения"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы обеспечиваете эффективное выполнение делегированных задач?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Устанавливаю четкие цели и ожидания перед делегированием"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Регулярно проверяю прогресс выполнения задач"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Все вышеперечисленное"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d31301585c698877dafef"),
    "title": "тест по фильму 'Трансформеры'",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Каким образом вы определяете важность задач перед их выполнением?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Оцениваю влияние задачи на достижение целей и срочность выполнения"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Исходя из личных предпочтений и интересов"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Следую за указаниями руководителя или приоритизации из отчетности"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы обеспечиваете соблюдение установленных приоритетов в ходе выполнения задач?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Регулярно переоцениваю приоритеты и делаю необходимые корректировки"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Получаю обратную связь от коллег для проверки соответствия"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Все вышеперечисленное"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d32021585c698877daff1"),
    "title": "тест по фильму о баскетболе",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Каким образом вы структурируете свои цели перед их достижением?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Разбиваю цели на более мелкие задачи с конкретными сроками выполнения"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Зависит от текущей ситуации и настроения"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Все вышеперечисленное"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы планируете выполнение структурированных задач?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Устанавливаю приоритеты и сроки для каждой мелкой задачи"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Просто начинаю выполнять задачи без предварительного планирования"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Доверяю интуиции и естественному ходу вещей"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d32371585c698877daff3"),
    "title": "тест по фильму о футболе",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какими техниками вы пользуетесь для снижения стресса на работе?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Практикую дыхательные упражнения и медитацию в течение дня"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Отдыхаю, общаясь с коллегами в свободное время"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Использую развлекательные приложения на смартфоне"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы оцениваете эффективность выбранных вами техник борьбы со стрессом?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Заметно улучшаю свою концентрацию и общее состояние техник"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Ощущаю временное облегчение но не всегда достигаю результатов"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Не замечаю значимых изменений после использования техник"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d32831585c698877daff5"),
    "title": "тест по фильму о волейболе",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Каким образом вы управляете своими эмоциями в стрессовых ситуациях?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Использую методы дыхательной практики для снятия напряжения"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Позволяю эмоциям влиять на принятие решений"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Ищу поддержку и советы у коллег или друзей"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы контролируете свои реакции на критику или неожиданные изменения в работе?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Анализирую информацию, прежде чем отвечать"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Реагирую эмоционально и часто показываю свои эмоции"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Все вышеперечисленное"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d32f91585c698877daff7"),
    "title": "тест по истории СССР",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Каким образом вы анализируете стрессовые ситуации на работе?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Оцениваю факторы, вызывающие стресс"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Просто переношу стресс без анализа его причин"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Ищу поддержку у коллег, не задумываясь о причинах стресса"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы определяете, какие ситуации могут стать потенциально стрессовыми для вас?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Анализирую прошлые опыты"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Реагирую на стрессовые ситуации в момент"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Не обращаю внимания на потенциально стрессовые ситуации"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d33721585c698877daff9"),
    "title": "тест по истории Америки",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Как вы организуете свою работу, когда у вас есть несколько приоритетных задач?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Распределяю время и приоритеты, делаю план действий для каждой задачи"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Решаю задачи в порядке их поступления без учета приоритетов"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Сосредотачиваюсь на одной задаче"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы управляете своим временем, когда на работе возникает несколько срочных задач одновременно?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Устанавливаю приоритеты для каждой задачи"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Решаю задачи в хаотичном порядке, в зависимости от текущего настроения"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Делегирую большую часть задач коллегам"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d33e21585c698877daffb"),
    "title": "тест по финансам",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Как вы обычно готовитесь к выполнению задач в жестких дедлайнах?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Ценная бумага, удостоверяющая долю в собственности компании"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Долговое обязательство"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Денежный депозит"
          }
        ]
      },
      {
        "id": "2",
        "text": "Что означает понятие 'ROI' в финансах?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Возврат инвестиций"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Общий доход инвестиций"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Доход от инвестиций"
          }
        ]
      }
    ]
  },
  {
    "_id": ObjectId("659d368d1585c698877daffd"),
    "title": "тест на стрессоустойчивость",
    "options": {
      "is_default": true,
      "count_questions": 2,
      "take_questions": 2,
      "limit_duration_in_seconds": 120
    },
    "rule": {
      "min_value_for_passed": 1
    },
    "recommendations": [
      {
        "title": "metanit.com",
        "description": "Изучение python",
        "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Как вы обычно реагируете на стрессовые ситуации?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Пытаюсь сохранять спокойствие и решать проблемы по мере их поступления."
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Чувствую панику и теряю контроль над ситуацией."
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Избегаю стрессовых ситуаций, стараясь им не подвергаться."
          }
        ]
      },
      {
        "id": "2",
        "text": "Как вы обычно реагируете на физические признаки стресса, такие как учащенное сердцебиение или потливые ладони?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Применяю техники релаксации, чтобы снизить физическую реакцию на стресс."
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Поддаюсь эмоциям и мне трудно контролировать свои физические реакции."
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Не обращаю на это внимания, стараясь игнорировать физические признаки стресса."
          }
        ]
      }
    ]
  }
]);
