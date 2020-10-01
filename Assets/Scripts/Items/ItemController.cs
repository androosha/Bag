using System;
using UnityEngine;
using UnityEngine.Events;

public class ItemController : MonoBehaviour //управление поведением предмета и передача событий в ItemsManager
{
    [Serializable]
    public class OnItemDragEvent : UnityEvent<int, bool>  {}

    [Serializable]
    public class OnItemTapEvent : UnityEvent<ItemController>  {}

    public Item item;
    public TouchObjectDrag skin;
    public TouchObject tap;

    public OnItemDragEvent onDragEvent;
    public OnItemTapEvent onTapEvent;

    private void Awake()
    {
        onDragEvent = new OnItemDragEvent();
        onTapEvent = new OnItemTapEvent();
    }

    private void Start()
    {
        if (item == null || item.id < 1)
            Debug.LogError("no valid data for item " + name);

        if (skin == null) Debug.LogError("skin is null!");
        else
        {
            skin.onDragEvent.AddListener(OnDragEvent);
            tap.onItemRelease.AddListener(OnTapEvent);
            skin.Rigid.mass = item.mass;
        }
    }

    void OnDragEvent(bool isDrag)
    {
        if (onDragEvent != null) onDragEvent.Invoke(item.id, isDrag);
    }

    void OnTapEvent(TouchObject touch)
    {
        Debug.Log(name + " tap");
        if (onTapEvent != null) onTapEvent.Invoke(this);
    }

    public void Unlock()
    {
        Lock(false);
    }

    public void Lock(bool isLocked = true)
    {
        Debug.Log(name + " isLocked=" + isLocked);
        skin.Rigid.useGravity = !isLocked;
        if (isLocked) skin.Rigid.velocity = Vector3.zero;
        if (skin != null) skin.enabled = !isLocked;
    }
}
