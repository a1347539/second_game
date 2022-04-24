using UnityEngine;
using Newtonsoft.Json;

public class ResourcesLoader {

    private const string pathPrefix = "Assets/Resources/";

    public static T Load <T> (string prefix, string fileName) {
        string path = prefix + "/" + fileName;
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (textAsset != null) {
            return JsonConvert.DeserializeObject<T>(textAsset.ToString());
        } 
        return default(T);
    }
}
