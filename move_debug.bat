
mkdir tmp
cd tmp
mkdir NoStopMod

copy ..\bin\Debug\NoStopMod.dll NoStopMod

xcopy .\NoStopMod\*.* %1\NoStopMod\* /y

cd ..
