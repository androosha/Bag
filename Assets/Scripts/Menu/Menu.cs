using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Animator animator;
    public AudioClip switchOnAudio;
    public AudioClip switchOffAudio;
    public float switchOnDuration = 0.5f;
    public float switchOffDuration = 0.5f;

    public bool lockTouchObjects = true;
    public bool muteMusicOnSwitchOn = false;

    static List<Menu> menuList = null;

    protected bool isOn = false;
    protected IEnumerator ieDone;

    protected Action onOkAction;
    protected Action onCancelAction;

    protected Action onOnAction;
    protected Action onOffAction;

    public bool IsOn
    {
        get
        {
            return isOn;
        }
    }

    static void Register(Menu menu)
    {
        if (menuList == null) menuList = new List<Menu>();
        if (!menuList.Contains(menu)) menuList.Add(menu);
    }

    public static int OpenMenuCount
    {
        get
        {
            int res = 0;
            if (menuList != null)
                foreach (var m in menuList)
                    if (m.IsOn)
                        res++;
            return res;
        }
    }

    virtual protected void Init()
    {
    }

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        Register(this);
        Init();
    }

    private void OnDisable()
    {
        if (ieDone != null) StopCoroutine(ieDone);
        ieDone = null;
    }

    virtual public void SwitchOn(Action onOkAction, Action onCancelAction, Action onOnAction = null)
    {
        this.onOkAction = onOkAction;
        this.onCancelAction = onCancelAction;
        this.onOnAction = onOnAction;
        SwitchOn(onOnAction);
    }

    virtual public void SwitchOn(Action onOnAction)
    {
        if (!isOn && ieDone == null)
        {
            animator.SetTrigger("show");

            if (lockTouchObjects) TouchObjectsController.Instance.Lock();
            isOn = true;
            ieDone = IESwitchOn(switchOnDuration, onOnAction);
            StartCoroutine(ieDone);
        }
    }

    IEnumerator IESwitchOn(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        ieDone = null;
        SwitchOnDone(callback);
    }

    virtual protected void SwitchOnDone(Action callback)
    {
        if (callback != null) callback.Invoke();
    }

    virtual public void SwitchOff(Action onComplete)
    {
        if (isOn)
        {
            animator.SetTrigger("hide");
            isOn = false;

            if (ieDone != null) StopCoroutine(ieDone);
            ieDone = IESwitchOff(switchOffDuration, onComplete);
            StartCoroutine(ieDone);
        }
    }

    IEnumerator IESwitchOff(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        ieDone = null;
        SwitchOffDone(callback);
    }

    virtual protected void SwitchOffDone(Action callback)
    {
        if (lockTouchObjects) TouchObjectsController.Instance.Unlock();
        if (callback != null) callback.Invoke();
    }

    public void SwitchOff()
    {
        SwitchOff(onOffAction);
    }

    virtual public void OnOk()
    {
        onOffAction = onOkAction;
        SwitchOff();
    }

    public void OnCancel()
    {
        if (isOn)
        {
            onOffAction = onCancelAction;
            SwitchOff();
        }
    }

    virtual protected void onAndroidHome()
    {
    }

    virtual protected void onAndroidEscape()
    {
        OnCancel();
    }

    virtual protected void onAndroidMenu()
    {
    }

    void CheckAndroidButtons()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) onAndroidEscape();
        if (Input.GetKeyDown(KeyCode.Home)) onAndroidHome();
        if (Input.GetKeyDown(KeyCode.Menu)) onAndroidMenu();
    }

    virtual protected void Update()
    {
        CheckAndroidButtons();
    }
}