cd %~dp0/proto

for %%i in (*.proto) do (
	echo %%i
	protoc.exe -o %%~ni.pb %%i
	echo %%~ni.pb
)
echo end
pause
