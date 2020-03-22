FROM mcr.microsoft.com/dotnet/core/sdk as build

COPY ./WebApi/WebApi.csproj ./WebApi/WebApi.csproj
RUN dotnet restore WebApi/WebApi.csproj

COPY ./WebApi ./WebApi
RUN dotnet build -c Release WebApi/WebApi.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as prod

COPY --from=build /WebApi/bin/Release/netcoreapp3.1/ .
EXPOSE 5000
CMD dotnet WebApi.dll