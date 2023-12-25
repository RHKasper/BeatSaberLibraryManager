using BeatSaberLibraryManager.Outputs;
using BeatSaverSharp;
using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.WebDownload;

public static class MapDownloader
{
    public static List<Task<Beatmap?>> DownloadBeatmaps(IEnumerable<BPList> bpLists, BeatSaver beatSaverApi)
    {
        List<Task<Beatmap?>> beatmapDownloadTasks = new();
        
        foreach (BPList bpList in bpLists)
        {
            Console.WriteLine("\n" + bpList.playlistTitle + " (" + bpList.songs.Count + " Maps)");
            Console.WriteLine("============================================");
            
            foreach (SongInfo songInfo in bpList.songs)
            {
                Console.WriteLine(songInfo.songName + " - " + songInfo.hash);
                beatmapDownloadTasks.Add(beatSaverApi.BeatmapByHash(songInfo.hash));
            }
        }

        return beatmapDownloadTasks;
    }

    public static async Task DownloadZipFiles(HashSet<Beatmap> beatmaps)
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

                    secondsSinceNumWaitingTasksChanged = 0;
                }
            }
        }
    }

    private static async Task DownloadAndUnzipZipFile(Beatmap beatmap)
    {
        Task<byte[]?> downloadZipContents = beatmap.LatestVersion.DownloadZIP();
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