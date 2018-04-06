#!/bin/pwsh

#!/bin/pwsh

param(
    [string]$inputPath,
    [string]$outputPath
)

if ([string]::IsNullOrWhiteSpace($inputPath) -or [string]::IsNullOrWhiteSpace($outputPath)) {
    Write-Host "Usage: decrypt <input path> <output path>";
    Break;
}

$text = [System.IO.File]::ReadAllText($inputPath, [System.Text.Encoding]::UTF8);
$decryptedTextBytes = [System.Convert]::FromBase64String($text);
$decryptedText = [System.Text.Encoding]::UTF8.GetString($decryptedTextBytes);
[System.IO.File]::WriteAllText($outputPath, $decryptedText, [System.Text.Encoding]::UTF8);