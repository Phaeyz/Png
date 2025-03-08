setlocal
set libName=Phaeyz.Png
set repoUrl=https://github.com/Phaeyz/Png
dotnet run ..\%libName%\bin\Debug\net9.0\%libName%.dll ..\docs --source %repoUrl%/blob/main/%libName% --namespace %libName% --clean
endlocal