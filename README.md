# duckduckgo-images-api-unity

Unity3d wrapper for duckduckgo-images-api

### Screenshot

![Alt text](SS~/SS.gif?raw=true "Example Gif")

How to Import:
 - Add to manifest.json
```
"com.arikan.duckduckgo.images" : "https://github.com/bilal-arikan/duckduckgo-images-api-unity.git",
```

How to Use:

```C#
DuckDuckGo.Search(
    "banana",           // search input 
    SafeSearch.Off,     // safesearch
    0,                  // page 
    "us-en",            // locations
    OnSearchCallback    // callback
);  

void OnSearchCallback(ImageSearchResult result)
{
    if (result == null)
    {
        Debug.LogError("Result Null", this);
        return;
    }

    Debug.Log($"Result Count: " + result.results.Count, this);
    Debug.Log("First Result:\n" + JsonUtility.ToJson(result.results[0], true), this);
    foreach (var item in result.results.Take(ShowMaxResultAmount))
    {
        var request = UnityWebRequestTexture.GetTexture(item.thumbnail).SendWebRequest();
        request.completed +=
        (ao) =>
        {
            if (ao.isDone)
            {
                RawImage image = Instantiate(ResultImagePrefab, ResultsLayout.transform);
                image.texture = DownloadHandlerTexture.GetContent(request.webRequest);
            }
        };
    }
}
```