rmdir TestResults /S /Q
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool

cd TestResults
for /D %%x in (*) do if not defined f set "f=%%x"
cd ..
reportgenerator "-reports:.\TestResults\%f%\coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
cd coveragereport
index.html
pause
