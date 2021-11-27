
mkdir tmp
cd tmp
mkdir NoStopMod

copy ..\bin\Debug\NoStopMod.dll NoStopMod
copy ..\Info.json NoStopMod

tar -a -c -f NoStopMod-%1.zip NoStopMod
del /f /q ../NoStopMod-%1.zip
move NoStopMod-%1.zip ..
cd ..
rmdir /s /q tmp
cd ..
