$tempFile = "$($args[0]).tmp"
Copy-Item $args[0] "$($args[0]).tmp" -Force
Add-Type -Assembly System.Drawing

$bitmap = New-Object "System.Drawing.Bitmap" $tempFile
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$font = New-Object "System.Drawing.Font" "Tahoma", 8
$text = "ver. $($args[1])"
$textSize = $graphics.MeasureString($text, $font)
$textWidth = [System.Int32]([System.Math]::Ceiling($textSize.Width))
$textHeight = [System.Int32]([System.Math]::Ceiling($textSize.Height))
$rect = New-Object "System.Drawing.RectangleF" (164 - $textWidth), (312 - $textHeight), $textWidth, $textHeight

$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$graphics.DrawString($text, $font, [System.Drawing.Brushes]::Black, $rect)
$graphics.Flush()
$bitmap.Save($args[0])