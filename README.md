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
