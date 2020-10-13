FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=0.1.0

COPY BallanceRecordApi.csproj /build/

RUN dotnet restore ./build/BallanceRecordApi.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./BallanceRecordApi.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "BallanceRecordApi.dll"]