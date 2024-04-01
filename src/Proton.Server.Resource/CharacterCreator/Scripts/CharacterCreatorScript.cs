using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Proton.Server.Infrastructure.CharacterCreator;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.CharacterCreator.Scripts;

public class CharacterCreatorScript : IStartup
{
    public CharacterCreatorScript(CharacterHandler characterHandler)
    {
        AltAsync.OnClient<IPlayer>("server:testCharCreator", StartCharacterCreator);
    }

    private void StartCharacterCreator(IPlayer player)
    {
        player.Dimension = (int)(int.MaxValue - player.Id);
        player.Frozen = true;
        player.Position = new Position(402.90833f, -996.61365f, -99.00013f);
        player.Rotation = new Rotation(0, 0, 3.14f);
        player.Visible = false;
        
        player.Emit("characterClient:startCharacterCreator");
    }
}