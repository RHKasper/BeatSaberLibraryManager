using System.IO.Compression;
using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.Outputs
{
	public static class FileManager
	{
		public const string MapCachePath = "C:\\repos\\BeatSaberLibraryManager\\Cache\\MapCache";
		public const string ImagesCachePath = "C:\\repos\\BeatSaberLibraryManager\\Cache\\Images";
		
		public const string MapsOutputFolderPath = "C:\\repos\\BeatSaberLibraryManager\\output\\CustomLevels";
		public const string PlaylistsOutputFolderPath = "C:\\repos\\BeatSaberLibraryManager\\output\\Playlists";
		
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


		public static string GetMapDirectory(Beatmap beatmap) => GetMapDirectory(GetZipFilePath(GetZipFileName(beatmap)));

		public static string GetMapDirectory(string zipFilePath)
		{
			string targetDir = Path.Combine(MapCachePath, Path.GetFileNameWithoutExtension(zipFilePath));
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
		

		public static void PrepareOutputDirectories()
		{
			if (Directory.Exists(MapsOutputFolderPath))
				Directory.Delete(MapsOutputFolderPath, true);
			Directory.CreateDirectory(MapsOutputFolderPath);

			if (Directory.Exists(PlaylistsOutputFolderPath))
				Directory.Delete(PlaylistsOutputFolderPath, true);
			Directory.CreateDirectory(PlaylistsOutputFolderPath);
		}
		
		public static void PrepareMapZipCacheDirectory()
		{
			if (Directory.Exists(MapCachePath))
				Directory.Delete(MapCachePath, true);
			Directory.CreateDirectory(MapCachePath);
		}
		
		
		public static void ClearImagesCache()
		{
			if (Directory.Exists(ImagesCachePath))
				Directory.Delete(ImagesCachePath, true);
			Directory.CreateDirectory(ImagesCachePath);
		}
		
		public static void ExportMaps(IEnumerable<string> mapFolderPaths)
		{
			foreach (string mapFolderPath in mapFolderPaths)
			{
				if (Directory.Exists(mapFolderPath))
				{
					string filename = Path.GetFileName(mapFolderPath);
					string target = Path.Combine(MapsOutputFolderPath, filename);
					Console.WriteLine($"Copying {mapFolderPath} to {target}");
					CopyDirectory(mapFolderPath, target);
				}
				else
					Console.WriteLine($"Map Directory {mapFolderPath} doesn't exist");
			}
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
	}
}