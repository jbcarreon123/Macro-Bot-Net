using Discord.Interactions;
using Octokit;
using Discord.WebSocket;
using Develeon64.MacroBot.Utils;
using Discord;
using Develeon64.MacroBot.Commands;
using Develeon64.MacroBot.Services;
using Newtonsoft.Json;
using System.Net;

namespace Develeon64.MacroBot.Utils {
    public class StatusLoop {
        public ulong messageid = 1;

        public async Task DelMsgs(int count, ITextChannel channel) {
            await channel.DeleteMessagesAsync(await channel.GetMessagesAsync(count).FlattenAsync());
        }

        public StatusLoop(DiscordSocketClient client) {
            for (;;) {
                var channel = (client.GetGuild(ConfigManager.GlobalConfig.TestGuildId).GetChannel(ConfigManager.GlobalConfig.Channels.UpdateChannelId) as ITextChannel);
                try {
                    var msg = Task.Run<IMessage>(async () => await channel!.GetMessageAsync(messageid));
                    var content = msg.Result.Content;
                } catch (Exception) {
                    Task.Run(async () => await DelMsgs(10, channel));
                    Thread.Sleep(1000);

                    var msg = Task.Run<IUserMessage>(async () => await channel!.SendMessageAsync("Please wait..."));
                    messageid = msg.Result.Id;
                }

                int website = 0;
                int extstor = 0;
                int exstapi = 0;
                int updtapi = 0;

                StatusCommands.AsIf<WebResponse>(StatusCommands.CheckStatus("https://macro-deck.app/"), response => {
                    website = 1;
                });
                StatusCommands.AsIf<WebResponse>(StatusCommands.CheckStatus("https://extensionstore.macro-deck.app/"), response => {
                    extstor = 1;
                });
                StatusCommands.AsIf<WebResponse>(StatusCommands.CheckStatus("https://extensionstore.api.macro-deck.app/"), response => {
                    exstapi = 1;
                });
                StatusCommands.AsIf<WebResponse>(StatusCommands.CheckStatus("https://update.api.macro-deck.app/"), response => {
                    updtapi = 1;
                });

                EmbedBuilder embed = new();
                embed.WithTitle("Macro Deck Service Status");
                embed.WithDescription((website == 1 && extstor == 1 && exstapi == 1 && updtapi == 1)? "✅ All services are online. You should have no problems." : "⚠ There is a problem on one or more services. We are working to the issue.");
                string type = (website == 1)? "🟢\r\n" : "🔴\r\n";
                string typ2 = (extstor == 1)? "🟢\r\n" : "🔴\r\n";
                string typ3 = (exstapi == 1)? "🟢\r\n" : "🔴\r\n";
                string typ4 = (updtapi == 1)? "🟢" : "🔴";
                embed.AddField("Status", $"{type}{typ2}{typ3}{typ4}", true);
                embed.AddField("Service", "Macro Deck website\r\nExtension Store\r\nExtension Store API\r\nMD Update API", true);
                embed.WithFooter("Updated on");
                embed.WithCurrentTimestamp();

                Task.Run<IUserMessage>(async () => await channel!.ModifyMessageAsync(messageid, m => {m.Embed = embed.Build();}));

                Thread.Sleep(300000);
            }
        }
    }
}