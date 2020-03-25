FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build_stage

RUN mkdir -p /app/publish
WORKDIR /app
COPY ./ ./
WORKDIR /app/Vidload.Worker.MetadataCollector/
RUN dotnet publish --self-contained --runtime linux-x64 --framework netcoreapp3.1 -o /app/publish


FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS final_stage

RUN apt-get update -y && apt-get upgrade -y && apt-get dist-upgrade -y
RUN apt-get install python3 python3-pip -y
RUN apt-get autoremove
RUN pip3 install youtube-dl

RUN mkdir /app
WORKDIR /app
COPY --from=build_stage /app/publish .
CMD ["./Vidload.Worker.MetadataCollector"]
