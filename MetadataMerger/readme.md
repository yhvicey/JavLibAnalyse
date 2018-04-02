# Metadata Merger

Merge all metadata.json into a tsv file.

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
dotnet MetadataMerger.dll "path/to/data/dir" "path/to/output/file"
```