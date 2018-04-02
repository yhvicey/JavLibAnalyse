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

|Property|Type|Default|Description|
-|-|-|-
checkpointInterval|int|120|Checkpoint interval in second
downloadImage|bool|false|Whether the crawler should download image (TODO)
genres|string|(See source file)|[Required] Genres to crawl (Samples here, look complete list in source code)
idleTime|int|600|Idle time before quit in second
infoTimerInterval|int|30|Info timer execute interval in second
logDir|string|log|Log directory for saving log
logLevel|string|info|Log level. Available values: debug, info, error, fatal
maxRequestInterval|int|30|Maximum request interval in second
maxThreadCount|int|128|Max thread count for this application
minRequestInterval|int|0|Minimal request interval in second
outputDir|string|data|Output directory for saving result file
processorCount|int|10|Processor count, processor used for handling details page
producerCount|int|10|Producer count, producer used for handling genres list page
rootUrl|string|(See source file)|Root url for javlib
tempDir|string|temp|Temporary directory for saving checkpoint file