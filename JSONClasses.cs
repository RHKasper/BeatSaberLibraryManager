// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

namespace BeatSaberLibraryManager;

public class CustomData
{
    public string syncURL { get; set; }
}

public class SongInfo
{
    public string key { get; set; }
    public string hash { get; set; }
    public string songName { get; set; }

    protected bool Equals(SongInfo other)
    {
        return key == other.key && hash == other.hash && songName == other.songName;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SongInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(key, hash, songName);
    }
}

public class BPList
{
    public string playlistTitle { get; set; }
    public string playlistAuthor { get; set; }
    public string playlistDescription { get; set; }
    public string image { get; set; }
    public CustomData customData { get; set; }
    public List<SongInfo> songs { get; set; }

    public void RemoveDuplicates()
    {
        songs = new HashSet<SongInfo>(songs).ToList();
    }
}




//========= Song Data =================================================================================================

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class MapData
{
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public Uploader uploader { get; set; }
    public Metadata metadata { get; set; }
    public Stats stats { get; set; }
    public DateTime uploaded { get; set; }
    public bool automapper { get; set; }
    public bool ranked { get; set; }
    public bool qualified { get; set; }
    public List<Version> versions { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public DateTime lastPublishedAt { get; set; }
}

//========= Song Data =================================================================================================
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class Curator
{
    public string avatar { get; set; }
    public bool curator { get; set; }
    public string email { get; set; }
    public string hash { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public Stats stats { get; set; }
    public bool testplay { get; set; }
    public string type { get; set; }
    public bool uniqueSet { get; set; }
    public int uploadLimit { get; set; }
    public bool verifiedMapper { get; set; }
}


public class Diff
{
    public const string Easy = "Easy";
    public const string Normal = "Normal";
    public const string Hard = "Hard";
    public const string Expert = "Expert";
    public const string ExpertPlus = "ExpertPlus";

    public int bombs { get; set; }
    public string characteristic { get; set; }
    public bool chroma { get; set; }
    public bool cinema { get; set; }
    public string difficulty { get; set; }
    public int events { get; set; }
    public double length { get; set; }
    public int maxScore { get; set; }
    public bool me { get; set; }
    public bool ne { get; set; }
    public double njs { get; set; }
    public int notes { get; set; }
    public double nps { get; set; }
    public int obstacles { get; set; }
    public double offset { get; set; }
    public ParitySummary paritySummary { get; set; }
    public double seconds { get; set; }
    public double stars { get; set; }
}

public class DiffStats
{
    public int easy { get; set; }
    public int expert { get; set; }
    public int expertPlus { get; set; }
    public int hard { get; set; }
    public int normal { get; set; }
    public int total { get; set; }
}

public class Doc
{
    public bool automapper { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime curatedAt { get; set; }
    public Curator curator { get; set; }
    public DateTime deletedAt { get; set; }
    public string description { get; set; }
    public string id { get; set; }
    public DateTime lastPublishedAt { get; set; }
    public Metadata metadata { get; set; }
    public string name { get; set; }
    public bool qualified { get; set; }
    public bool ranked { get; set; }
    public Stats stats { get; set; }
    public List<string> tags { get; set; }
    public DateTime updatedAt { get; set; }
    public DateTime uploaded { get; set; }
    public Uploader uploader { get; set; }
    public List<Version> versions { get; set; }
}


public class Metadata
{
    public double bpm { get; set; }
    public int duration { get; set; }
    public string levelAuthorName { get; set; }
    public string songAuthorName { get; set; }
    public string songName { get; set; }
    public string songSubName { get; set; }
}

public class ParitySummary
{
    public int errors { get; set; }
    public int resets { get; set; }
    public int warns { get; set; }
}

public class SearchQuery
{
    public List<Doc> docs { get; set; }
    public string redirect { get; set; }
}


public class ScheduledAt
{
    public int epochSeconds { get; set; }
    public int nanosecondsOfSecond { get; set; }
    public DateTime value { get; set; }
}


public class Stats
{
    public double avgBpm { get; set; }
    public double avgDuration { get; set; }
    public double avgScore { get; set; }
    public DiffStats diffStats { get; set; }
    public DateTime firstUpload { get; set; }
    public DateTime lastUpload { get; set; }
    public int rankedMaps { get; set; }
    public int totalDownvotes { get; set; }
    public int totalMaps { get; set; }
    public int totalUpvotes { get; set; }
    public int downloads { get; set; }
    public int downvotes { get; set; }
    public int plays { get; set; }
    public double score { get; set; }
    public double scoreOneDP { get; set; }
    public int upvotes { get; set; }
}

public class Testplay
{
    public DateTime createdAt { get; set; }
    public string feedback { get; set; }
    public DateTime feedbackAt { get; set; }
    public BSUser user { get; set; }
    public string video { get; set; }
}

public class Uploader
{
    public string avatar { get; set; }
    public bool curator { get; set; }
    public string email { get; set; }
    public string hash { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public Stats stats { get; set; }
    public bool testplay { get; set; }
    public string type { get; set; }
    public bool uniqueSet { get; set; }
    public int uploadLimit { get; set; }
    public bool verifiedMapper { get; set; }
}

public class BSUser
{
    public string avatar { get; set; }
    public bool curator { get; set; }
    public string email { get; set; }
    public string hash { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public Stats stats { get; set; }
    public bool testplay { get; set; }
    public string type { get; set; }
    public bool uniqueSet { get; set; }
    public int uploadLimit { get; set; }
    public bool verifiedMapper { get; set; }
}

public class Version
{
    public string coverURL { get; set; }
    public DateTime createdAt { get; set; }
    public List<Diff> diffs { get; set; }
    public string downloadURL { get; set; }
    public string feedback { get; set; }
    public string hash { get; set; }
    public string key { get; set; }
    public string previewURL { get; set; }
    public double sageScore { get; set; }
    public ScheduledAt scheduledAt { get; set; }
    public string state { get; set; }
    public DateTime testplayAt { get; set; }
    public List<Testplay> testplays { get; set; }
}