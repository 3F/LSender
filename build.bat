@echo off

set cim=packages\vsSolutionBuildEvent\cim.cmd
set _gnt=tools/gnt

set reltype=%~1
if "%reltype%"=="" (
    set "reltype=DCI"
)

:: Activate vsSBE
call %_gnt% /p:wpath="%cd%" /p:ngconfig=".gnt/packages.config" /nologo /v:m /m:4 || goto err

:: Build
call %cim% "LSender.sln" /v:m /m:4 /p:Configuration="%reltype%_SDK15" || goto err

goto ok

:err

echo. Build failed. 1>&2
exit /B 1

:ok
exit /B 0
