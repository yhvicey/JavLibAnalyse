#!/bin/pwsh

param(
    [string]$inputPath,
    [string]$outputPath
)

if ([string]::IsNullOrWhiteSpace($inputPath) -or [string]::IsNullOrWhiteSpace($outputPath)) {
    Write-Host "Usage: encrypt <input path> <output path>";
    Break;
}

$text = [System.IO.File]::ReadAllText($inputPath, [System.Text.Encoding]::UTF8);
$textBytes = [System.Text.Encoding]::UTF8.GetBytes($text);
$encryptedText = [System.Convert]::ToBase64String($textBytes);
[System.IO.File]::WriteAllText($outputPath, $encryptedText, [System.Text.Encoding]::UTF8);