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
		
		if (playlistDownload.Result == null)
		{
			Console.WriteLine("ERROR: Failed to download beatsaber playlist id: " + id + " -- " + playlistDownload.Exception);
			return null;
		}
		
		return playlistDownload.Result;
	}
	
	public static async Task<BPList?> GetBeatSaverMapperPlaylist(string url)
	{
		string? message = await DownloadUtil.Get(url);
		return message != null ? JsonConvert.DeserializeObject<BPList>(message) : null;
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