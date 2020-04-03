
cd src
rm -f -r bin/

OutPath="bin/Release/NuGet/lib"

for a in "linux-x64" "osx-x64" "win-x64"
do
	if [ "$1" = "true" ]
	then
		dotnet publish -r $a -f "netcoreapp3.1" --self-contained $1
		cd bin/Release/netcoreapp3.1/$a
		mkdir "SharpChecker-v1.0-$a"
		mv publish/* "SharpChecker-v1.0-$a"
		zip ../../../"SharpChecker-v1.0-standalone-$a.zip" ./"SharpChecker-v1.0-$a"/*
		cd ../../../..
	else
		for b in "netcoreapp3.1" "netcoreapp3.0" "netcoreapp2.1"
		do
			dotnet publish -r $a -f $b --self-contained $1
			mkdir -p $OutPath/$b/$a
			echo "Moving to $OutPath/$b/$a"
			mv bin/Release/$b/$a/publish/* $OutPath/$b/$a/
		done
	fi
done

echo "Done publishing."
if [ "$1" = "true" ]
then
	echo "Done publishing self contained"
else
	dotnet pack
fi

cd ..
