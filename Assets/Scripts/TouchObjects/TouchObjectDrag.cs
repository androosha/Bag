using System;
using UnityEngine;
using UnityEngine.Events;

public class TouchObjectDrag : TouchObject
{
    [Serializable]
    public class OnDragEvent : UnityEvent<bool>
    {
    }

    public OnDragEvent onDragEvent;

    bool isTouched = false;
    Vector3 startTouchPosScreen;
    Vector3 startPosition;
    Rigidbody _rigid;
    float _radius = -1;

    public Rigidbody Rigid
    {
        get
        {
            if (_rigid == null) _rigid = GetComponent<Rigidbody>();
            return _rigid;
        }
    }

    public float Radius
    {
        get
        {
            if (_radius < 0)
            {
                var sphere = GetComponent<SphereCollider>();
                if (sphere != null) _radius = sphere.radius;
                else
                {
                    var box = GetComponent<BoxCollider>();
                    if (box != null) _radius = box.size.magnitude / 2f;
                    else _radius = 1;
                }
            }

            return _radius;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (onDragEvent == null)
            onDragEvent = new OnDragEvent();
    }

    public override void OnTouch()
    {
        if (!isTouched)
        {
            isTouched = true;
            startTouchPosScreen = TouchObjectsController.Instance.GetTouchPointScreen();
            startPosition = transform.position;

            Rigid.useGravity = false;
            GetComponent<Collider>().enabled = false;
            if (onDragEvent != null) onDragEvent.Invoke(true);
        }
        base.OnTouch();
    }

    void DragComplete()
    {
        isTouched = false;
        if (onDragEvent != null) onDragEvent.Invoke(false);
        if (enabled)
        {
            Rigid.useGravity = true;
        }
        GetComponent<Collider>().enabled = true;
        base.OnRelease();
    }

    void UpdateModeGround()
    {
        if (Camera.main != null)
        {
            var pointScreen = TouchObjectsController.Instance.GetTouchPointScreen();

            Ray touchBeam = Camera.main.ScreenPointToRay(pointScreen);
            RaycastHit hit;

            Physics.SphereCast(touchBeam, Radius, out hit);
            if (hit.collider != null)
            {
                float dist = hit.distance - Radius;
                transform.position = touchBeam.origin + touchBeam.direction * dist;
            }
        }
    }

/*    void UpdateMoveScreen()
    {
        var touchPosScreen = TouchObjectsController.Instance.GetTouchPointScreen();
        var touchPoint = Camera.main.ScreenPointToRay(touchPosScreen).origin;
        var startTouchPoint = Camera.main.ScreenPointToRay(startTouchPosScreen).origin;

        float r = (Camera.main.transform.position - startTouchPoint).magnitude;
        float rReal = (Camera.main.transform.position - startPosition).magnitude;
        float rate = rReal / r;

        Vector3 dv = touchPoint - startTouchPoint;
        transform.position = startPosition + dv * rate;
    }*/

    private void Update()
    {
        if (isTouched)
        {
            if (TouchObjectsController.Instance.GetTouchState() != TouchObjectsController.TouchState.None)
            {
                UpdateModeGround();
                /*if (mode == Mode.Screen) UpdateMoveScreen();
                if (mode == Mode.Ground) UpdateModeGround();*/
            }
            else DragComplete();
        }
    }
}
