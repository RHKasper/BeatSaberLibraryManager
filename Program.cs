using System.Diagnostics;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager;

public class Program
{
    public static async Task Main()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        BeatSaver beatSaverApi = new(nameof(BeatSaberLibraryManager), new System.Version(0, 1));
        SpotifyClient spotify = await SpotifyPlaylistConverter.CreateSpotifyClient();
        Console.WriteLine("Spotify and BeatSaver client init complete");
        
        // download BPLists and wait for them to finish
        Console.WriteLine("Downloading and generating BeatSaber Playlists");
        List<BPList> filteredBpLists = await PlaylistGetter.GetFilteredBeatSaverBpLists(beatSaverApi);
        List<BPList> unfilteredBpLists = await PlaylistGetter.GetUnfilteredBpLists(beatSaverApi, spotify);

        // Write all BpList names to the console
        Console.WriteLine("\n\n==================== Downloaded and Generated Playlists ====================");
        foreach (BPList bpList in filteredBpLists.Concat(unfilteredBpLists))
        {
            Console.WriteLine(bpList.playlistTitle + " (" + bpList.songs.Count + " Maps)");
        }
        Console.WriteLine("========================================\n");
        
        // start downloading Beatmaps (map info)
        Console.WriteLine("Downloading Beatmaps (map file metadata) for map filtering");
        List<Task<Beatmap?>> downloadFilteredBeatmapsTasks = MapDownloader.DownloadBeatmaps(filteredBpLists, beatSaverApi);
        List<Task<Beatmap?>> downloadUnfilteredBeatmapsTasks = MapDownloader.DownloadBeatmaps(unfilteredBpLists, beatSaverApi);
        
        // wait for filtered beatmaps to finish downloading and then filter them
        await downloadFilteredBeatmapsTasks.AwaitAll();
        List<Beatmap> filteredBeatmaps = downloadFilteredBeatmapsTasks.Select(t => t.Result).Where(b => b != null).Cast<Beatmap>().ToList();
        FilterBeatmaps(filteredBeatmaps);
        
        // wait for unfiltered beatmaps to finish downloading
        await downloadFilteredBeatmapsTasks.AwaitAll();
        List<Beatmap> unfilteredBeatmaps = downloadUnfilteredBeatmapsTasks.Select(t => t.Result).Where(b => b != null).Cast<Beatmap>().ToList();
        Console.WriteLine("Beatmap downloading and filtering complete.");

        // prep working directories
        FileManager.PrepareWorkingDirectories();
        
        // download zip files
        Console.WriteLine("Starting map .zip file downloads");
        await MapDownloader.DownloadZipFiles(unfilteredBeatmaps.Concat(filteredBeatmaps).ToHashSet());
        
        // Output map folders and playlist files
        FileManager.OutputPlaylists(filteredBpLists.Concat(unfilteredBpLists));

        Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
    }
    
    private static void FilterBeatmaps(List<Beatmap> beatmaps)
    {
        for (var i = beatmaps.Count - 1; i >= 0; i--)
        {
            if (beatmaps[i].FailsAnyQualityFilter())
            {
                Console.WriteLine("Filtering out: " + beatmaps[i].Name + " - " + beatmaps[i].Stats.Score + "; " + beatmaps[i].LatestVersion.Difficulties.Most(d => d.Parity.Errors).Parity.Errors + " parity errors");
                beatmaps.RemoveAt(i);
            }
        }
    }
}