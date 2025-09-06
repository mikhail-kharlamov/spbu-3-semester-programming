if [[ "$1" == "build" ]] ; then
    for sln in $(find -name *.sln); do
        dotnet build $sln
    done
elif [[ "$1" == "test" ]] ; then
    for sln in $(find -name *.sln); do
        dotnet test $sln
    done
fi
