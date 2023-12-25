using System.Diagnostics;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaberLibraryManager.Outputs;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager;

public class Program
{
    public static async Task Main()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        BeatSaver beatSaverApi = new(nameof(BeatSaberLibraryManager), new System.Version(0, 1));
        
        // download BPLists and wait for them to finish
        List<BPList> filteredBpLists = await PlaylistGetter.GetFilteredBeatSaverBpLists(beatSaverApi);
        List<BPList> unfilteredBpLists = await PlaylistGetter.GetUnfilteredBpLists(beatSaverApi);

        // Write all BpList names to the console
        Console.WriteLine("\n\n==================== Playlists ====================");
        foreach (BPList bpList in filteredBpLists.Concat(unfilteredBpLists))
        {
            Console.WriteLine(bpList.playlistTitle + " (" + bpList.songs.Count + " Maps)");
        }
        Console.WriteLine("========================================\n");

        // start downloading Beatmaps (map info)
        List<Task<Beatmap?>> downloadFilteredBeatmapsTasks = MapDownloader.DownloadBeatmaps(filteredBpLists, beatSaverApi);
        List<Task<Beatmap?>> downloadUnfilteredBeatmapsTasks = MapDownloader.DownloadBeatmaps(unfilteredBpLists, beatSaverApi);
        
        // wait for filtered beatmaps to finish downloading and then filter them
        await downloadFilteredBeatmapsTasks.AwaitAll();
        List<Beatmap> filteredBeatmaps = downloadFilteredBeatmapsTasks.Select(t => t.Result).Where(b => b != null).Cast<Beatmap>().ToList();
        FilterBeatmaps(filteredBeatmaps);
        
        // wait for unfiltered beatmaps to finish downloading
        await downloadFilteredBeatmapsTasks.AwaitAll();
        List<Beatmap> unfilteredBeatmaps = downloadUnfilteredBeatmapsTasks.Select(t => t.Result).Where(b => b != null).Cast<Beatmap>().ToList();

        // prep output and cache directories
        FileManager.PrepareMapZipCacheDirectory();
        FileManager.PrepareOutputDirectories();
        
        // download zip files and output map directories
        await MapDownloader.DownloadZipFiles(unfilteredBeatmaps.Concat(filteredBeatmaps).ToHashSet());

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