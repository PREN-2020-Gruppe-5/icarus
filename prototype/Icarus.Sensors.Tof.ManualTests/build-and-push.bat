cd ..
docker build -f Icarus.Sensors.Tof.ManualTests/Dockerfile -t derungsapp/icarus.sensors.tof.manualtests .
docker push derungsapp/icarus.sensors.tof.manualtests
cd Icarus.Sensors.Tof.ManualTests
pause
