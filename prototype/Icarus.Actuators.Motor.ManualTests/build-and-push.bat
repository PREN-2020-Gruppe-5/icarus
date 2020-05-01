cd ..
docker build -f Icarus.Actuators.Motor.ManualTests/Dockerfile -t derungsapp/icarus.actuators.motor.manualtests .
docker push derungsapp/icarus.actuators.motor.manualtests
cd icarus.actuators.motor.manualtests
pause
