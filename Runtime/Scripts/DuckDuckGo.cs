using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Arikan
{
    using Arikan.Models;

    public enum SafeSearch
    {
        // Strict = 1, // Not Available yet
        Moderate = -1,
        Off = -2
    }

    public class DuckDuckGo : MonoBehaviour
    {
        private static DuckDuckGo instance;

        private const string url = "https://duckduckgo.com/";
        private KeyValuePair<string, string> lastSearch; // keyword : token
        private bool isLastRequestSuccessfull;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>(){
            {"authority", "duckduckgo.com"},
            {"accept", "application/json, text/javascript, */*; q=0.01"},
            {"x-requested-with", "XMLHttpRequest"},
            {"user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36"},
            {"referer", "https://duckduckgo.com/"},
            {"accept-language", "en-US,en;q=0.9"},
        };


        public static void Search(string text, int pageNo, Action<ImageSearchResult> onCompleted)
            => Search(text, SafeSearch.Moderate, pageNo, "us-en", onCompleted);
        public static void Search(string text, SafeSearch safeSearch, int pageNo, string location, Action<ImageSearchResult> onCompleted)
        {
            if (!instance)
            {
                instance = new GameObject("DuckDuckGoAPI").AddComponent<Arikan.DuckDuckGo>();
            }
            if (string.IsNullOrWhiteSpace(location))
            {
                location = "us-en";
            }
            instance.StartCoroutine(instance.SearchCoRo(text, safeSearch, pageNo, location, onCompleted));
        }


        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void SearchFromInstance(string text, SafeSearch safeSearch, int pageNo, string location, Action<ImageSearchResult> onCompleted)
        private IEnumerator SearchCoRo(string keyword, SafeSearch safeSearch, int pageNo, string location, Action<ImageSearchResult> onCompleted)
        {
            string token = lastSearch.Value;
            if (lastSearch.Key != keyword)
            {
                // Debug.Log("Requesting new Token...");
                yield return RequestToken(keyword, safeSearch, (t) => token = t);
                lastSearch = new KeyValuePair<string, string>(keyword, token);
            }

            // Debug.Log("SrcOb:" + currentToken);
            yield return RequestSearchResult(keyword, token, safeSearch, pageNo, location, onCompleted);
        }

        private IEnumerator RequestToken(string keyword, SafeSearch safeSearch, Action<string> tokenCallback)
        {
            var requestUrl = $"{url}?q={keyword}";
            // Debug.Log(requestUrl);
            var ao = UnityWebRequest.Get(requestUrl).SendWebRequest();

            yield return new WaitUntil(() => ao.isDone);
            if (ao.webRequest.isNetworkError || ao.webRequest.isHttpError)
            {
                Debug.LogError("Requesting Failed ! " + ao.webRequest.error);
                tokenCallback.Invoke(null);
                yield break;
            }

            Regex regex = new Regex(@"vqd=([\d-]+)\&");
            var match = regex.Match(ao.webRequest.downloadHandler.text);
            if (!match.Success)
            {
                Debug.LogError("Token Parsing Failed !");
                tokenCallback.Invoke(null);
                yield break;
            }

            var searchObj = match.Groups[1];
            tokenCallback.Invoke(match.Groups[1].Value);
        }

        private string BuildRequestUrl(string keyword, string token, SafeSearch safeSearch, int pageNo, string location)
        {
            pageNo = Mathf.Clamp(pageNo, 1, int.MaxValue);
            Dictionary<string, string> parameters = new Dictionary<string, string>(){
                {"l", location},
                {"o", "json"},
                {"q", keyword},
                {"vqd", token},
                {"f", ",,,,,"},
                {"s", ((pageNo - 1) * 100).ToString()},
                {"p", safeSearch == SafeSearch.Off ? "-1" : "1"},
                {"v7exp", "a"},
            };
            return url + "i.js?" + string.Join("&", parameters.Select(kv => kv.Key + "=" + kv.Value));
        }
        
        private IEnumerator RequestSearchResult(string keyword, string token, SafeSearch safeSearch, int pageNo, string location, Action<ImageSearchResult> callback)
        {
            string requestUrl = BuildRequestUrl(keyword, token, safeSearch, pageNo, location);
            var request = UnityWebRequest.Get(requestUrl);
            SetRequestHeaders(request);
            var ao = SendRequest(request);
            
            yield return new WaitUntil(() => ao.isDone);
            if (ao.webRequest.isNetworkError || ao.webRequest.isHttpError)
            {
                Debug.LogError("Searching Failed ! " + ao.webRequest.error);
                isLastRequestSuccessfull = false;
                callback.Invoke(new ImageSearchResult { error = ao.webRequest.error });
                yield break;
            }
            isLastRequestSuccessfull = true;
            
            // Debug.Log(ao.webRequest.downloadHandler.text);
            var result = JsonUtility.FromJson<ImageSearchResult>(ao.webRequest.downloadHandler.text);
            callback.Invoke(result);
        }
    }
}
