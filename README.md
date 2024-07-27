## Build the docker image

- obs: For ease of use, run the commands in the same directory as the yaml file.

To build the image, follow the command:

```C#
docker-compose -f docker-compose-local.yaml build
```

To build the image and create the container, follow the command:

```C#
docker-compose -f docker-compose-local.yaml -p english-project up -d
```

Using the Stopwatch class to measure the elapsed time:

```C#
   var stopwatch = new Stopwatch();
   stopwatch.Start();
   // the operation you want to check
   stopwatch.Stop();
   _logger.LogWarning(stopwatch.Elapsed.ToString());

```

## Template add new question

```C#
{
  "header": "The cat is hiding ___ the table.",
  "alternatives": [
    {
      "id": 1,
      "content": "under"
    },
    {
      "id": 2,
      "content": "on"
    },
    {
      "id": 3,
      "content": "in"
    },
    {
      "id": 4,
      "content": "by"
    },
    {
      "id": 5,
      "content": "at"
    }
  ],
  "difficulty": 1,
  "type": 1,
  "rightAnswer": 1,
  "explanation": "not added yet",
  "topic": "prepositions",
  "subtopics": [
    "all"
  ]
}
```

## Index MongoDb

be careful with lower and uppercase letters

```javascript
db.Questions.createIndex({ topic: 1, subtopics: 1, difficulty: 1 });
```

```javascript
db.UserAnswers.createIndex({ userId: 1 });
```

```javascript
db.Questions.explain("executionStats").find({
  topic: "condicionais",
  subtopics: "clauses",
  difficulty: 1,
});
```
