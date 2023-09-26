FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:5000

#Enviroment variables
ARG ASPNETCORE_URLS=http://+:5000
ARG DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ARG LocalDev=http://localhost:3000
ARG LocalDev2=http://localhost:8000
ARG FE=https://manage-pe.reso.vn
ARG FE1=https://manage.unilo.net
ARG ConnectionStrings_SQLServerDatabase=Server=13.212.101.182;Database=Pre_prod_LoyaltyPlatform;User ID=pre_prod_loyalty_user;Password=Jd5aZWBAuYxpmzf:7SGb6(%Ej;Trusted_Connection=false;MultipleActiveResultSets=true

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build
WORKDIR /src

COPY . .
RUN dotnet restore "./PromotionEngineAPI/WebAPI.csproj"

WORKDIR "/src/PromotionEngineAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WebAPI.dll"]