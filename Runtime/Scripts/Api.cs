using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Arikan.Duckduckgo.Api
{
    public class Images : MonoBehaviour
    {
        private static Images instance;

        private const string url = "https://duckduckgo.com/";
        private string currentToken = null; // "3-303624948724554003560379222523657157514-88895724275933766665141839412464435395";
        private bool isLastRequestSuccessfull;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>(){
            {"authority", "duckduckgo.com"},
            {"accept", "application/json, text/javascript, */*; q=0.01"},
            {"x-requested-with", "XMLHttpRequest"},
            {"user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36"},
            {"referer", "https://duckduckgo.com/"},
            {"accept-language", "en-US,en;q=0.9"},
        };


        public static void Search(string text, int page, Action<ImageSearchResult> onCompleted)
        {
            if (!instance)
            {
                instance = new GameObject("DuckDuckGoAPI").AddComponent<Arikan.Duckduckgo.Api.Images>();
            }
            instance.SearchFromInstance(text, page, onCompleted);
        }


        private void Awake()
        {
            instance = this;
        }
        private void SearchFromInstance(string text, int page, Action<ImageSearchResult> onCompleted)
        {
            StartCoroutine(SearchCoRo(text, page, onCompleted));
        }
        private IEnumerator SearchCoRo(string keyword, int page, Action<ImageSearchResult> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(currentToken) || !isLastRequestSuccessfull)
            {
                yield return RequestToken(keyword, (token) => currentToken = token);
            }
            // Debug.Log("SrcOb:" + currentToken);
            yield return RequestSearchResult(keyword, currentToken, page, onCompleted);
        }

        private IEnumerator RequestToken(string keyword, Action<string> tokenCallback)
        {
            var ao = UnityWebRequest.Get(url + "?q=" + keyword).SendWebRequest();
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

        private IEnumerator RequestSearchResult(string keyword, string token, int page, Action<ImageSearchResult> callback)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>(){
                {"l", "us-en"},
                {"o", "json"},
                {"q", keyword},
                {"vqd", currentToken},
                {"f", ",,,"},
                {"p", "1"},
                {"v7exp", "a"},
            };
            string requestUrl = url
                + "i.js?"
                + string.Join("&", parameters.Select(kv => kv.Key + "=" + kv.Value));
            // Debug.Log(requestUrl);

            var request = UnityWebRequest.Get(requestUrl);
            foreach (var kv in headers)
            {
                // Debug.Log(kv.Key);
                request.SetRequestHeader(kv.Key, kv.Value);
            }
            var ao = request.SendWebRequest();

            yield return new WaitUntil(() => ao.isDone);
            if (ao.webRequest.isNetworkError || ao.webRequest.isHttpError)
            {
                Debug.LogError("Searching Failed ! " + ao.webRequest.error);
                callback.Invoke(null);
                yield break;
            }

            // Debug.Log(ao.webRequest.downloadHandler.text);
            var result = JsonUtility.FromJson<ImageSearchResult>(ao.webRequest.downloadHandler.text);
            callback.Invoke(result);
        }
    }
}
