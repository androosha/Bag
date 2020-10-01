using UnityEngine;

public class BagParker : MonoBehaviour   //плавно перемещает предмет в нужное место
{
    System.Action<ItemController> onComplete;
    Vector3 position;
    Quaternion rotation;
    ItemController item;
    bool isBusy = false;

    const string easeType = "linear";
    const float stepTime = 0.5f;        //длительность одного шага перемещения

    public bool IsBusy()
    {
        return isBusy;
    }

    public void Park(Transform place, ItemController item, System.Action<ItemController> onComplete)
    {
        Park(place.position, place.rotation, item, Park1, onComplete);
    }

    public void UnPark(Vector3 position, Quaternion rotation, ItemController item, System.Action<ItemController> onComplete)
    {
        Park(position, rotation, item, UnPark1, onComplete);
    }

    void Park(Vector3 position, Quaternion rotation, ItemController item, System.Action action, System.Action<ItemController> onComplete)
    {
        this.onComplete = onComplete;
        this.item = item;
        this.position = position;
        this.rotation = rotation;
        isBusy = true;
        action();
    }

    void Park1()
    {
        float delay = stepTime;
        Vector3 pos = position + Vector3.up * 2;
        iTween.MoveTo(item.gameObject, iTween.Hash("position", pos, "time", delay, "easetype", easeType, "oncompletetarget", gameObject, "oncomplete", "Park2"));
        iTween.RotateTo(item.gameObject, rotation.eulerAngles, delay);
    }

    void Park2()
    {
        float delay = stepTime;
        Vector3 pos = position;
        iTween.MoveTo(item.gameObject, iTween.Hash("position", pos, "time", delay, "easetype", easeType, "oncompletetarget", gameObject, "oncomplete", "OnComplete"));
    }

    void UnPark1()
    {
        float delay = stepTime;
        Vector3 pos = item.transform.position + Vector3.up * 2;
        iTween.MoveTo(item.gameObject, iTween.Hash("position", pos, "time", delay, "easetype", easeType, "oncompletetarget", gameObject, "oncomplete", "UnPark2"));
    }

    void UnPark2()
    {
        float delay = stepTime;
        Vector3 pos = position;
        iTween.MoveTo(item.gameObject, iTween.Hash("position", pos, "time", delay, "easetype", easeType, "oncompletetarget", gameObject, "oncomplete", "OnComplete"));
        iTween.RotateTo(item.gameObject, rotation.eulerAngles, delay);
    }

    void OnComplete()
    {
        isBusy = false;
        if (onComplete != null) onComplete.Invoke(item);
        Destroy(this, Time.deltaTime);
    }
}
