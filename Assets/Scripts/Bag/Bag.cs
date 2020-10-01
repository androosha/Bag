using System;
using System.Collections.Generic;

[Serializable]
public class ItemCell
{
    public ItemType type;
    public Item item;
}

[Serializable]
public class Bag    //структура для хранения предметов в рюкзаке
{
    public List<ItemType> itemTypes;    //список типов предметов, под которые надо создать ячейки
    List<ItemCell> itemCells;    //ячейки под предметы в рюкзаке

    public void Init()
    {
        itemCells = new List<ItemCell>();
        for (int i = 0; i < itemTypes.Count; i++)
            itemCells.Add(new ItemCell { type = itemTypes[i] });
    }

    public bool HasCell(ItemType type)
    {
        return GetCell(type) != null;
    }

    public ItemCell GetCell(ItemType type)
    {
        if (itemCells != null)
            foreach (var cell in itemCells)
                if (cell != null && cell.type == type)
                    return cell;
        return null;
    }

    public ItemCell GetCell(int index)
    {
        if (itemCells != null && index < itemCells.Count)
            return itemCells[index];
        return null;
    }

    public int GetCellIndex(int itemId)
    {
        if (itemCells != null)
            for (int i = 0; i < itemCells.Count; i++)
                if (itemCells[i].item != null && itemCells[i].item.id == itemId)
                    return i;
        return -1;
    }

    public bool Place(Item item)    //попробовать положить предмет item на своё место в рюкзаке
    {
        var cell = GetCell(item.type);
        if (cell != null)
        {
            cell.item = item;
            return true;
        }
        return false;
    }

    public Item Withdraw(int itemId)    //вынуть предмет с itemId из рюкзака
    {
        int index = GetCellIndex(itemId);
        if (index >= 0)
        {
            var cell = itemCells[index];
            var item = cell.item;
            cell.item = null;
            return item;
        }

        return null;
    }

    public int CellsCount
    {
        get
        {
            if (itemCells != null)
                return itemCells.Count;
            return 0;
        }
    }

    new public string ToString()
    {
        string res = "{";
        if (itemCells != null)
            for (int i = 0; i < itemCells.Count; i++)
            {
                var cell = itemCells[i];
                if (i > 0) res += ",";
                res += "[type=" + cell.type.ToString() + ", item=" + (cell.item != null ? cell.item.ToString() : "null") + "]";
            }
        return res;
    }
}
