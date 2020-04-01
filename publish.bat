
@echo off

cd src
rmdir /S /Q bin
set OutPath="bin/Release/NuGet/lib"

for %%a in ("linux-x64", "osx-x64", "win-x64") do (
	if "%1" equ "true" (
		dotnet publish -r %%a -f "netcoreapp3.1" --self-contained %1
		cd bin/Release/netcoreapp3.1/%%a
		rename publish "SharpChecker-v1.0-%%a"
		powershell "Compress-Archive SharpChecker-v1.0-%%a -DestinationPath SharpChecker-v1.0-standalone-%%a.zip"
		cd ../../../..
	) else (
		for %%b in ("netcoreapp3.1", "netcoreapp3.0", "netcoreapp2.1") do (
			dotnet publish -r %%a -f %%b --self-contained %1
			mkdir %OutPath%/%%b/%%a
			echo "Moving to %OutPath%/%%b/%%a"
			move bin/Release/%%b/%%a/publish/* %OutPath%/%%b/%%a/
		)
	)
)

if "%1" equ "true" ( echo "Done publishing." ) else ( dotnet pack )
