# =======================================================
# STAGE 1: BUILD (The "Kitchen")
# We use the full .NET SDK to compile the code
# Registry: mcr.microsoft.com → Microsoft’s trusted container registry.
# Image: dotnet/sdk → Full SDK (runtime + build tools).
# Tag: 9.0 → Version of .NET SDK you want.
# =======================================================
#Use the official .NET 8 SDK image as the build stage(base image ).
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build  
WORKDIR /src #Set /src as the working directory inside the container for all following commands.

# 1. Copy the Project Files (csproj) for ALL layers
# We do this first to cache the NuGet restore step. 
# If you don't change dependencies, Docker skips this part next time (Fast builds!)
# Why Copy Only .csproj Files First
#   The .csproj files contain references to all NuGet packages your app needs. Docker uses these files to run dotnet restore and download dependencies.
#   If you copy all source code first, any small code change invalidates the cache → Docker re‑downloads all NuGet packages.
#   By copying only .csproj files first, Docker can cache the restore layer.
#   As long as dependencies don’t change, Docker reuses the cached NuGet packages, skipping the restore step.
#→ This makes rebuilds much faster.
COPY ["MyPortfolio.Web/MyPortfolio.Web.csproj", "MyPortfolio.Web/"]
COPY ["MyPortfolio.Business/MyPortfolio.Business.csproj", "MyPortfolio.Business/"]
COPY ["MyPortfolio.DataAccess/MyPortfolio.DataAccess.csproj", "MyPortfolio.DataAccess/"]
COPY ["MyPortfolio.Models/MyPortfolio.Models.csproj", "MyPortfolio.Models/"]
# (If you created the EmailService, add it here too, or ignore for now)


# 2. Restore Dependencies
# This downloads all the NuGet packages required by the solution
RUN dotnet restore "MyPortfolio.Web/MyPortfolio.Web.csproj"

# 3. Copy the rest of the source code
# Since we already ignored bin/obj, this is safe
# COPY <host source path> <image destination path> 
#   First . (source):Refers to the current directory on your host machine (the build context).
#       Example: If you run docker build in your project root, this means “copy everything from here.”
# Second . (destination):
#   Refers to the current working directory inside the container (set earlier with WORKDIR /src).
#   So all files from your host’s current directory are copied into /src inside the container.
COPY . .

# 4. Build and Publish
# We switch to the Web folder and publish the Release version
#WORKDIR "/src/MyPortfolio.Web"
RUN dotnet publish "MyPortfolio.Web/MyPortfolio.Web.csproj" -c Release -o /app/publish

# =======================================================
# STAGE 2: RUNTIME (The "Serving Plate")
# We use the lightweight ASP.NET runtime (no code, just DLLs)
# =======================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose ports (8080 is the new default for .NET 8 inside containers)
# The EXPOSE instruction in a Dockerfile declares which ports the container will listen on at runtime.
# 8080 → HTTP traffic (non‑TLS).8081 → HTTPS traffic (TLS).
EXPOSE 8080
EXPOSE 8081

# Copy the compiled files FROM the build stage (Stage 1) to this stage
# The publish step does put files into /app/publish, but only in the build stage’s container filesystem.
# When Docker moves to the runtime stage, that filesystem is discarded.
# Without COPY --from=build, your runtime image would be empty — no DLLs, no app to run.
COPY --from=build /app/publish .

# Define the entry point: "When this container starts, run this DLL"
ENTRYPOINT ["dotnet", "MyPortfolio.Web.dll"]