using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour //передача (и в перспектике получение) данных на (в перспектике с) сервер(в перспективе а)
{
    const string URL = "https://dev3r02.elysium.today/inventory/status";
    const string AUTH = "BMeHG5xqJeB4qCjpuJCTQLsqNGaqkfB6";

    public BagManager bag;

    void Start()
    {
        if (bag != null)
            bag.onItemInOutEvent.AddListener(OnItemInOutEvent);
        else
            Debug.LogError("no BAG attached!");
    }

    void OnItemInOutEvent(int itemId, int eventId)
    {
        StartCoroutine(Send(itemId, eventId));
    }

    IEnumerator Wait(float delay, IEnumerator ieAction)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ieAction);
    }

    IEnumerator Send(int itemId, int eventId)
    {
        Debug.Log("Server.Send: item=" + itemId + ", event=" + eventId);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(itemId.ToString(), eventId.ToString()));

        UnityWebRequest www = UnityWebRequest.Post(URL, formData);
        www.SetRequestHeader("auth", AUTH);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Server.Send result:" + www.error);
            StartCoroutine(IEResent(itemId, eventId, 0.5f));
        }
        else
        {
            Debug.Log("Server.Send result: text=" + www.downloadHandler.text);
        }
    }

    IEnumerator IEResent(int itemId, int eventId, float delay)
    {
        yield return new WaitForSeconds(delay);
        Send(itemId, eventId);
    }
}