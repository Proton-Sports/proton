using Discord.Rest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Infrastructure.Authentication
{
    public class DiscordHandler
    {
        private readonly IConfiguration configuration;
        private readonly DiscordRestClient DiscordRest;
        public DiscordHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.DiscordRest = new DiscordRestClient();
            DiscordRest.LoginAsync(Discord.TokenType.Bot, configuration["Discord:Token"]);
        }
    }
}
