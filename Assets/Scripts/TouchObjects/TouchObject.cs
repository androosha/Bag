using System;
using UnityEngine;
using UnityEngine.Events;

public class TouchObject : MonoBehaviour    //предмет, реагирующий на нажатия пользователя
{
    [Serializable]
    public class OnItemTap : UnityEvent<TouchObject>  {}
    public class OnItemRelease : UnityEvent<TouchObject> { }

    public OnItemTap onItemTap;
    public OnItemRelease onItemRelease;

    virtual protected void Awake()
    {
        onItemTap = new OnItemTap();
        onItemRelease = new OnItemRelease();
    }

    virtual protected void Start()
    {
        if (TouchObjectsController.Instance == null)
            Debug.LogError("No TouchObjectsController on the scene!");
    }

    virtual public void OnTouch()   //прикоснулись к объекту (нажали или провели по нему пальцем/мышкой)
    {
    }

    virtual public void OnRelease()
    {
        //Debug.Log(name + " OnRelease");
        onItemRelease.Invoke(this);
    }

    virtual public void OnTouchOut()
    {
        //Debug.Log(name + " OnTouchOut");
    }

    virtual public void OnPress()   //нажали на объект
    {
        //Debug.Log(name + " OnPress");
    }

    virtual public void OnTap() //нажали и отпустили
    {
        onItemTap.Invoke(this);
    }
}
