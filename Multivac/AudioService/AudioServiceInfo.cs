﻿using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Multivac.Main
{
    public partial class AudioService
    {
        public async Task NowPlayingAsync(SocketCommandContext Context)
        {
            ulong guildId = Context.Guild.Id;

            if (!IsMusicPlaying(guildId))
            {
                await Context.Channel.SendMessageAsync("nothing is playing");
                return;
            }

            GuildPlaylist.TryGetValue(guildId, out var value);
            var track = value.Tracklist.First().track;
            var requester = _client.GetUser(value.Tracklist.First().requesterId);

            await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                .AddField("Now Playing",
                    $"**Title:** [{track.Title}]({track.Url})\n" +
                    $"**Artist:** {track.Author}\n" +
                    $"**Length:** {track.Length}\n" +
                    $"**Requested by:** {requester.Mention}")
                .Build());
        }

        public async Task UpNextAsync(SocketCommandContext Context)
        {
            ulong guildId = Context.Guild.Id;

            if (!IsMusicPlaying(guildId))
            {
                await Context.Channel.SendMessageAsync("nothing is playing");
                return;
            }

            GuildPlaylist.TryGetValue(guildId, out var value);
            var nextTrack = value.Tracklist.ElementAt(1).track;
            var requester = _client.GetUser(value.Tracklist.First().requesterId);

            await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                .AddField("Now Playing",
                    $"**Title:** [{nextTrack.Title}]({nextTrack.Url})\n" +
                    $"**Artist:** {nextTrack.Author}\n" +
                    $"**Length:** {nextTrack.Length}\n" +
                    $"**Requested by:** {requester.Mention}")
                .Build());
        }


        public async Task ShowVolumeAsync(SocketCommandContext Context)
        {
            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            if (player == null)
            {
                await Context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                    .AddField("Error", "Player does not exist in theis guild")
                    .Build());
                return;
            }

            await Context.Channel.SendMessageAsync($"Volume is set to (grab volume from db)");
        }

        public async Task ShowQueueAsync(SocketCommandContext Context)
        {
            GuildPlaylist.TryGetValue(Context.Guild.Id, out var value);

            var tracknames = value.Tracklist.Select(x => x.track.Title).ToArray();

            await Context.Channel.SendMessageAsync($"```{string.Join('\n', tracknames)}```");
        }

    }
}
