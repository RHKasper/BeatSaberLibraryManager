using System.Diagnostics;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using BeatSaverSharp.Models.Pages;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.WebDownload
{
    public static class SpotifyPlaylistConverter
    {
        public static async Task<BPList?> GenerateBeatSaberPlaylist(string spotifyPlaylistUrl, BeatSaver beatSaverApi,
            SpotifyClient spotify)
        {
            try
            {
                // Get Playlist from Spotify API
                string playlistId = GetPlaylistIdFromUrl(spotifyPlaylistUrl);
                FullPlaylist playlist = await spotify.Playlists.Get(playlistId);
                Console.WriteLine($"Found Spotify Playlist: {playlist.Name}");

                // Get Playlist image from URL
                string playlistImagePath = await DownloadPlaylistImage(playlist);
                string base64Image = ImageEncoder.Base64Encode(playlistImagePath);
                string base64ImagePath = Path.ChangeExtension(playlistImagePath, ".txt");
                await File.WriteAllTextAsync(base64ImagePath, base64Image);

                // Generate Beatsaber BPList 
                BPList bpList = new BPList
                {
                    playlistTitle = playlist.Name ?? "Unknown Playlist",
                    playlistAuthor = "Spotify",
                    playlistDescription = playlist.Description ?? string.Empty,
                    songs = new List<SongInfo>(),
                    image = base64Image,
                };

                // Get the first page in the spotify playlist
                Paging<PlaylistTrack<IPlayableItem>> page = await spotify.Playlists.GetItems(playlistId);
                int requestedTracks = 0;
                int foundTracks = 0;

                // Foreach page in the spotify playlist
                await foreach (var track in spotify.Paginate(page))
                {
                    // If this is a track (not a podcast episode)
                    if (track.Track is FullTrack fullTrack)
                    {
                        requestedTracks++;
                        Beatmap? bestMap = await GetBestMap(fullTrack, beatSaverApi);
                        
                        if (bestMap != null)
                        {
                            BeatmapVersion version = bestMap.LatestVersion;
                            bpList.songs.Add(new SongInfo
                            {
                                hash = version.Hash,
                                key = version.Key ?? String.Empty,
                                songName = bestMap.Name,
                            });
                            foundTracks++;
                        }
                    }
                }

                bpList.RemoveDuplicates();
                LogSearchResults(playlist.Name, foundTracks, requestedTracks);
                return bpList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static void LogSearchResults(string playlistName, int songsFound, int songsSearchedFor)
        {
            Console.WriteLine(
                $"\n{playlistName}: Found {songsFound} out of {songsSearchedFor} ({((float)songsFound / songsSearchedFor) * 100}%)\n");
        }

        private static string GetPlaylistIdFromUrl(string url) => Path.GetFileName(url).Split('?')[0];

        private static async Task<string> DownloadPlaylistImage(FullPlaylist playlist)
        {
            // Get image URL
            string imageUrl = playlist.Images.First().Url;

            string dir = FileManager.imagesTempFolderPath;
            string filename = playlist.Name + " Cover";

            // Request Image		
            return await DownloadUtil.DownloadImageAsync(dir, filename, new Uri(imageUrl));
        }

        public static async Task<Beatmap?> GetBestMap(FullTrack fullTrack, BeatSaver beatSaverApi)
        {
            Beatmap? selectedMap = null;
            
            Console.WriteLine("Requesting map for: \"" + fullTrack.Name + " " + fullTrack.Artists.First().Name + "\"");

            if (Cache.SpotifySearchResults.ContainsKey(fullTrack.Id))
            {
                selectedMap = Cache.SpotifySearchResults[fullTrack.Id];
                Console.WriteLine("\tFound cached Spotify => BeatSaver search result: " + fullTrack.Name + " => " + (selectedMap == null ? "NULL" : selectedMap.Name));
            }
            else
            {
                Page? searchResults = await SearchBeatSaverForTrack(fullTrack, beatSaverApi);

                if (searchResults != null)
                {
                    List<Beatmap?> acceptableOptions = new List<Beatmap?>();
                    for (int i = searchResults.Beatmaps.Count - 1; i >= 0; i--)
                    {
                        Beatmap? beatmap = searchResults.Beatmaps[i];
                        BeatmapVersion version = beatmap.LatestVersion;

                        if (!beatmap.FailsAnyQualityFilter() &&
                            version.HasAnyOfRequestedDifficulties(MapFilters.AcceptableSpotifyDifficulties) &&
                            beatmap.Name.Contains(fullTrack.Artists.First().Name))
                        {
                            acceptableOptions.Add(beatmap);
                        }
                    }

                    selectedMap = acceptableOptions.Most(beatmap => beatmap.ScoreOverall(fullTrack));
                }
                Console.WriteLine("\tDownloaded Spotify => BeatSaver search result: " + fullTrack.Name + " => " + (selectedMap == null ? "NULL" : selectedMap.Name));
            }

            Cache.CacheSpotifySearchResults(fullTrack, selectedMap);
            return selectedMap;
        }

        private static async Task<Page?> SearchBeatSaverForTrack(FullTrack fullTrack, BeatSaver beatSaverApi)
        {
            string query = fullTrack.Name + " " + fullTrack.Artists.First().Name;
            SearchTextFilterOption searchFilter = new SearchTextFilterOption
            {
                MaxNPS = MapFilters.MaxNps,
                MinRating = MapFilters.MinScoreForManyRatings,
                Query = query,
                SortOrder = SortingOptions.Relevance,
            };

            Page? searchResults = await beatSaverApi.SearchBeatmaps(searchFilter);
            return searchResults;
        }

        public static async Task<SpotifyClient> CreateSpotifyClient()
        {
            var config = SpotifyClientConfig.CreateDefault();
            var credentials = GetSpotifyCredentials();
            var request = new ClientCredentialsRequest(credentials.clientId, credentials.clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            return spotify;
        }

        private static (string clientId, string clientSecret) GetSpotifyCredentials()
        {
            var appConfig = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? clientId =  appConfig["SpotifyClientId"];
            string? clientSecret = appConfig["SpotifyClientSecret"];

            Debug.Assert(clientId != null, " \"SpotifyClientId\" must be set in the secrets.json file");
            Debug.Assert(clientSecret != null, " \"SpotifyClientSecret\" must be set in the secrets.json file");

            return (clientId, clientSecret);
        }
    }
}