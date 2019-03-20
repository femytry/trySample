FROM microsoft/dotnet
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out
CMD ASPNETCORE_URLS=http://*:$PORT dotnet out/trySample.dll