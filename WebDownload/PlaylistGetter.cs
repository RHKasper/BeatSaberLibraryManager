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
		foreach (int id in Playlists.FilteredBeatSaverPlaylists.Values)
		{
			BPList? bpList = await GetBeatSaverPlaylist(id, beatSaverApi);
			if (bpList != null)
			{
				bpLists.Add(bpList);
				Console.WriteLine("Downloaded playlist: " + bpList.playlistTitle);
			}
		}
        
		//Download beatsaver mapper playlists
		foreach (string url in Playlists.BeatSaverMapperPlaylists.Values)
		{
			BPList? bpList = await GetBeatSaverMapperPlaylist(url);
			if (bpList != null)
			{
				bpLists.Add(bpList);
				Console.WriteLine("Downloaded playlist: " + bpList.playlistTitle);
			}
		}

		return bpLists;
	}

	public static async Task<List<BPList>> GetUnfilteredBpLists(BeatSaver beatSaverApi)
	{
		List<Task<BPList?>> tasks = new();
        
		SpotifyClient spotify = await CreateSpotifyClient();
		foreach (string spotifyPlaylistUrl in Playlists.SpotifyPlaylistUrls.Values)
		{
			Task<BPList?> t = SpotifyPlaylistConverter.GenerateBeatSaberPlaylist(spotifyPlaylistUrl, beatSaverApi, spotify);
			tasks.Add(t);

		}
		
		foreach (int id in Playlists.UnfilteredBeatSaverPlaylists.Values)
		{
			Task<BPList?> t = GetBeatSaverPlaylist(id, beatSaverApi);
			tasks.Add(t);
		}

		await tasks.AwaitAll();
		return tasks.Where(t => t.Result != null).Cast<Task<BPList>>().Select(task => task.Result).ToList();
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
	
	// todo: don't store credentials in repo
	private static async Task<SpotifyClient> CreateSpotifyClient()
	{
		var config = SpotifyClientConfig.CreateDefault();
		var request = new ClientCredentialsRequest("1d303e20e9c8498f95c8d39f244b143d", "45648a39b896462594915ed2d7d48714");
		var response = await new OAuthClient(config).RequestToken(request);
		var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
		return spotify;
	}
}