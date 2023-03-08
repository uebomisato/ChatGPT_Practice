using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class StreamingAssetsLoader
{
    public static IEnumerator LoadTextFile(string filename, System.Action<string> onComplete)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath + "/Settings/", filename);

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string result = www.downloadHandler.text;
                onComplete?.Invoke(result);
            }
        }
        else
        {
            string result = File.ReadAllText(filePath);
            onComplete?.Invoke(result);
        }
    }
}
