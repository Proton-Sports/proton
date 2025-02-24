set windows-shell := ["cmd.exe", "/C"]

default: publish run

dev:
    just publish -c Debug
    just run

build: build-client build-server

build-server:
    dotnet build ./src/Proton.Server.Resource/Proton.Server.Resource.csproj

build-client:
    dotnet build ./src/Proton.Client.Resource/Proton.Client.Resource.csproj

run: publish
    altv-server

publish *rest:
    just publish-client {{rest}}
    just publish-server {{rest}}

publish-server *rest:
    dotnet publish ./src/Proton.Server.Resource/Proton.Server.Resource.csproj {{rest}}

publish-client *rest:
    dotnet publish ./src/Proton.Client.Resource/Proton.Client.Resource.csproj {{rest}}

update: update-server update-modules update-data

update-server:
    wget -N https://cdn.alt-mp.com/voice-server/release/x64_linux/altv-voice-server https://cdn.alt-mp.com/coreclr-module/release/x64_linux/AltV.Net.Host.dll https://cdn.alt-mp.com/coreclr-module/release/x64_linux/AltV.Net.Host.runtimeconfig.json https://cdn.alt-mp.com/data/release/update.json https://cdn.alt-mp.com/server/release/x64_linux/altv-crash-handler https://cdn.alt-mp.com/server/release/x64_linux/altv-server

update-modules:
    wget -P ./modules -N https://cdn.alt-mp.com/coreclr-module/release/x64_linux/modules/libcsharp-module.so

update-data:
    wget -P ./data -N https://cdn.alt-mp.com/data/release/data/vehmodels.bin https://cdn.alt-mp.com/data/release/data/vehmods.bin https://cdn.alt-mp.com/data/release/data/clothes.bin https://cdn.alt-mp.com/data/release/data/pedmodels.bin https://cdn.alt-mp.com/data/release/data/rpfdata.bin https://cdn.alt-mp.com/data/release/data/weaponmodels.bin

update-windows: update-windows-server update-windows-modules update-windows-data

update-windows-server:
    wget -N https://cdn.alt-mp.com/voice-server/release/x64_win32/altv-voice-server.exe https://cdn.alt-mp.com/coreclr-module/release/x64_win32/AltV.Net.Host.dll https://cdn.alt-mp.com/coreclr-module/release/x64_win32/AltV.Net.Host.runtimeconfig.json https://cdn.alt-mp.com/data/release/update.json https://cdn.alt-mp.com/server/release/x64_win32/altv-crash-handler.exe https://cdn.alt-mp.com/server/release/x64_win32/altv-server.exe

update-windows-modules:
    wget -P ./modules -N https://cdn.alt-mp.com/coreclr-module/release/x64_win32/modules/csharp-module.dll

update-windows-data:
    wget -P ./data -N https://cdn.alt-mp.com/data/release/data/vehmodels.bin https://cdn.alt-mp.com/data/release/data/vehmods.bin https://cdn.alt-mp.com/data/release/data/clothes.bin https://cdn.alt-mp.com/data/release/data/pedmodels.bin https://cdn.alt-mp.com/data/release/data/rpfdata.bin https://cdn.alt-mp.com/data/release/data/weaponmodels.bin

optimize:
    dotnet ef dbcontext optimize --project src/Proton.Server.Resource -n Proton.Server.Infrastructure.Persistence.CompiledModels -o ../Proton.Server.Infrastructure/Persistence/CompiledModels

mig +rest:
    dotnet ef migrations {{rest}} --project src/Proton.Server.Resource

db +rest:
    dotnet ef database {{rest}} --project src/Proton.Server.Resource

bundle:
    dotnet ef migrations bundle -r linux-x64 --project src/Proton.Server.Resource
