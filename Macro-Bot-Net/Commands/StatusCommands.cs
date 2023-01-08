using Discord.Interactions;
using Octokit;
using Discord.WebSocket;
using Develeon64.MacroBot.Utils;
using Discord;
using Newtonsoft.Json;
using System.Net;

namespace Develeon64.MacroBot.Commands {
    public class StatusCommands : InteractionModuleBase<SocketInteractionContext> {
        [SlashCommand("status", "Check status of each Macro Deck URL endpoints")]
        public async Task Status() {
            await DeferAsync();

            int website = 0;
            int webclie = 0;
            int extstor = 0;
            int exstapi = 0;
            int updtapi = 0;

            AsIf<WebResponse>(CheckStatus("https://macro-deck.app/"), response => {
                website = 1;
            });
            AsIf<WebResponse>(CheckStatus("https://web.macro-deck.app/"), response => {
                webclie = 1;
            });
            AsIf<WebResponse>(CheckStatus("https://extensionstore.macro-deck.app/"), response => {
                extstor = 1;
            });
            AsIf<WebResponse>(CheckStatus("https://extensionstore.api.macro-deck.app/"), response => {
                exstapi = 1;
            });
            AsIf<WebResponse>(CheckStatus("https://update.api.macro-deck.app/"), response => {
                updtapi = 1;
            });

            EmbedBuilder embed = new();
            embed.WithTitle("Macro Deck Service Status");
            embed.WithDescription((website == 1 && extstor == 1 && exstapi == 1 && updtapi == 1)? "✅ All services are online. You should have no problems." : "⚠ There is a problem on one or more services. We are working to the issue.");
            string type = (website == 1)? "🟢\r\n" : "🔴\r\n";
            string typ2 = (webclie == 1)? "🟢\r\n" : "🔴\r\n";
            string typ3 = (extstor == 1)? "🟢\r\n" : "🔴\r\n";
            string typ4 = (exstapi == 1)? "🟢\r\n" : "🔴\r\n";
            string typ5 = (updtapi == 1)? "🟢" : "🔴";
            embed.AddField("Service", "Macro Deck website\r\nWeb Client\r\nExtension Store\r\nExtension Store API\r\nMD Update API", true);
            embed.AddField("Status", $"{type}{typ2}{typ3}{typ4}{typ5}", true);
            embed.WithFooter("Updated on");
            embed.WithCurrentTimestamp();

            await FollowupAsync(embed: embed.Build());
        }

        public static WebResponse? CheckStatus(string url) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
            request.Method = "HEAD";
            try {
                return request.GetResponse();
            } catch (WebException wex)
            {
                if (wex.Status != WebExceptionStatus.Success) {
                    return null;
                }
            }
            return null;
        }

        public static void AsIf<T>(object? value, Action<T> action) where T : class
        {
            T? t = value as T;
            if (t != null)
            {
                action(t);
            }
        }
    }
}