cd ..
docker build -f Icarus.App/Dockerfile -t derungsapp/icarus.app .
docker push derungsapp/icarus.app
cd Icarus.App
pause
