using Develeon64.MacroBot.Utils;
using Develeon64.MacroBot.Models;
using Develeon64.MacroBot.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Develeon64.MacroBot.Commands
{
    [Group("roles", "roles")]
    public class RolesCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("list", "Show a list of all roles")]
        public async Task list()
        {
            DiscordEmbedBuilder embed = new()
            {
                Title = Context.Guild.Name,
                Description = "Here is the list of roles of this server."
            };
            embed.WithThumbnailUrl(Context.Guild.IconUrl);
            embed.AddField("Roles", string.Join("\r\n ", Context.Guild.Roles.Select(r => r.Mention)));
            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("give", "Give a user a role on this server")]
        public async Task give([Summary(description: "the user to give the role")] IUser user, [Summary(description: "the role ID")] string roleid)
        {
            DiscordEmbedBuilder embed = new()
            {
                Title = $"Give {user.Username} the role {roleid}",
                Description = $"Do you want to give the user <@{user.Id}> the role <@&{roleid}>?"
            };
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.AddField("ㅤ", "**__Information__**");
            embed.AddField("User", user.Mention, true);
            embed.AddField("Giver", Context.User.Mention, true);
            embed.AddField("Role to Give", $"<@&{roleid}>", true);

            ComponentBuilder componentBuilder = new ComponentBuilder();
            componentBuilder.WithButton("Yes", "role-give-yes");
            componentBuilder.WithButton("No", "role-give-no");

            Context.Client.ButtonExecuted += rolegiveh;

            await RespondAsync($"RID: {roleid}, UID: {user.Id}", embed: embed.Build(), components: componentBuilder.Build());
        }

        [SlashCommand("select", "Shows a role selector")]
        public async Task roleselect([Summary(description: "the channel")] IChannel? channel = null)
        {

            if (channel == null) { channel = Context.Channel; }
            await RespondAsync($"Sent a select message on channel <#{channel.Id}>", ephemeral: true);

            var chnl = Context.Guild.GetTextChannel(channel.Id);

            DiscordEmbedBuilder embed = new()
            {
                Title = Context.Guild.Name,
                Description = "Here you can select your role in this channel",
            };

            ComponentBuilder componentBuilder = new ComponentBuilder();
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select a option")
                .WithCustomId("role-select")
                .WithMaxValues(3)
                .WithMinValues(0)
                .AddOption("Macro Deck Updates", "role-a", "Notify you every Macro Deck announcements or updates")
                .AddOption("Plugin Updates", "role-b", "Notify you on specific plugins announcements or updates")
                .AddOption("Helper", "role-c", "Helper helps the moderators to answer problems and queries on Macro Deck");

            componentBuilder.WithSelectMenu(menuBuilder);
            Context.Client.SelectMenuExecuted += roleselecth;

            await chnl.SendMessageAsync(embed: embed.Build(), components: componentBuilder.Build());
        }

        public async Task roleselecth(SocketMessageComponent arg)
        {
            foreach (string i in arg.Data.Values)
            {
                if (i == "role-a")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1001693907853258853));
                }
                if (i == "role-b")
                {
                    DiscordEmbedBuilder embed = new()
                    {
                        Title = Context.Guild.Name,
                        Description = "What plugins you want to be notified on?",
                    };

                    ComponentBuilder componentBuilder = new ComponentBuilder();
                    SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                        .WithPlaceholder("Select a option")
                        .WithCustomId("role-select")
                        .WithMaxValues(5)
                        .WithMinValues(0)
                        .AddOption("Windows Utils [SuchByte.WindowsUtils]", "plugin-windows-utils")
                        .AddOption("SpotifyPlugin [Develeon64.SpotifyPlugin]", "plugin-spotify")
                        .AddOption("Magic Home [Xenox003.MagicHome]", "plugin-magic-home")
                        .AddOption("AutoHotkey Plugin [jbcarreon123.AHKPlugin]", "plugin-ahk")
                        .AddOption("Discord Plugin [RecklessBoon.DiscordPlugin]", "plugin-discord");

                    componentBuilder.WithSelectMenu(menuBuilder);
                    Context.Client.SelectMenuExecuted += roleselectp;

                    await arg.RespondAsync(embed: embed.Build(), components: componentBuilder.Build());
                }
                if (i == "role-c")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1009681479535697981));
                }
            }

            await arg.RespondAsync("Done. You should have the role now.", ephemeral: true);
        }

        public async Task rolegiveh(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "role-give-yes":
                    await RespondAsync($"{arg.Message.Content}");
                    string[] comp = arg.Message.Content.Split(", ");
                    var msg = comp[0];
                    msg = msg.Replace("RID: ", "");
                    var user = comp[1];
                    user = user.Replace("UID: ", "");
                    await Context.Channel.SendMessageAsync($"{msg}, {user}");
                    break;
            }
        }

        public async Task roleselectp(SocketMessageComponent arg)
        {
            foreach (string i in arg.Data.Values)
            {
                if (i == "plugin-windows-utils")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1009991733649616917));
                }
                if (i == "plugin-spotify")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1009991816977862666));
                }
                if (i == "plugin-magic-home")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1009991865434656808));
                }
                if (i == "plugin-ahk")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1009991871281504407));
                }
                if (i == "plugin-windows-utils")
                {
                    await Context.Guild.GetUser(arg.User.Id).AddRoleAsync(Context.Guild.GetRole(1009991962666995783));
                }
            }
        }
    }
}
