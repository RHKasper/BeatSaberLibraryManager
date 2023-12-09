namespace BeatSaberLibraryManager.Inputs;

public class Playlists
{
	public static readonly Dictionary<string, int> BeatSaverMapperPlaylists = new()
	{
		{ "Teuflum", 68740 },
		{ "TheCzar1994", 4285984 },
		{ "NixieKorten - Electroswing mapper ", 4286374 },
		{ "Revelate", 2768 },
	};

	public static readonly Dictionary<string, int> BeatSaverPlaylists = new()
	{
		{ "Favorites", 7903 },
		{ "Aspirational", 7038 },
		{ "Ajr NeoTheater", 171573 },
		{ "Skillet", 85217 },
		{ "Linkin Park", 12144 },
		{ "KDA", 70553 },
		{ "BTS", 70543 },
	};

	public static readonly Dictionary<string, string> BeastSaberPlaylists = new()
	{
		{ "Alphabeat PixelTerror", "https://bsaber.com/PlaylistAPI/21-07-01_pixel-terror-pack_alphabeat.bplist" },
	};

	public static readonly Dictionary<string, string> SpotifyPlaylistUrls = new()
	{
		{ "Robert's Epic Mix", "https://open.spotify.com/playlist/6QTGBDOzjxnkfQ9X6OmWIY?si=9d82829900c84462" },
		{ "Robert's Pop Mix", "https://open.spotify.com/playlist/7vSZ2b4591qCZhi8bn2xkn?si=4a570e22013747fa" },
		{ "Robert's Edm Mix", "https://open.spotify.com/playlist/5tY88XWKaUArUISNkY697j?si=3afb235dac594189" },
		{ "Robert's Rock Mix", "https://open.spotify.com/playlist/43jMBPZVR5cdD7Cw1gZF8j?si=9a990d1647eb4cde" },
		{ "All Likes", "https://open.spotify.com/playlist/5Zi1NzMK91ImLODGEWHNqS?si=bcde847d6766403f" },
	};

	private static string GetBeatSaverPlaylistUrl(int playlistId) =>
		"https://api.beatsaver.com/playlists/id/" + playlistId + "/download";

	private static string GetBeatSaverMapperPlaylistUrl(int mapperId) =>
		"https://api.beatsaver.com/users/id/" + mapperId + "/playlist";
}