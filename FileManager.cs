using System.IO.Compression;
using BeatSaverSharp.Models;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager
{
	public static class FileManager
	{
		//public const string MapCachePath = "/Users/robert/Repos/BeatSaberLibraryManager/Cache/MapZips";
		//public const string ImagesCachePath = "/Users/robert/Repos/BeatSaberLibraryManager/Cache/Images";
		//public const string MapsOutputFolderPath = "/Users/robert/Repos/BeatSaberLibraryManager/Outputs/CustomLevels";
		//public const string PlaylistsOutputFolderPath = "/Users/robert/Repos/BeatSaberLibraryManager/Outputs/Playlists";
		
		public const string MapCachePath = @"C:\repos\BeatSaberLibraryManager\Cache\MapCache";
		public const string ImagesCachePath = @"C:\repos\BeatSaberLibraryManager\Cache\Images";
		public const string MapsOutputFolderPath = @"C:\repos\BeatSaberLibraryManager\output\CustomLevels";
		public const string PlaylistsOutputFolderPath = @"C:\repos\BeatSaberLibraryManager\output\Playlists";
		
		public static void UnzipFile(string zipFilePath, out string unzipDir)
		{
			unzipDir = GetMapDirectory(zipFilePath);

			try
			{
				if(Directory.Exists(unzipDir)) 
					Directory.Delete(unzipDir, true);
				ZipFile.ExtractToDirectory(zipFilePath, unzipDir);
			}
			catch(Exception e)
			{
				Console.WriteLine("Failed to extract " + zipFilePath);
				Console.WriteLine(e);
			}
			File.Delete(zipFilePath);
		}


		public static string GetMapDirectory(string zipFilePath)
		{
			string targetDir = Path.Combine(MapsOutputFolderPath, Path.GetFileNameWithoutExtension(zipFilePath));
			return targetDir;
		}

		public static string GetZipFilePath(string zipFileName)
		{
			string zipFilePath = Path.Combine(MapCachePath, zipFileName);
			foreach (char c in Path.GetInvalidPathChars())
				zipFileName = zipFileName.Replace(c + "", "");
			return zipFilePath;
		}
		
		public static string GetZipFilePath(Beatmap beatmap) => GetZipFilePath(GetZipFileName(beatmap));
		
		public static string GetZipFileName(Beatmap beatmap)
		{
			string zipFileName = beatmap.Name + " - " + beatmap.ID + ".zip";
			foreach (char c in Path.GetInvalidFileNameChars())
				zipFileName = zipFileName.Replace(c, ' ');
			return zipFileName;
		}

		public static void PrepareWorkingDirectories()
		{
			if (Directory.Exists(MapsOutputFolderPath))
				Directory.Delete(MapsOutputFolderPath, true);
			Directory.CreateDirectory(MapsOutputFolderPath);

			if (Directory.Exists(PlaylistsOutputFolderPath))
				Directory.Delete(PlaylistsOutputFolderPath, true);
			Directory.CreateDirectory(PlaylistsOutputFolderPath);
			
			if (Directory.Exists(MapCachePath))
				Directory.Delete(MapCachePath, true);
			Directory.CreateDirectory(MapCachePath);
		
			if (Directory.Exists(ImagesCachePath))
				Directory.Delete(ImagesCachePath, true);
			Directory.CreateDirectory(ImagesCachePath);
		}

		static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
		{
			// Get information about the source directory
			var dir = new DirectoryInfo(sourceDir);

			// Check if the source directory exists
			if (!dir.Exists)
				throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

			// Cache directories before we start copying
			DirectoryInfo[] dirs = dir.GetDirectories();

			// Create the destination directory
			Directory.CreateDirectory(destinationDir);

			// Get the files in the source directory and copy to the destination directory
			foreach (FileInfo file in dir.GetFiles())
			{
				string targetFilePath = Path.Combine(destinationDir, file.Name);
				file.CopyTo(targetFilePath);
			}

			// If recursive and copying subdirectories, recursively call this method
			if (recursive)
			{
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}
			
		}

		public static void OutputPlaylists(IEnumerable<BPList> playlists)
		{
			foreach (BPList bpList in playlists)
			{
				string serializedBpList = JsonConvert.SerializeObject(bpList);
				string path = Path.Combine(PlaylistsOutputFolderPath, bpList.playlistTitle + ".bplist");
				File.WriteAllText(path, serializedBpList);
			}
		}
	}
}