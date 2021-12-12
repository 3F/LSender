@echo off

set cim=packages\vsSolutionBuildEvent\cim.cmd

set reltype=%1
if not defined reltype set reltype=DCI

call tools/gnt /p:wpath="%cd%" /p:ngconfig="tools/packages.config" /nologo /v:m /m:7 || goto err
call %cim% /v:m /m:7 /p:Configuration="%reltype%_SDK15" || goto err

exit /B 0

:err
echo. Build failed. 1>&2
exit /B 1