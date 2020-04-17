cd ..
docker build -f Icarus.Sensors.Tilt/Dockerfile -t derungsapp/icarus.sensors.tilt .
docker push derungsapp/icarus.sensors.tilt
pause
