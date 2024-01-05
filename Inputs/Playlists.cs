namespace BeatSaberLibraryManager.Inputs;

public static class Playlists
{
	public static readonly Dictionary<string, int> UnfilteredBeatSaverPlaylists = new()
	{
		{ "Favorites", 7903 },
		{ "Aspirational", 7038 },
	};

	public static readonly Dictionary<string, int> FilteredBeatSaverPlaylists = new()
	{
		{ "Skillet", 85217 },
		{ "KDA", 70553 },
		{ "BTS", 70543 },
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
		{ "Epic Soundtracks from Movies and Games", "https://open.spotify.com/playlist/5OsCLnvSitOlJNGaQRBV5R?si=87611a1981ab4537" },
		{ "Monster Cat - Best of All Time", "https://open.spotify.com/playlist/5WMYEm92RFVtcemp3i2rQS?si=522782c0de94435f" },
		{ "Pop Bangers 2000-2023", "https://open.spotify.com/playlist/5nYj43JXdyVx0yAtY5OVFP?si=a6c883a1f02f40ea" },
		{ "Melodic Dubstep", "https://open.spotify.com/playlist/6Qu7co7czjjXwSEuCyCOAD?si=0bf4298744614c20" },
		{ "Pop EDM", "https://open.spotify.com/playlist/6PCw4kIBtHKueRCaJdzU7o?si=eda496d95575471b" },
		{ "Disney Hits", "https://open.spotify.com/playlist/37i9dQZF1DX8C9xQcOrE6T?si=6e3537894b6c4349" },
	};
}