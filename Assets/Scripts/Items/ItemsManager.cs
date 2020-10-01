using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour   //реакция на события из жизни предметов: нажали/потащили/бросили
{
    public BagManager bag;
    public List<ItemController> items;

    private void Start()
    {
        foreach (var i in items)
            if (i != null)
            {
                i.onDragEvent.AddListener(OnItemDrag);
                i.onTapEvent.AddListener(OnItemTap);
            }
    }

    ItemController GetItem(int id)
    {
        foreach (var i in items)
            if (i != null && i.item.id == id)
                return i;
        return null;
    }

    void OnItemDrag(int id, bool isDrag)
    {
        foreach (var i in items)
            if (i != null && i.item.id != id && !bag.IsInBag(i))
            {
                i.skin.enabled = !isDrag;
                i.enabled = !isDrag;
            }

        var item = GetItem(id);
        if (!isDrag && bag.IsBagTouched(item.skin.Radius))
            bag.PlaceItem(item);
    }

    void OnItemTap(ItemController item)
    {
        if (bag.IsInBag(item))
            bag.ReleaseItem(item);
    }
}
