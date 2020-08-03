using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ApiExample : MonoBehaviour
{
    public int MaxResultCount = 15;
    public InputField SearchInput;
    public Button SearchButton;
    public LayoutGroup ResultsLayout;
    public RawImage ResultImagePrefab;

    private void Start()
    {
        SearchButton.onClick.AddListener(SendExample);
    }

    void SendExample()
    {
        foreach (Transform child in ResultsLayout.transform)
            Destroy(child.gameObject);
        Resources.UnloadUnusedAssets();

        Debug.Log($"Searching : {SearchInput.text}", this);

        SearchButton.interactable = false;
        Arikan.Duckduckgo.Api.ImagesApi.Search(SearchInput.text, 1, (result) =>
        {
            SearchButton.interactable = true;

            if (result == null)
            {
                Debug.LogError("Result Null", this);
                return;
            }

            Debug.Log(JsonUtility.ToJson(result.results[0], true), this);
            foreach (var item in result.results.Take(MaxResultCount))
            {
                var request = UnityWebRequestTexture.GetTexture(item.thumbnail).SendWebRequest();
                request.completed +=
                (ao) =>
                {
                    if (ao.isDone)
                    {
                        var image = Instantiate(ResultImagePrefab, ResultsLayout.transform);
                        image.texture = DownloadHandlerTexture.GetContent(request.webRequest);
                    }
                };
            }
        });
    }

}
