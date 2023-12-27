using System.Diagnostics;
using System.IO.Compression;
using BeatSaverSharp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager
{
	public static class FileManager
	{
		
		private static string outputDirectory { get; }
		public static string mapZipTempFolderPath {get; }
		public static string imagesTempFolderPath {get; }
		public static string mapsOutputFolderPath {get; }
		public static string playlistsOutputFolderPath {get; }


		#region Cache Paths
		public static string cacheFolderPath { get; }
		public static string preFilterBpListsCacheFolderPath {get; }
		public static string beatmapsCacheFolderPath {get; }
		public static string mapZipCacheFolderPath {get; }

		#endregion

		static FileManager()
		{
			var appConfig = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
			string? outputDirFromAppConfig = appConfig["OutputDirectory"];
			Debug.Assert(outputDirFromAppConfig != null, " \"OutputDirectory\" must be set in the secrets.json file");
			outputDirectory = outputDirFromAppConfig;

			mapZipTempFolderPath = Path.Combine(outputDirectory, "Temp", "MapZips");
			imagesTempFolderPath = Path.Combine(outputDirectory, "Temp", "Images");
			
			cacheFolderPath = Path.Combine(outputDirectory, "Cache");
			preFilterBpListsCacheFolderPath = Path.Combine(cacheFolderPath, "PreFilterBpLists");
			beatmapsCacheFolderPath = Path.Combine(cacheFolderPath, "Beatmaps");
			mapZipCacheFolderPath = Path.Combine(cacheFolderPath, "MapZips");

			mapsOutputFolderPath = Path.Combine(outputDirectory, "CustomLevels");
			playlistsOutputFolderPath = Path.Combine(outputDirectory, "Playlists");
			
			PrepareWorkingDirectories();
			Console.WriteLine("Initialized FileManager");
		}

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
			string targetDir = Path.Combine(mapsOutputFolderPath, Path.GetFileNameWithoutExtension(zipFilePath));
			return targetDir;
		}

		public static string GetZipFilePath(string zipFileName)
		{
			string zipFilePath = Path.Combine(mapZipTempFolderPath, zipFileName);
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
			EnsureDirectoryExists(outputDirectory);
			
			EnsureDirectoryExists(cacheFolderPath);
			EnsureDirectoryExists(preFilterBpListsCacheFolderPath);
			EnsureDirectoryExists(beatmapsCacheFolderPath);
			EnsureDirectoryExists(mapZipCacheFolderPath);

			ClearOrCreateDirectory(mapsOutputFolderPath);
			ClearOrCreateDirectory(playlistsOutputFolderPath);
			ClearOrCreateDirectory(mapZipTempFolderPath);
			ClearOrCreateDirectory(imagesTempFolderPath);
		}

		public static void OutputPlaylists(IEnumerable<BPList> playlists)
		{
			foreach (BPList bpList in playlists)
			{
				string serializedBpList = JsonConvert.SerializeObject(bpList);
				string path = Path.Combine(playlistsOutputFolderPath, bpList.playlistTitle + ".bplist");
				File.WriteAllText(path, serializedBpList);
			}
		}

		private static void ClearOrCreateDirectory(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);
			Directory.CreateDirectory(path);
		}

		private static void EnsureDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		public static Dictionary<string,Beatmap> GetCachedBeatmaps()
		{
			Dictionary<string, Beatmap> beatmaps = new Dictionary<string, Beatmap>();

			foreach (string filePath in Directory.GetFiles(beatmapsCacheFolderPath))
			{
				Beatmap? beatmap = JsonConvert.DeserializeObject<Beatmap>(File.ReadAllText(filePath));
				Debug.Assert(beatmap != null, nameof(beatmap) + " != null");
				beatmaps.Add(Path.GetFileNameWithoutExtension(filePath), beatmap);	
			}

			return beatmaps;
		}
		
		public static Dictionary<string, BPList> GetCachedPreFilterBpLists()
		{
			Dictionary<string, BPList> cachedBpLists = new Dictionary<string, BPList>();
			
			foreach (string filePath in Directory.GetFiles(preFilterBpListsCacheFolderPath))
			{
				var bpList = JsonConvert.DeserializeObject<BPList>(File.ReadAllText(filePath));
				Debug.Assert(bpList != null, nameof(bpList) + " != null");
				cachedBpLists.Add(Path.GetFileNameWithoutExtension(filePath), bpList);
			}

			return cachedBpLists;
		}
	}
}