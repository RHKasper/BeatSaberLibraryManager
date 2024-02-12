namespace BeatSaberLibraryManager.Inputs;

public static class Playlists
{
	public static readonly Dictionary<string, int> UnfilteredBeatSaverPlaylists = new()
	{
		{ "Favorites", 7903 },
		{ "Aspirational", 7038 },
		{ "B&R Hearts", 309514}
	};

	public static readonly Dictionary<string, int> FilteredBeatSaverPlaylists = new()
	{
		{ "Skillet", 85217 },
		{ "Alphabeat", 8441 },
		{ "Curator's Picks Modcharts", 173546 },
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
		{ "Robert's Liked Spotify Songs", "https://open.spotify.com/playlist/5Zi1NzMK91ImLODGEWHNqS?si=bcde847d6766403f" },
		{ "Top EDM Hits of the 2010s", "https://open.spotify.com/playlist/2vHE1sFqVHx3lYwBFyFZcY?si=cb06b0ca293a4847" },
		{ "2000s Emo & Pop Punk", "https://open.spotify.com/playlist/52DEJKt18LNbFy46ldKkfY?si=595b9e88464b4dc8" },
		{ "Epic Fantasy Music", "https://open.spotify.com/playlist/0K6f2rZ5xdOZwpPe1KQmpZ?si=c028f72eeb3f410e" },
		{ "Pop Bangers 2000-2023", "https://open.spotify.com/playlist/5nYj43JXdyVx0yAtY5OVFP?si=a6c883a1f02f40ea" },
		{ "Melodic Dubstep", "https://open.spotify.com/playlist/6Qu7co7czjjXwSEuCyCOAD?si=0bf4298744614c20" },
		{ "Pop EDM", "https://open.spotify.com/playlist/6PCw4kIBtHKueRCaJdzU7o?si=eda496d95575471b" },
		{ "Best Modern Folk Music", "https://open.spotify.com/playlist/7vlFZEU9lXqmDXNxwrv7xD?si=d042fa51603d4aa0"}
	};
}