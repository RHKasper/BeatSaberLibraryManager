using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.MapEvaluation
{
	public static class MapEvalUtils
	{
		public static double Remap(this double f, double inMin, double inMax, double outMin, double outMax)
		{
			double inPortion = Math.Clamp((f - inMin) / (inMax - inMin), 0, 1);
			return outMin + inPortion * (outMax - outMin);
		}
		public static string FilterToJustAlphaNumerics(string str)
		{
			string result = "";
			foreach (char c in str)
			{
				if (char.IsLetterOrDigit(c))
					result += c;
				else
					result += ' ';
			}

			return result;
		}

		public static int FindWordsInMapName(this Beatmap? beatmap, string[] words) => words.Count(w => beatmap.Name.Contains(w, StringComparison.OrdinalIgnoreCase));
	}
}