cd ..
docker build -f Icarus.Sensors.Tilt.ManualTests/Dockerfile -t derungsapp/icarus.sensors.tilt.manualtests .
docker push derungsapp/icarus.sensors.tilt.manualtests
cd Icarus.Sensors.Tilt.ManualTests
pause
