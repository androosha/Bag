using UnityEngine;
using UnityEngine.UI;

public class MenuBagItem : MonoBehaviour    //элемент описания предмета в меню MenuBag
{
    public Text textSkinType;
    public Text textNickName;

    public void Set(ItemCell cell)
    {
        textNickName.text = cell.item != null ? cell.item.name : ". . .";
        textSkinType.text = cell.type.ToString();
    }
}
