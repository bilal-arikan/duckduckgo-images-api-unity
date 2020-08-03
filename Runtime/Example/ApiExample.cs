using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApiExample : MonoBehaviour
{
    public InputField SearchInput;
    public Button SearchButton;
    public LayoutGroup ResultsLayout;
    public string ExampleSearch = "apple";
    public Api api;

    void Start()
    {
        api = new GameObject("API").AddComponent<Api>();
    }

    [ContextMenu("Example")]
    void SendExample()
    {
        Debug.Log("Searching", this);
        api.Search(ExampleSearch, 1, (result) =>
        {
            Debug.Log(JsonUtility.ToJson(result.results[0], true), this);
        });
    }
}
