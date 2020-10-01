using System.Collections.Generic;
using UnityEngine;

public class TouchObjectState
{
    public float timeTouchStart = -1;
    public float timeTapStart = -1;
    public TouchObject touchObject;
}

public class TouchObjectsController : MonoBehaviour //управление нажатием на предметы
{
    public static TouchObjectsController Instance;
    const float tapDuration = 0.5f;

    public enum TouchState
    {
        None = 0,
        Mouse = 1,
        Touch = 2,
    }

    TouchState touchState = TouchState.None;
    TouchState touchStatePrev = TouchState.None;
    Vector2 touchPoint;
    List<TouchObjectState> touchedPrev = null;   //список TouchObject, на которые были нажато в предыдущем кадре
    bool isLocked = false;

    private void Awake()
    {
        Instance = this;
        touchedPrev = new List<TouchObjectState>();
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public TouchState GetTouchState()
    {
        return touchState;
    }

    public Vector2 GetTouchPointScreen()
    {
        return touchPoint;
    }

    public GameObject HitObject(float radius = 0)
    {
        return HitObject(touchPoint, radius);
    }

    GameObject HitObject(Vector2 pointScreen, float radius = 0)
    {
        GameObject res = null;

        if (Camera.main != null)
        {
            Ray touchBeam = Camera.main.ScreenPointToRay(pointScreen);
            RaycastHit hit;

            if (radius > 0)
                Physics.SphereCast(touchBeam, radius, out hit);
            else
                Physics.Raycast(touchBeam, out hit);
            if (hit.collider != null) res = hit.collider.gameObject;
        }
        return res;
    }

    int HitID(Vector2 pointScreen)
    {
        int res = 0;

        if (Camera.main != null)
        {
            Ray touchBeam = Camera.main.ScreenPointToRay(pointScreen);
            RaycastHit hit;

            Physics.Raycast(touchBeam, out hit);
            if (hit.collider != null) res = hit.collider.gameObject.GetInstanceID();
        }
        return res;
    }

    TouchState DefineTouchState()
    {
        if (Input.touchCount > 0) return TouchState.Touch;
        else if (Input.GetMouseButton(0)) return TouchState.Mouse;
        return TouchState.None;
    }

    TouchObjectState GetTouchedPrev(TouchObject touchObject)
    {
        if (touchedPrev != null)
            foreach (var t in touchedPrev)
                if (t.touchObject == touchObject)
                    return t;
        return null;
    }

    void UpdateTouchedObjects(List<TouchObject> touched)
    {
        if (touchedPrev != null)
        {
            foreach (var touchObject in touchedPrev)
                if (touchObject.touchObject.enabled && (touched == null || !touched.Contains(touchObject.touchObject)))
                {
                    if (touchState == TouchState.None)
                    {
                        touchObject.touchObject.OnRelease();
                        if (touchObject.timeTapStart > 0 && Time.realtimeSinceStartup - touchObject.timeTapStart < tapDuration)
                            touchObject.touchObject.OnTap();
                    }
                    else touchObject.touchObject.OnTouchOut();
                }
        }

        if (touched != null)
        {
            var touchedPrevNew = new List<TouchObjectState>();
            float startTouchTime = Time.realtimeSinceStartup;
            float startTapTime = touchStatePrev == TouchState.None ? Time.realtimeSinceStartup : -1;

            foreach (var touchObject in touched)
                if (touchObject.enabled)
                {
                    touchObject.OnTouch();
                    var tp = GetTouchedPrev(touchObject);
                    if (tp != null) touchedPrevNew.Add(tp);
                    else
                    {
                        touchedPrevNew.Add(new TouchObjectState { touchObject = touchObject, timeTouchStart = startTouchTime, timeTapStart = startTapTime });
                        if (touchStatePrev == TouchState.None) touchObject.OnPress();
                    }
                }

            if (touchedPrev != null)
                touchedPrev.Clear();

            touchedPrev = touchedPrevNew;
        }
        else touchedPrev.Clear();
    }

    void UpdateTouch()
    {
        List<TouchObject> touched = null;

        if (Input.touchCount > 0)
        {
            //пробегаем по всем точкам нажатия на экран и собираем список TouchObject, на которые было нажато
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.touches[i];
                GameObject obj = HitObject(touch.position);

                if (i == 0) touchPoint = touch.position;

                if (obj != null)
                {
                    TouchObject[] touchObjects = obj.GetComponents<TouchObject>();
                    if (touchObjects != null)
                    {
                        if (touched == null) touched = new List<TouchObject>();
                        foreach (var t in touchObjects)
                            if (t.enabled)
                                touched.Add(t);

                        //touched.AddRange(touchObjects);
                        touchPoint = touch.position;
                    }
                }
            }
        }

        UpdateTouchedObjects(touched);
    }

    void UpdateMouse()
    {
        List<TouchObject> touched = null;

        if (Input.GetMouseButton(0))
        {
            //находим объект, на который было нажато мышкой
            touchPoint = Input.mousePosition;
            GameObject obj = HitObject(touchPoint);

            //если такой объект есть, то собираем его TouchObjects
            if (obj != null)
            {
                TouchObject[] touchObjects = obj.GetComponents<TouchObject>();
                if (touchObjects != null)
                {
                    if (touched == null) touched = new List<TouchObject>();
                    touched.AddRange(touchObjects);
                }
            }
        }

        UpdateTouchedObjects(touched);
    }

    void Update()
    {
        if (isLocked)
            return;

        touchStatePrev = touchState;

        switch (DefineTouchState())
        {
            case TouchState.Touch:
                UpdateTouch();
                touchState = TouchState.Touch;
                break;
            case TouchState.Mouse:
                UpdateMouse();
                touchState = TouchState.Mouse;
                break;
            default:
                touchState = TouchState.None;
                UpdateTouchedObjects(null);
                break;
        }
    }
}

