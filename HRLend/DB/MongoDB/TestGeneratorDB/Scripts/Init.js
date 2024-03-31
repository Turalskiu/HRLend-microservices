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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "ea03ec8d-4c94-43b3-1943-41d6b041d758",
        "text": "Как дела?",
        "description": "c",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "7a03ec8d-4c94-43b3-1943-41d6b041d758",
            "is_true": true,
            "text": "Отлично!"
          },
          {
            "id": "ea73ec8d-4c94-43b3-1943-41d6b041d758",
            "is_true": false,
            "text": "Плохо!"
          }
        ]
      },
      {
        "id": "ea03ec8d-4c94-43b3-1943-41d6b041d756",
        "text": "Какие типы данных имеются в языке программирования c#?",
        "description": "c",
        "type": "checkbox",
        "max_value": 1,
        "options": [
          {
            "id": "ea03ec8d-4c04-43b3-1943-41d6b041d758",
            "is_true": false,
            "text": "Хорошие"
          },
          {
            "id": "ea03ec8d-4c94-43b3-1943-51d6b041d758",
            "is_true": true,
            "text": "int"
          },
          {
            "id": "ea03ec8d-4994-43b3-1943-41d6b041d758",
            "is_true": true,
            "text": "string"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "8a03ec8d-4c94-43b3-1943-41d6b041d759",
        "text": "Что такое HTML?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d760",
            "is_true": true,
            "text": "Язык разметки"
          },
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d761",
            "is_true": false,
            "text": "Язык программирования"
          }
        ]
      },
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d762",
        "text": "Что такое CSS?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d763",
            "is_true": true,
            "text": "Язык стилей"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d764",
            "is_true": false,
            "text": "Язык программирования"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "8a03ec8d-4c94-43b3-1943-41d6b041d759",
        "text": "Что такое Python?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d760",
            "is_true": true,
            "text": "Язык программирования"
          },
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d761",
            "is_true": false,
            "text": "Язык разметки"
          }
        ]
      },
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d762",
        "text": "Что такое JavaScript?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d763",
            "is_true": true,
            "text": "Язык программирования"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d764",
            "is_true": false,
            "text": "Язык стилей"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d762",
        "text": "Что такое Java?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d763",
            "is_true": true,
            "text": "Язык программирования"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d764",
            "is_true": false,
            "text": "Язык стилей"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "8a03ec8d-4c94-43b3-1943-41d6b041d765",
        "text": "Что такое JSON?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d766",
            "is_true": true,
            "text": "Формат передачи данных"
          },
          {
            "id": "8a03ec8d-4c94-43b3-1943-41d6b041d767",
            "is_true": false,
            "text": "Язык программирования"
          }
        ]
      },
      {
        "id": "9a03ec8d-4c94-43b3-1943-41d6b041d768",
        "text": "Что такое API?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d769",
            "is_true": true,
            "text": "Интерфейс программирования приложений"
          },
          {
            "id": "9a03ec8d-4c94-43b3-1943-41d6b041d770",
            "is_true": false,
            "text": "Язык разметки"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какой самый популярный язык программирования?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "JavaScript"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Python"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Вопрос 1",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Верный ответ"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Неверный ответ"
          }
        ]
      },
      {
        "id": "2",
        "text": "Вопрос 2",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "3",
            "is_true": true,
            "text": "Верный ответ"
          },
          {
            "id": "4",
            "is_true": false,
            "text": "Неверный ответ"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какая самая большая планета в Солнечной системе?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Юпитер"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Марс"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Земля"
          }
        ]
      },
      {
        "id": "2",
        "text": "Что такое Черная дыра?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Область пространства с экстремально сильным гравитационным полем"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Сверхновая звезда"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Туманность"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Кто исполнил главную роль в фильме 'Терминатор 2: Судный день'?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Арнольд Шварценеггер"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Сильвестр Сталлоне"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Брюс Уиллис"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как называется робот, который играет важную роль в фильме 'Терминатор 2: Судный день'?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Т-800"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "R2-D2"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "C-3PO"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Кто режиссировал фильм 'Трансформеры'?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Майкл Бэй"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Кристофер Нолан"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Питер Джексон"
          }
        ]
      },
      {
        "id": "2",
        "text": "Как называется главный герой-робот в фильме 'Трансформеры'?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Оптимус Прайм"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Бамблби"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Мегатрон"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какая команда выиграла NBA в сезоне 2020-2021?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Милуоки Бакс"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Лос-Анджелес Лейкерс"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Голден Стэйт Уорриорз"
          }
        ]
      },
      {
        "id": "2",
        "text": "Кто является лучшим снайпером в истории NBA?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Рэй Аллен"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Леброн Джеймс"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Майкл Джордан"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какая команда выиграла Чемпионат мира по футболу в 2018 году?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Сборная Франции"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Сборная Аргентины"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Сборная Бразилии"
          }
        ]
      },
      {
        "id": "2",
        "text": "Какой игрок сборной Бразилии известен как 'Феномен'?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Роналдо"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Роналдиньо"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Пеле"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какая страна выиграла золотую медаль на Олимпийских играх по волейболу у мужчин в 2016 году?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Бразилия"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Россия"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "США"
          }
        ]
      },
      {
        "id": "2",
        "text": "Какое количество игроков находится на поле у каждой команды в волейболе?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "6"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "7"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "5"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "В каком году была образована СССР?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "1922"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "1917"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "1945"
          }
        ]
      },
      {
        "id": "2",
        "text": "Кто был первым президентом СССР?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Михаил Горбачёв"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Иосиф Сталин"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Владимир Ленин"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Какое событие в истории США произошло 4 июля 1776 года?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "1",
            "is_true": true,
            "text": "Декларация независимости"
          },
          {
            "id": "2",
            "is_true": false,
            "text": "Война за независимость"
          },
          {
            "id": "3",
            "is_true": false,
            "text": "Открытие первого парка развлечений"
          }
        ]
      },
      {
        "id": "2",
        "text": "Кто был первым президентом США?",
        "description": "",
        "type": "radiobutton",
        "max_value": 1,
        "options": [
          {
            "id": "4",
            "is_true": true,
            "text": "Джордж Вашингтон"
          },
          {
            "id": "5",
            "is_true": false,
            "text": "Томас Джефферсон"
          },
          {
            "id": "6",
            "is_true": false,
            "text": "Авраам Линкольн"
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
    "recommendations":[
      {
          "title" : "metanit.com",
          "description": "Изучение python",
          "link": "https://metanit.com/python/fastapi/1.8.php"
      }
    ],
    "questions": [
      {
        "id": "1",
        "text": "Что такое акция?",
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
    "recommendations":[
      {
          "title" : "metanit.com",
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
