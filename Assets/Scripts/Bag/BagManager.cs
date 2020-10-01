using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BagManager : TouchObject   //управление рюкзаком - положить/вынуть/посмотреть
{
    public class OnItemInOutEvent : UnityEvent<int, int> {}

    public bool lockOnPark = true;  //true - пока предыдущий предмет не запаркован в рюкзаке, новый нельзя положить/вынуть, false - предметы можно класть/вынимать без ожидания
    public Bag bag;
    public List<BagPlace> places;
    public MenuBag menuBag;
    public Color colorBusy = Color.red;
    public OnItemInOutEvent onItemInOutEvent;   //событие в жизни предметов в рюкзаке

    Color colorFree;
    bool isLocked = false;

    protected override void Awake()
    {
        base.Awake();

        colorFree = GetComponent<MeshRenderer>().material.color;
        bag.Init();
        onItemInOutEvent = new OnItemInOutEvent();
    }

    void Lock(bool isLocked = true)
    {
        this.isLocked = lockOnPark && isLocked;
        UpdateLockIndicator();
    }

    void Unlock()
    {
        Lock(false);
    }

    void UpdateLockIndicator()
    {
        GetComponent<MeshRenderer>().material.color = isLocked ? colorBusy : colorFree;
    }

    public bool IsBagTouched(float radius)
    {
        GameObject touched = TouchObjectsController.Instance.HitObject(radius);
        return touched != null && touched == gameObject;
    }

    BagParker GetParker(GameObject gameObject)
    {
        return gameObject.AddComponent<BagParker>();
    }

    BagPlace GetPlace(ItemType type)
    {
        foreach (var place in places)
            if (place != null && place.type == type)
                return place;
        return null;
    }

    public bool IsInBag(ItemController item)
    {
        return item != null && item.item != null && bag.GetCellIndex(item.item.id) >= 0;
    }

    public void PlaceItem(ItemController item)
    {
        if (item != null && bag.HasCell(item.item.type) && !isLocked)
        {
            var place = GetPlace(item.item.type);
            if (place == null)
                Debug.LogError("no place for " + item.item.type + " in the bag");
            else
            {
                bag.Place(item.item);
                item.Lock();

                var parker = GetParker(item.gameObject);
                parker.Park(place.transform, item, OnParkComplete);
                Lock();
            }
        }
    }

    void OnParkComplete(ItemController item)
    {
        if (onItemInOutEvent != null) onItemInOutEvent.Invoke(item.item.id, 1);
        Unlock();
        Debug.Log("Bag parking complete: " + item.item.name);
    }

    public void ReleaseItem(ItemController item)
    {
        if (IsInBag(item) && !isLocked)
        {
            Lock();
            float a = Random.Range(0, Mathf.PI * 2);
            Vector3 pos = transform.position + Vector3.up * item.skin.Radius * 4 + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 5f;

            var parker = GetParker(item.gameObject);
            parker.UnPark(pos, Quaternion.identity, item, OnReleaseItemComplete);
        }
    }

    void OnReleaseItemComplete(ItemController item)
    {
        bag.Withdraw(item.item.id);
        item.Unlock();
        if (onItemInOutEvent != null) onItemInOutEvent.Invoke(item.item.id, 0);
        Unlock();
        Debug.Log("Bag release complete: " + item.item.name);
    }

    public override void OnPress()
    {
        base.OnPress();
        if (!menuBag.IsOn)
        {
            menuBag.Set(bag);
            menuBag.SwitchOn(null);
        }
    }

    public override void OnRelease()
    {
        base.OnRelease();
        menuBag.SwitchOff();
    }

    public override void OnTouchOut()
    {
        base.OnTouchOut();
        menuBag.SwitchOff();
    }
}
