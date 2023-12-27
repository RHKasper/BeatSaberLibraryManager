using BeatSaverSharp;
using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.WebDownload;

public static class MapDownloader
{
    public static async Task<List<Task<Beatmap?>>> DownloadBeatmaps(IEnumerable<BPList> bpLists, BeatSaver beatSaverApi)
    {
        List<Task<Beatmap?>> beatmapDownloadTasks = new();
        
        foreach (BPList bpList in bpLists)
        {
            foreach (SongInfo songInfo in bpList.songs)
            {
                Task<Beatmap?> task = GetBeatmap(songInfo, beatSaverApi);
                beatmapDownloadTasks.Add(task);
                await task;
            }
        }

        return beatmapDownloadTasks;
    }

    public static async Task DownloadZipFiles(HashSet<Beatmap> beatmaps, BeatSaver beatSaverClient)
    {
        Console.WriteLine("Requesting " + beatmaps.Count + " map zip files");

        foreach (Beatmap beatmap in beatmaps)
        {
            var task = DownloadAndUnzipZipFile(beatmap, beatSaverClient);
            await task;
        }

    }

    private static async Task<string?> DownloadAndUnzipZipFile(Beatmap beatmap, BeatSaver beatSaver)
    {
        string mapDirectory = FileManager.GetMapDirectory(beatmap);
        
        if (Cache.MapFolders.ContainsKey(beatmap.ID))
        {
            Console.WriteLine("\tFound map folder in cache: " + beatmap.Name);
            FileManager.CopyDirectory(Cache.MapFolders[beatmap.ID], mapDirectory);
            return Cache.MapFolders[beatmap.ID];
        }
        else
        {
            Console.WriteLine("Downloading .zip byte[] for " + beatmap.Name);

            Task<byte[]?> downloadZipContents = beatSaver.DownloadZIP(beatmap.LatestVersion);
            await downloadZipContents;

            if (downloadZipContents is { IsCompletedSuccessfully: true, Result: not null })
            {
                string zipFilePath = FileManager.GetZipFilePath(beatmap);
                Console.WriteLine("\tDownloaded .zip byte[] for " + beatmap.Name);
                await File.WriteAllBytesAsync(zipFilePath, downloadZipContents.Result);
                FileManager.UnzipFile(zipFilePath, mapDirectory);
                Console.WriteLine("\tUnzipped map to: " + mapDirectory);
                Cache.CacheMapFolder(mapDirectory);
                
                return mapDirectory;
            }
            else
            {
                Console.WriteLine("Failed to download zip file for " + beatmap.Name);
            }
        }

        return null;
    }

    private static async Task<Beatmap?> GetBeatmap(SongInfo songInfo, BeatSaver beatSaverApi)
    {
        if (Cache.Beatmaps.ContainsKey(songInfo.hash))
        {
            Console.WriteLine("\tBeatmap found in cache for: " + songInfo.songName + " - " + songInfo.hash);
            return Cache.Beatmaps[songInfo.hash];
        }
        else
        {
            Console.WriteLine("Downloading Beatmap for: " + songInfo.songName + " - " + songInfo.hash);
            Beatmap? beatmap = await beatSaverApi.BeatmapByHash(songInfo.hash);
            if (beatmap != null)
            {
                Cache.CacheBeatmap(songInfo, beatmap);
            }
            Console.WriteLine("\tDownloaded Beatmap for: " + songInfo.songName + " - " + songInfo.hash);
            return beatmap;
        }
    }
}