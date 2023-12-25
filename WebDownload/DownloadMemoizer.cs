using System.Collections.Concurrent;
using BeatSaverSharp.Models;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.WebDownload;

public static class DownloadMemoizer
{
    // Spotify track ID to Beatmap 
    private static readonly ConcurrentDictionary<string, Beatmap> SpotifyTrackIdToBeatmap = new();

    public static void RecordSpotifyTrackToBeatmapSearchResult(FullTrack fullTrack, Beatmap beatmap)
    {
        SpotifyTrackIdToBeatmap.TryAdd(fullTrack.Id, beatmap);
    }
    public static bool TryGetMemoizedBeatmap(FullTrack spotifyTrack, out Beatmap beatmap)
    {
        return SpotifyTrackIdToBeatmap.TryGetValue(spotifyTrack.Id, out beatmap);
    }
}