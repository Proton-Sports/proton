using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Proton.Server.Infrastructure.Authentication;
using Proton.Server.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource
{
    internal class Authentication(DiscordHandler discord, DefaultDbContext dbContext) : IScript
    {
        private readonly DiscordHandler discord = discord;
        private readonly DefaultDbContext dbContext = dbContext;

        /// <summary>
        /// Checking if the OAuth Token is still valid and offer to login as User
        /// </summary>
        [AsyncScriptEvent(ScriptEventType.PlayerConnect)]
        public async Task OnPlayerConnect(IPlayer p, string reason)
        {
            
        }

        /// <summary>
        /// The client sends the Discord OAuth Challenge Token
        /// </summary>
        [AsyncClientEvent("authentication:token:exchange")]
        public async Task OnTokenExchange(IPlayer p, string token)
        {

        }
    }
}
