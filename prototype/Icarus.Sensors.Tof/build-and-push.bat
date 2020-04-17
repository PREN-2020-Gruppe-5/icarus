cd ..
docker build -f Icarus.Sensors.Tof/Dockerfile -t derungsapp/icarus.sensors.tof .
docker push derungsapp/icarus.sensors.tof
pause
