FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY . .
WORKDIR /app
RUN dotnet publish Icarus.Sensors.ObjectDetection.ManualTests -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-bionic-arm64v8 AS runtime
WORKDIR /app

## Install some dependencies
RUN apt-get update && apt-get install -y \
      libtbb-dev \
      qt5-default \
      libgstreamer1.0-0 \
      gstreamer1.0-plugins-base \
      gstreamer1.0-plugins-good \
      gstreamer1.0-plugins-bad \
      gstreamer1.0-plugins-ugly \
      gstreamer1.0-libav \
      gstreamer1.0-doc \
      gstreamer1.0-tools \
      gstreamer1.0-x \
      gstreamer1.0-alsa \
      gstreamer1.0-gl \
      gstreamer1.0-gtk3 \
      gstreamer1.0-qt5 \
      gstreamer1.0-pulseaudio \
      libv4l-dev \
      libcurl4 \ 
      curl \
      git
	  
ENV LD_LIBRARY_PATH "$LD_LIBRARY_PATH:/usr/local/lib"

COPY --from=build /app/out ./
COPY Icarus.Sensors.ObjectDetection/darknet darknet
COPY Icarus.Sensors.ObjectDetection.ManualTests/traffic_cone_1.png darknet/traffic_cone_1.png
COPY Icarus.Sensors.ObjectDetection.ManualTests/traffic_cone_2.png darknet/traffic_cone_2.png
COPY Icarus.Sensors.ObjectDetection.ManualTests/traffic_cone_3.png darknet/traffic_cone_3.png
COPY Icarus.Sensors.ObjectDetection.ManualTests/traffic_cone_4.png darknet/traffic_cone_4.png
ENTRYPOINT ["dotnet", "Icarus.Sensors.ObjectDetection.ManualTests.dll"]