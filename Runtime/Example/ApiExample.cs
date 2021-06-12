using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Arikan.Duckduckgo.Api;

public class ApiExample : MonoBehaviour
{
    public int ShowMaxResultAmount = 15;
    public InputField SearchInput;
    public Button SearchButton;
    public LayoutGroup ResultsLayout;
    public RawImage ResultImagePrefab;
    [Header("Paging")]
    public Button prevButton;
    public Button nextButton;
    public Text pageNo;
    [Space]
    public int lastPageNo;

    private List<RawImage> resultImages = new List<RawImage>();

    private void Start()
    {
        SearchButton.onClick.AddListener(SendExample);
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
    }

    void SendExample()
    {
        foreach (Transform child in ResultsLayout.transform)
            Destroy(child.gameObject);
        Resources.UnloadUnusedAssets();

        Debug.Log($"Searching : {SearchInput.text}", this);

        SearchButton.interactable = false;
        lastPageNo = 1;
        ClearResults();
        ImagesApi.Search(SearchInput.text, SafeSearch.Off, lastPageNo, OnSearchCallback);
    }

    void NextPage()
    {
        lastPageNo++;
        ClearResults();
        ImagesApi.Search(SearchInput.text, SafeSearch.Off, lastPageNo, OnSearchCallback);
    }
    void PreviousPage()
    {
        lastPageNo--;
        ClearResults();
        ImagesApi.Search(SearchInput.text, SafeSearch.Off, lastPageNo, OnSearchCallback);
    }

    void ClearResults(){
        foreach (var img in resultImages)
        {
            Destroy(img.gameObject);
        }
        resultImages.Clear();
    }

    void OnSearchCallback(ImageSearchResult result)
    {
        SearchButton.interactable = true;
        prevButton.interactable = lastPageNo > 1;
        pageNo.text = lastPageNo.ToString("00");

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
                    var image = Instantiate(ResultImagePrefab, ResultsLayout.transform);
                    image.texture = DownloadHandlerTexture.GetContent(request.webRequest);
                    resultImages.Add(image);
                }
            };
        }
    }
}
