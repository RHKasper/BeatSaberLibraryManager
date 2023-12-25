using System.Diagnostics;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaberLibraryManager.Outputs;
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

        foreach (BPList bpList in filteredBpLists.Concat(unfilteredBpLists))
        {
            Console.WriteLine(bpList.playlistTitle);
        }

        // start downloading Beatmaps (map info)
        List<Task<Beatmap?>> downloadFilteredBeatmapsTasks = DownloadBeatmaps(filteredBpLists, beatSaverApi);
        List<Task<Beatmap?>> downloadUnfilteredBeatmapsTasks = DownloadBeatmaps(unfilteredBpLists, beatSaverApi);
        
        // wait for filtered beatmaps to finish downloading and then filter them
        await downloadFilteredBeatmapsTasks.AwaitAll();
        List<Beatmap> filteredBeatmaps = downloadFilteredBeatmapsTasks.Select(t => t.Result).Where(b => b != null).Cast<Beatmap>().ToList();
        FilterOutput(filteredBeatmaps);
        
        // wait for unfiltered beatmaps to finish downloading
        await downloadFilteredBeatmapsTasks.AwaitAll();
        List<Beatmap> unfilteredBeatmaps = downloadUnfilteredBeatmapsTasks.Select(t => t.Result).Where(b => b != null).Cast<Beatmap>().ToList();

        // prep output and cache directories
        FileManager.PrepareMapZipCacheDirectory();
        FileManager.PrepareOutputDirectories();
        
        // download zip files and output map directories
        await DownloadZipFiles(unfilteredBeatmaps.Concat(filteredBeatmaps).ToHashSet());

        Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
    }
    
    private static void FilterOutput(List<Beatmap> beatmaps)
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

    private static List<Task<Beatmap?>> DownloadBeatmaps(IEnumerable<BPList> bpLists, BeatSaver beatSaverApi)
    {
        List<Task<Beatmap?>> beatmapDownloadTasks = new();
        
        foreach (BPList bpList in bpLists)
        {
            Console.WriteLine("\n" + bpList.playlistTitle);
            Console.WriteLine("============================================");
            
            foreach (SongInfo songInfo in bpList.songs)
            {
                Console.WriteLine(songInfo.songName + " - " + songInfo.hash);
                beatmapDownloadTasks.Add(beatSaverApi.BeatmapByHash(songInfo.hash));
            }
        }

        return beatmapDownloadTasks;
    }

    private static async Task DownloadZipFiles(HashSet<Beatmap> beatmaps)
    {
        Dictionary<Task, Beatmap> zipFileDownloadAndUnzipTasks = new();
        foreach (Beatmap beatmap in beatmaps)
        {
            Console.WriteLine("Requesting download for " + beatmap.Name);
            zipFileDownloadAndUnzipTasks.Add(DownloadAndUnzipZipFile(beatmap), beatmap);
        }

        Console.WriteLine("Requested " + beatmaps.Count + " map zip files");
        
        // wait ten seconds
        await Task.Delay(10000);

        int retryCount = 0;
        int previousNumberOfWaitingTasks = zipFileDownloadAndUnzipTasks.Count;
        int secondsSinceNumWaitingTasksChanged = 0;
        
        // whenever the number of waiting maps remains unchanged for 10 seconds, restart the tasks that haven't resolved. 
        while (zipFileDownloadAndUnzipTasks.Any() && retryCount < 3)
        {
            Console.WriteLine("Waiting on " + previousNumberOfWaitingTasks + " map zip downloads");
            await Task.Delay(1000);
            
            foreach (Task task in zipFileDownloadAndUnzipTasks.Keys)
            {
                if (task.IsCompletedSuccessfully)
                {
                    zipFileDownloadAndUnzipTasks.Remove(task);
                }
            }

            if (zipFileDownloadAndUnzipTasks.Count != previousNumberOfWaitingTasks)
            {
                previousNumberOfWaitingTasks = zipFileDownloadAndUnzipTasks.Count;
                secondsSinceNumWaitingTasksChanged = 0;
            }
            else
            {
                secondsSinceNumWaitingTasksChanged += 1;
                if (secondsSinceNumWaitingTasksChanged > 3)
                {
                    // retry unresolved tasks
                    retryCount++;
                    Beatmap[] beatmapsToRetry = zipFileDownloadAndUnzipTasks.Values.ToArray();
                    zipFileDownloadAndUnzipTasks.Clear();
                    
                    foreach (Beatmap beatmap in beatmapsToRetry)
                    {
                        Console.WriteLine("Retrying (#" + retryCount + ") download for " + beatmap.Name);
                        zipFileDownloadAndUnzipTasks.Add(DownloadAndUnzipZipFile(beatmap), beatmap);
                    }
                }
            }
        }
    }

    private static async Task DownloadAndUnzipZipFile(Beatmap beatmap)
    {
        var downloadZipContents = beatmap.LatestVersion.DownloadZIP();
        await downloadZipContents;

        if (downloadZipContents is { IsCompletedSuccessfully: true, Result: not null })
        {
            Console.WriteLine("Downloaded .zip byte[] for " + beatmap.Name);
            
            string zipFilePath = FileManager.GetZipFilePath(beatmap);
            await File.WriteAllBytesAsync(zipFilePath, downloadZipContents.Result);
            FileManager.UnzipFile(zipFilePath, out var mapFolderPath);
            Console.WriteLine("Unzipped map to: " + mapFolderPath);
        }
        else
        {
            Console.WriteLine("Failed to download zip file for " + beatmap.Name);
        }
    }
}