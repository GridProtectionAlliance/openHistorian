$tempFile = "$($args[0]).tmp"
Copy-Item $tempFile $args[0] -Force
Remove-Item $tempFile -Force