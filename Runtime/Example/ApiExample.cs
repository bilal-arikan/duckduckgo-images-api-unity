using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApiExample : MonoBehaviour
{
    public InputField SearchInput;
    public Button SearchButton;
    public LayoutGroup ResultsLayout;
    public Image ResultImagePrefab;

    private void Start()
    {
        SearchButton.onClick.AddListener(SendExample);
    }

    void SendExample()
    {
        foreach (Transform child in ResultsLayout.transform)
            Destroy(child.gameObject);

        Debug.Log("Searching", this);

        SearchButton.interactable = false;
        Arikan.Duckduckgo.Api.Images.Search(SearchInput.text, 1, (result) =>
        {
            SearchButton.interactable = true;
            Debug.Log(JsonUtility.ToJson(result.results[0], true), this);
            foreach (var item in result.results.Take(12))
            {
                var image = Instantiate(ResultImagePrefab, ResultsLayout.transform);
            }
        });
    }
}
