
mkdir tmp

cd tmp
mkdir NoStopMod

cd NoStopMod
mkdir runtimes
cd ..

copy ..\bin\Debug\NoStopMod.dll NoStopMod
copy ..\Info.json NoStopMod

xcopy ..\..\lib\common\original\SharpHook\3.0.0\runtimes\* NoStopMod\runtimes /e /h /k /y

tar -a -c -f NoStopMod-%1.zip NoStopMod
del /f /q ../NoStopMod-%1.zip
move NoStopMod-%1.zip ..
cd ..
rmdir /s /q tmp
cd ..
