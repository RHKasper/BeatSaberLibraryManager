using System.Diagnostics;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaberLibraryManager.Outputs;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using BeatSaverSharp.Models.Pages;
using Newtonsoft.Json;
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
                        string query = fullTrack.Name + " " + fullTrack.Artists.First().Name;
                        Console.WriteLine("Searching for: \"" + query + "\"");
                        SearchTextFilterOption searchFilter = new SearchTextFilterOption
                        {
                            MaxNPS = MapFilters.MaxNps,
                            MinRating = MapFilters.MinScoreForManyRatings,
                            Query = query,
                            SortOrder = SortingOptions.Relevance,
                        };
                        Page? searchResults = await beatSaverApi.SearchBeatmaps(searchFilter);

                        if (searchResults is { Empty: false })
                        {
                            Beatmap? bestMap = GetBestMap(fullTrack, searchResults);
                            if (bestMap != null)
                            {
                                Console.WriteLine($"Track Found For: \"{fullTrack.Name} ({string.Join(", ", fullTrack.Artists.Select(a => a.Name))})\":");
                                Console.WriteLine($"\t{bestMap.Name}");

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

            string dir = FileManager.ImagesCachePath;
            string filename = playlist.Name + " Cover";

            // Request Image		
            return await DownloadUtil.DownloadImageAsync(dir, filename, new Uri(imageUrl));
        }

        public static Beatmap? GetBestMap(FullTrack fullTrack, Page searchResults)
        {
            List<Beatmap> acceptableOptions = new List<Beatmap>();
            for (int i = searchResults.Beatmaps.Count - 1; i >= 0; i--)
            {
                Beatmap beatmap = searchResults.Beatmaps[i];
                BeatmapVersion version = beatmap.LatestVersion;

                if (!beatmap.FailsAnyQualityFilter() &&
                    version.HasAnyOfRequestedDifficulties(MapFilters.AcceptableSpotifyDifficulties) &&
                    beatmap.Name.Contains(fullTrack.Artists.First().Name))
                {
                    acceptableOptions.Add(beatmap);
                }
            }

            return acceptableOptions.Most(beatmap => beatmap.ScoreOverall(fullTrack));
        }
    }
}