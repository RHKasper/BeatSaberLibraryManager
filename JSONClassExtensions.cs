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
    }
}