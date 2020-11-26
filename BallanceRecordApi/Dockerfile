FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY BallanceRecordApi.csproj /build/

RUN dotnet restore ./build/BallanceRecordApi.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./BallanceRecordApi.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "BallanceRecordApi.dll"]