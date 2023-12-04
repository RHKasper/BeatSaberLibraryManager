namespace BeatSaberLibraryManager;

public class ImageEncoder
{
	public static string Base64Encode(string path) 
	{
		byte[] imageArray = System.IO.File.ReadAllBytes(path);
		string base64ImageRepresentation = Convert.ToBase64String(imageArray);
		return base64ImageRepresentation;
	}
}