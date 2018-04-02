# Crawler

Crawl data on javlib, and save it to data directory.

## Prerequisite

[.NET Core](https://www.microsoft.com/net)

## Build

```shell
dotnet build
```

## Publish

```shell
dotnet publish -o "path/to/output/dir"
```

## Run

```shell
cd "path/to/output/dir"
dotnet Crawler.dll
```

## Config

Control config by modifying config.json under publish dir.

```json
{
  "checkpointInterval": 60,             // Checkpoint interval in second
  "downloadImage": false,               // Whether the crawler should download image (TODO)
  "genres": "a;b;c",                    // [Required] Genres to crawl (Samples here, look complete list in source code)
  "idleTime": 600,                      // Idle time before quit in second
  "infoTimerInterval": 60,              // Info timer execute interval in second
  "logDir": "log",                      // Log directory for saving log
  "logLevel": "info",                   // Log level. Available values: debug, info, error, fatal
  "maxRequestInterval": 30,             // Maximum request interval in second
  "maxThreadCount": 128,                // Max thread count for this application
  "minRequestInterval": 0,              // Minimal request interval in second
  "outputDir": "data",                  // Output directory for saving result file
  "processorCount": 10,                 // Processor count, processor used for handling details page
  "producerCount": 10,                  // Producer count, producer used for handling genres list page
  "rootUrl": "http://www.19lib.com/cn", // Root url for javlib
  "tempDir": "temp"                     // Temporary directory for saving checkpoint file
}
```
