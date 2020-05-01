cd ..
docker build -f Icarus.Sensors.ObjectDetection.ManualTests/Dockerfile -t derungsapp/icarus.sensors.objectdetection.manualtests .
docker push derungsapp/icarus.sensors.objectdetection.manualtests
cd Icarus.Sensors.ObjectDetection.ManualTests
pause
