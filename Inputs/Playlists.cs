namespace BeatSaberLibraryManager.Inputs;

public class Playlists
{
	public static readonly Dictionary<string, int> UnfilteredBeatSaverPlaylists = new()
	{
		{ "Favorites", 7903 },
		{ "Aspirational", 7038 },
	};

	public static readonly Dictionary<string, int> FilteredBeatSaverPlaylists = new()
	{
		{ "Ajr NeoTheater", 171573 },
		{ "Skillet", 85217 },
		{ "Linkin Park", 12144 },
		{ "KDA", 70553 },
		{ "BTS", 70543 },
		{ "Alphabeat", 8441 }
	};

	public static readonly Dictionary<string, string> BeatSaverMapperPlaylists = new()
	{
		{ "Teuflum", "https://api.beatsaver.com/users/id/68740/playlist/beatsaver-user-68740.bplist" },
		{ "TheCzar1994", "https://api.beatsaver.com/users/id/4285984/playlist/beatsaver-user-4285984.bplist" },
		{ "NixieKorten - Electroswing mapper ", "https://api.beatsaver.com/users/id/4286374/playlist/beatsaver-user-4286374.bplist" },
		{ "Revelate", "https://api.beatsaver.com/users/id/2768/playlist/beatsaver-user-2768.bplist" },
	};

	public static readonly Dictionary<string, string> SpotifyPlaylistUrls = new()
	{
		{ "Robert's Epic Mix", "https://open.spotify.com/playlist/6QTGBDOzjxnkfQ9X6OmWIY?si=9d82829900c84462" },
		//{ "Robert's Pop Mix", "https://open.spotify.com/playlist/7vSZ2b4591qCZhi8bn2xkn?si=4a570e22013747fa" },
		//{ "Robert's Edm Mix", "https://open.spotify.com/playlist/5tY88XWKaUArUISNkY697j?si=3afb235dac594189" },
		//{ "Robert's Rock Mix", "https://open.spotify.com/playlist/43jMBPZVR5cdD7Cw1gZF8j?si=9a990d1647eb4cde" },
		//{ "All Likes", "https://open.spotify.com/playlist/5Zi1NzMK91ImLODGEWHNqS?si=bcde847d6766403f" },
	};
}