FROM mcr.microsoft.com/dotnet/core/sdk as build

COPY . .
RUN dotnet build -c Release WebApi/WebApi.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as prod

COPY --from=build /WebApi/bin/Release/netcoreapp3.1/ .
EXPOSE 80
CMD dotnet WebApi.dll