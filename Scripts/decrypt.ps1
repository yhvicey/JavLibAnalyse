#!/bin/pwsh

param(
    [string]$inputPath,
    [string]$outputPath
)

if ([string]::IsNullOrWhiteSpace($inputPath) -or [string]::IsNullOrWhiteSpace($outputPath)) {
    Write-Host "Usage: decrypt <input path> <output path>";
    Break;
}

$text = [System.IO.File]::ReadAllText($inputPath);
$decryptedTextBytes = [System.Convert]::FromBase64String($text);
$decryptedText = [System.Text.Encoding]::UTF8.GetString($decryptedTextBytes);
Add-Content -Path $outputPath -Value $decryptedText;