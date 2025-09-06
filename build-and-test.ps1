if ($args[0] -eq "build") {
    foreach ($sln in Get-ChildItem "*.sln" -Recurse) {
        dotnet build $sln
    }
}
elseif ($args[0] -eq "test") {
    foreach ($sln in Get-ChildItem "*.sln" -Recurse) {
        dotnet test $sln
    }
}
