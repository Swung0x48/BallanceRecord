FILE=updateDB
if test -f "$FILE"; then
    echo "$FILE exists."
    dotnet tool install --global dotnet-ef
    dotnet ef database update
    rm updateDB
fi

dotnet BallanceRecordApi.dll