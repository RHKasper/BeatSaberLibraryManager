using BeatSaverSharp.Models;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager.WebDownload;

public static class Cache
{
    public static Dictionary<string, BPList> PreFilterBpLists {get; private set;}
    public static Dictionary<string, Beatmap> Beatmaps {get; private set;}
    public static Dictionary<string, Beatmap?> SpotifySearchResults { get; private set; }
    
    /// <summary>
    /// key/ID => file path
    /// </summary>
    public static Dictionary<string, string> MapFolders { get; private set; }

    public static void InitializeCache()
    {
        PreFilterBpLists = FileManager.GetCachedPreFilterBpLists();
        Beatmaps = FileManager.GetCachedBeatmaps();
        MapFolders = FileManager.GetCachedMapFolders();
    }
    
    public static void CacheBeatmap(SongInfo songInfo, Beatmap beatmap)
    {
        Beatmaps[songInfo.hash] = beatmap;
        string path = Path.Combine(FileManager.beatmapsCacheFolderPath, songInfo.hash + ".beatmap");
        File.WriteAllText(path, JsonConvert.SerializeObject(beatmap));
    }
    
    public static void CacheBpListPreFilter(string name, BPList bpList)
    {
        PreFilterBpLists[name] = bpList;
        string path = Path.Combine(FileManager.preFilterBpListsCacheFolderPath, name.SanitizeForFileName() + ".bplist");
        File.WriteAllText(path, JsonConvert.SerializeObject(bpList));
    }

    public static void CacheMapFolder(string mapFolderPath)
    {
        MapFolders[mapFolderPath.GetKeyFromMapFolderPath()] = mapFolderPath;
        FileManager.CopyDirectory(mapFolderPath, Path.Combine(FileManager.mapFolderCacheFolderPath, Path.GetFileName(mapFolderPath)));
    }
}