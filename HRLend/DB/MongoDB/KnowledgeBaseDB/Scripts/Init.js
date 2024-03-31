// init.js
db = db.getSiblingDB('HRLend');
db.createCollection('profession');
db.profession.insertMany([
    {
        "_id": ObjectId("659152b679510b0a8fd27f13"),
    
        "title": "менеджер",
    
        "competencies":[
            {
                "title" : "Стрессоустойчивость",
    
                "competence_need":{
                    "required_code": 1,
                    "title": "hard"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен определять потребность клиента",
                        "test_module_id": "659d2b771585c698877dafdd"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен удовлетворить потербность клиента",
                        "test_module_id": "659d2bf81585c698877dafdf"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Осведомлен о стандартах обслуживания клиентов",
                        "test_module_id": "659d2c771585c698877dafe1"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен быстро наладить контакт с другими людьми",
                        "test_module_id": "659d2d011585c698877dafe3"
                    }
                ]
    
            },
            {
                "title" : "Коммуникабельность",
    
                "competence_need":{
                    "required_code": 2,
                    "title": "middle"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен активно слушать",
                        "test_module_id": "659d2dc01585c698877dafe5"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен ясно и четко выражать свои мысли",
                        "test_module_id": "659d2f561585c698877dafe7"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен к эмпатии",
                        "test_module_id": "659d2faf1585c698877dafe9"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен планировать свое  расписание",
                        "test_module_id": "659d30461585c698877dafeb"
                    }
                ]
    
            },
            {
                "title" : "Клиентоориентрированность",
    
                "competence_need":{
                    "required_code": 1,
                    "title": "hard"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен грамотно делегировать задачи и полномочия",
                        "test_module_id": "659d30d81585c698877dafed"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен правильно расставлять приоритеты",
                        "test_module_id": "659d31301585c698877dafef"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен структурировать цели и задачи",
                        "test_module_id": "659d32021585c698877daff1"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Осведомлен о техниках борьбы со стрессом",
                        "test_module_id": "659d32371585c698877daff3"
                    }
                ]
    
            },
            {
                "title" : "Управление временем",
    
                "competence_need":{
                    "required_code": 3,
                    "title": "soft"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен к самоконтролю",
                        "test_module_id": "659d32831585c698877daff5"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен анализировать стрессовые ситуации",
                        "test_module_id": "659d32f91585c698877daff7"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен одновременно выполнять несколько задач",
                        "test_module_id": "659d33721585c698877daff9"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способность работать в жестких дедлайнах",
                        "test_module_id": "659d33e21585c698877daffb"
                    }
                ]
            }
        ]
    },
    {
        "_id": ObjectId("659152b679510b0a8fd27f14"),
    
        "title": "программист",
    
        "competencies":[
            {
                "title" : "Стрессоустойчивость",
    
                "competence_need":{
                    "required_code": 1,
                    "title": "hard"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен определять потребность клиента",
                        "test_module_id": "659d2b771585c698877dafdd"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен удовлетворить потербность клиента",
                        "test_module_id": "659d2bf81585c698877dafdf"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Осведомлен о стандартах обслуживания клиентов",
                        "test_module_id": "659d2c771585c698877dafe1"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен быстро наладить контакт с другими людьми",
                        "test_module_id": "659d2d011585c698877dafe3"
                    }
                ]
    
            },
            {
                "title" : "Коммуникабельность",
    
                "competence_need":{
                    "required_code": 2,
                    "title": "middle"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен активно слушать",
                        "test_module_id": "659d2dc01585c698877dafe5"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен ясно и четко выражать свои мысли",
                        "test_module_id": "659d2f561585c698877dafe7"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен к эмпатии",
                        "test_module_id": "659d2faf1585c698877dafe9"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен планировать свое  расписание",
                        "test_module_id": "659d30461585c698877dafeb"
                    }
                ]
    
            },
            {
                "title" : "Управление временем",
    
                "competence_need":{
                    "required_code": 3,
                    "title": "soft"
                },
                
    
                "skills":[            
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен к самоконтролю",
                        "test_module_id": "659d32831585c698877daff5"
                    },
                    {
                        "skill_need":{
                            "required_code": 3,
                            "title": "soft"
                        },
    
                        "title" : "Способен анализировать стрессовые ситуации",
                        "test_module_id": "659d32f91585c698877daff7"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способен одновременно выполнять несколько задач",
                        "test_module_id": "659d33721585c698877daff9"
                    },
                    {
                        "skill_need":{
                            "required_code": 1,
                            "title": "hard"
                        },
    
                        "title" : "Способность работать в жестких дедлайнах",
                        "test_module_id": "659d33e21585c698877daffb"
                    }
                ]
            }
        ]
    }
])
