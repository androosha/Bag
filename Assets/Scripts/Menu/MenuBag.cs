using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuBag : Menu     //меню для отображения содержимого рюкзака
{
    public MenuBagItem itemTemplate;

    List<MenuBagItem> bgItems;

    protected override void Init()
    {
        base.Init();
        itemTemplate.gameObject.SetActive(false);
    }

    MenuBagItem CreateBgItem(ItemCell cell)
    {
        var bgItem = Instantiate(itemTemplate);
        bgItem.transform.SetParent(itemTemplate.transform.parent);
        bgItem.transform.localScale = Vector3.one;
        bgItem.Set(cell);
        bgItem.gameObject.SetActive(true);
        return bgItem;
    }

    public void Set(Bag bag)
    {
        if (bgItems == null) bgItems = new List<MenuBagItem>();

        for (int i = 0; i < Mathf.Max(bag.CellsCount, bgItems.Count); i++)
        {
            if (i < bag.CellsCount)
            {
                var cell = bag.GetCell(i);
                if (i >= bgItems.Count)
                    bgItems.Add(CreateBgItem(cell));
                else
                {
                    bgItems[i].gameObject.SetActive(true);
                    bgItems[i].Set(cell);
                }
            }
            else bgItems[i].gameObject.SetActive(false);
        }
    }
}
