using Develeon64.MacroBot.Utils;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Develeon64.MacroBot.Commands
{
    public class OtherCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("getinviteurl", "Gets the invite URL of the Macro Deck official Discord server")]
        [EnabledInDm(true)]
        public async Task GetInviteURL()
        {
            await RespondAsync("https://discord.gg/AG3TeP9pAs");
        }
    }
}
