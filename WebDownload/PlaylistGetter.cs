using System.Diagnostics;
using BeatSaberLibraryManager.Inputs;
using BeatSaverSharp;
using BeatSaverSharp.Models.Pages;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.WebDownload;

public static class PlaylistGetter
{
	public static async Task<List<BPList>> GetFilteredBeatSaverBpLists(BeatSaver beatSaverApi)
	{
		List<BPList> bpLists = new List<BPList>();

		//Download beatsaver playlists
		foreach (string playlistName in Playlists.FilteredBeatSaverPlaylists.Keys)
		{
			BPList? bpList = await GetBpListWithCaching(playlistName, () => GetBeatSaverPlaylist(Playlists.FilteredBeatSaverPlaylists[playlistName], beatSaverApi));
			if (bpList != null) 
				bpLists.Add(bpList);
		}
        
		//Download beatsaver mapper playlists
		foreach (string playlistName in Playlists.BeatSaverMapperPlaylists.Keys)
		{
			BPList? bpList = await GetBpListWithCaching(playlistName, () => GetBeatSaverMapperPlaylist(Playlists.BeatSaverMapperPlaylists[playlistName]));
			if (bpList != null)
				bpLists.Add(bpList);
		}

		return bpLists;
	}

	public static async Task<List<BPList>> GetUnfilteredBpLists(BeatSaver beatSaverApi, SpotifyClient spotify)
	{
		List<BPList> bpLists = new List<BPList>();

		//Download beatsaver playlists
		foreach (string playlistName in Playlists.UnfilteredBeatSaverPlaylists.Keys)
		{
			BPList? bpList = await GetBpListWithCaching(playlistName, () => GetBeatSaverPlaylist(Playlists.UnfilteredBeatSaverPlaylists[playlistName], beatSaverApi));
			if (bpList != null)
				bpLists.Add(bpList);
		}

		//Download Spotify playlists
		foreach (string playlistName in Playlists.SpotifyPlaylistUrls.Keys)
		{
			BPList? bpList = await GetBpListWithCaching(playlistName, () => SpotifyPlaylistConverter.GenerateBeatSaberPlaylist(Playlists.SpotifyPlaylistUrls[playlistName], beatSaverApi, spotify));
			if (bpList != null)
				bpLists.Add(bpList);
		}

		return bpLists;
	}

	private static async Task<BPList?> GetBeatSaverPlaylist(int id, BeatSaver beatSaverApi)
	{
		Task<PlaylistDetail?> playlistDetailDownload = beatSaverApi.Playlist(id);
		await playlistDetailDownload;

		if (playlistDetailDownload.Result == null)
		{
			Console.WriteLine("ERROR: Failed to download beatsaber playlist id: " + id);
			return null;
		}

		Task<BPList?> playlistDownload = playlistDetailDownload.Result.GetBPList();
		await playlistDownload;
		
		if (playlistDownload.Result == null)
		{
			Console.WriteLine("ERROR: Failed to download beatsaber playlist id: " + id + " -- " + playlistDownload.Exception);
			return null;
		}
		
		return playlistDownload.Result;
	}

	private static async Task<BPList?> GetBeatSaverMapperPlaylist(string url)
	{
		string? message = await DownloadUtil.Get(url);
		return message != null ? JsonConvert.DeserializeObject<BPList>(message) : null;
	}

	private static async Task<BPList?> GetBpListWithCaching(string playlistName, Func<Task<BPList?>> downloadFunction)
	{
		BPList? bpList = null;
		if (Cache.PreFilterBpLists.ContainsKey(playlistName))
		{
			bpList = Cache.PreFilterBpLists[playlistName];
			Console.WriteLine("Retrieved playlist from cache: " + bpList.playlistTitle);
		}
		else
		{
			bpList = await downloadFunction.Invoke();
			if (bpList != null)
			{
				Cache.CacheBpListPreFilter(playlistName, bpList);
				Console.WriteLine("Downloaded playlist: " + bpList.playlistTitle);
			}
		}

		return bpList;
	}
}