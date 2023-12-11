using System.Diagnostics;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaberLibraryManager.Outputs;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using BeatSaverSharp.Models.Pages;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager;

/// <summary>
/// Todo: figure out what to name this file.
/// </summary>
public static class HighLevelTasks
{
	public static async Task<BPList?> GetBeatSaverPlaylist(int id, BeatSaver beatSaverApi)
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
		return playlistDownload.Result;
	}
	
	// todo: figure out how paging is supposed to work :(
	public static async Task<BPList?> GetBeatSaverMapperPlaylist(int userId, BeatSaver beatSaverApi)
	{
		Task<User?> userDownload = beatSaverApi.User(userId);
		await userDownload;

		if (userDownload.Result == null)
		{
			Console.WriteLine("ERROR: Failed to download info for user id: " + userId);
			return null;
		}

		List<Beatmap> maps = new List<Beatmap>();
		Task<Page?> beatmapsDownload = userDownload.Result.Beatmaps();
		await beatmapsDownload;

		while (!beatmapsDownload.Result.Empty)
		{
			maps.AddRange(beatmapsDownload.Result.Beatmaps);
			beatmapsDownload = beatmapsDownload.Result.Next();
			await beatmapsDownload;
		}

		return new BPList
		{
			playlistTitle = "Maps by " + userDownload.Result.Name,
			playlistAuthor = userDownload.Result.Name,
			songs = maps.Select(m => new SongInfo
			{
				songName = m.Name,
				hash = m.LatestVersion.Hash,
				key = m.LatestVersion.Key,
			}).ToList(),
		};
	}
	
	public static async Task<BPList?> GetWebBpList(string url)
	{
		Task<string?> bpListStringDownload = DownloadUtil.Get(url);
		await bpListStringDownload;
		if (bpListStringDownload.Result == null)
		{
			Console.WriteLine("ERROR: Failed to download playlist at " + url);
			return null;
		}

		return JsonConvert.DeserializeObject<BPList>(bpListStringDownload.Result);
	}

	// public static async Task<BPList> GenerateBpListFromSpotifyPlaylistAsync(string spotifyPlaylistUrl)
	// {
	// 	Console.WriteLine("Starting " + nameof(GenerateBpListFromSpotifyPlaylistAsync));
	// 	await Task.Delay(1000);
	// 	Console.WriteLine("Finished " + nameof(GenerateBpListFromSpotifyPlaylistAsync));
	// 	return new BPList();
	// }



	// public static async Task<T[]> FilterAsync<T>(IEnumerable<T> sourceEnumerable, Func<T, Task<bool>> predicateAsync)
	// {
	// 	return (await Task.WhenAll(
	// 			sourceEnumerable.Select(
	// 				v => predicateAsync(v)
	// 					.ContinueWith(task => new { Predicate = task.Result, Value = v })))
	// 		).Where(a => a.Predicate).Select(a => a.Value).ToArray();
	// }
}