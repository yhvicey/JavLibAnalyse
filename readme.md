# Project for analysing data on javlib.com

## Project structure

```txt
JavLibAnalyse
│
├─Analyse (Files for analysing data) (TODO)
│
├─Crawler (Crawler for crawling data on javlib)
│
├─Data (Crawled data, simply encrypted)
|
├─MetadataMerger (Merger for merging metadata to tsv file)
│
└─Scripts (Useful scripts)
```

## Analyse

See [Analyse](Analyse/readme.md)

## Crawler

See [Crawler](Crawler/readme.md)

## Metadata Merger

See [Metadata Merger](MetadataMerger/readme.md)

## Data Encrypting & Decrypting

### Encrypt

Encrypted into base 64 string. You can use encrypt script under "Script" folder to encrypt data.

```shell
./Script/encrypt "path/to/raw/data/file" "path/to/output/file"
```

### Decrypt

Decrypted from base 64 string. You can use decrypt script under "Script" folder to decrypt data.

```shell
./Script/decrypt "path/to/encrypted/data/file" "path/to/output/file"
```