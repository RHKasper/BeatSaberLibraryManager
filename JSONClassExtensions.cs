using BeatSaverSharp.Models.Pages;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager
{
    public static class JSONClassExtensions
    {
        public static Version GetLatestVersion(this MapData map)
        {
            Version latest = null;
            foreach (Version version in map.versions)
            {
                if (latest == null || version.createdAt > latest.createdAt)
                    latest = version;
            }

            return latest;
        }
        
        public static Version GetLatestVersion(this Doc doc)
        {
            Version latest = null;
            foreach (Version version in doc.versions)
            {
                if (latest == null || version.createdAt > latest.createdAt)
                    latest = version;
            }

            return latest;
        }

        public static async Task<BPList?> GetBPList(this PlaylistDetail playlistDetail)
        {
            Task<byte[]?> playlistDownload = playlistDetail.Playlist.DownloadPlaylist();
            await playlistDownload;

            int retryCount = 0;
            while (playlistDownload.Result == null)
            {
                if (retryCount < 5)
                {
                    retryCount++;
                    Console.WriteLine("ERROR: Failed to get byte[] for beatsaber playlist: " + playlistDetail.Playlist.Name + ". Retry #" + retryCount);
                    playlistDownload = playlistDetail.Playlist.DownloadPlaylist();
                    await playlistDownload;
                }
                else
                {
                    Console.WriteLine("ERROR: Failed to get byte[] for beatsaber playlist: " + playlistDetail.Playlist.Name + ". Retry limit exceeded");
                    break;
                }
            }

            return JsonConvert.DeserializeObject<BPList>(System.Text.Encoding.Default.GetString(playlistDownload.Result));
        }
    }
}