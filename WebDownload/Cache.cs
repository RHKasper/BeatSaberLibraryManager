using BeatSaberLibraryManager.Inputs;
using BeatSaverSharp.Models;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.WebDownload;

public static class Cache
{
    /// <summary>
    /// Playlist name (in <see cref="Playlists"/> => BPList
    /// </summary>
    public static Dictionary<string, BPList> PreFilterBpLists {get; private set;}
    
    // hash => Beatmap
    public static Dictionary<string, Beatmap> Beatmaps {get; private set;}
    
    /// <summary>
    /// FullTrack ID => Beatmap
    /// </summary>
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
        SpotifySearchResults = FileManager.GetCachedSpotifySearchResults();
    }
    
    public static void CacheBeatmap(SongInfo songInfo, Beatmap beatmap)
    {
        Beatmaps[songInfo.hash] = beatmap;
        string path = Path.Combine(FileManager.beatmapsCacheFolderPath, songInfo.hash + FileExtensions.Beatmap);
        File.WriteAllText(path, JsonConvert.SerializeObject(beatmap));
    }
    
    public static void CacheBpListPreFilter(string name, BPList bpList)
    {
        PreFilterBpLists[name] = bpList;
        string path = Path.Combine(FileManager.preFilterBpListsCacheFolderPath, name.SanitizeForFileName() + FileExtensions.BpList);
        File.WriteAllText(path, JsonConvert.SerializeObject(bpList));
    }

    public static void CacheMapFolder(string mapFolderPath)
    {
        MapFolders[mapFolderPath.GetKeyFromMapFolderPath()] = mapFolderPath;
        FileManager.CopyDirectory(mapFolderPath, Path.Combine(FileManager.mapFolderCacheFolderPath, Path.GetFileName(mapFolderPath)));
    }
    
    public static void CacheSpotifySearchResults(FullTrack spotifyTrack, Beatmap? beatmap)
    {
        SpotifySearchResults[spotifyTrack.Id] = beatmap;
        string path = Path.Combine(FileManager.spotifySearchCacheFolderPath, spotifyTrack.Id + FileExtensions.SpotifySearchResult);
        File.WriteAllText(path, JsonConvert.SerializeObject(beatmap));
    }
}