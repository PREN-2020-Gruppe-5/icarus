cd ..
docker build -f Icarus.Actuators.Motor/Dockerfile -t derungsapp/icarus.actuators.motor .
docker push derungsapp/icarus.actuators.motor
pause
