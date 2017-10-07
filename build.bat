@echo off
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe FruityUI.sln /p:Configuration=Debug /p:Platform="Any CPU"

echo "Done building FruityUI";
pause