using BeatSaberLibraryManager.MapEvaluation;
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
	public static async Task<Playlist?> GetBeatSaverPlaylist(int id, BeatSaver beatSaverApi, bool filter = true)
	{
		Task<PlaylistDetail?> playlistDetailDownload = beatSaverApi.Playlist(id);
		await playlistDetailDownload;

		if (playlistDetailDownload.Result == null)
		{
			return null;
		}

		if (filter)
		{
			var filteredMapsList = playlistDetailDownload.Result.Beatmaps.Where(map => !map.FailsAnyQualityFilter());
		}
		
		foreach (var beatMap in playlistDetailDownload.Result.Beatmaps)
		{
			Console.WriteLine(beatMap.Map.Name + ": " + beatMap.Map.ID);
		}



		return playlistDetailDownload.Result.Playlist;
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