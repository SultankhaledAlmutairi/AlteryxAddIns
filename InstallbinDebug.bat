@echo off
powershell "Start-Process -FilePath powershell.exe -ArgumentList '%~fs0\..\Installbin.ps1', 'Debug' -verb RunAs -Wait"