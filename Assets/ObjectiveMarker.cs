using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveMarker : MonoBehaviour
{
    public Transform TargetTransform;
    public Image img;

    public Mode updateMode;

    public int offsetUp;
    public int offsetDown;
    public int offsetLeft;
    public int offsetRight;

    public enum Mode
    {
        Common,
        Overwatch,
    }

    private void LateUpdate()
    {
        switch (updateMode)
        {
            case Mode.Common:
                Method1();
                break;
            case Mode.Overwatch:
                Method2();
                break;
            default:
                break;
        }
    }

    private void Method1()
    {
        Transform camTransform = Camera.main.transform;

        Vector3 newPos = new();

        Vector3 delta = TargetTransform.position - camTransform.position;
        float dot = Vector3.Dot(camTransform.forward, delta);

        if (dot < 0)
        {
            Vector3 projectedPos = camTransform.position + (delta - camTransform.forward * Vector3.Dot(camTransform.forward, delta) * 1.01f);
            newPos = Camera.main.WorldToScreenPoint(projectedPos);
        }
        else
        {
            newPos = Camera.main.WorldToScreenPoint(TargetTransform.position);
        }

        if (newPos.x > Screen.width - offsetRight || newPos.x < offsetLeft || newPos.y > Screen.height - offsetUp || newPos.y < offsetDown)
            newPos = KClamp(newPos);

        img.transform.position = newPos;
    }
    private void Method2()
    {
        Transform camTransform = Camera.main.transform;

        var vFov = Camera.main.fieldOfView;
        var radHFov = 2 * Mathf.Atan(Mathf.Tan(vFov * Mathf.Deg2Rad / 2) * Camera.main.aspect);
        var hFov = Mathf.Rad2Deg * radHFov;

        Vector3 deltaUnitVec = (TargetTransform.position - camTransform.position).normalized;

        /* How the angles work:
         * vdegobj: objective vs xz plane (horizontal plane). Upright = -90, straight down = 90.
         * vdegcam: camera forward vs xz plane. same as above.
         * vdeg: obj -> cam. if obj is higher, value is negative.
         */

        float vdegobj = Vector3.Angle(Vector3.up, deltaUnitVec) - 90f;
        float vdegcam = Vector3.SignedAngle(Vector3.up, camTransform.forward, camTransform.right) - 90f;

        float vdeg = vdegobj - vdegcam;

        float hdeg = Vector3.SignedAngle(Vector3.ProjectOnPlane(camTransform.forward, Vector3.up), Vector3.ProjectOnPlane(deltaUnitVec, Vector3.up), Vector3.up);

        vdeg = Mathf.Clamp(vdeg, -89f, 89f);
        //vdeg = Mathf.Clamp(vdeg, vFov * -0.5f, vFov * 0.5f);
        //hdeg = Mathf.Clamp(hdeg, -89f, 89f);
        hdeg = Mathf.Clamp(hdeg, hFov * -0.5f, hFov * 0.5f);

        Vector3 projectedPos = Quaternion.AngleAxis(vdeg, camTransform.right) * Quaternion.AngleAxis(hdeg, camTransform.up) * camTransform.forward;
        Debug.DrawLine(camTransform.position, camTransform.position + projectedPos, Color.red);

        Vector3 newPos = Camera.main.WorldToScreenPoint(camTransform.position + projectedPos);

        if (newPos.x > Screen.width - offsetRight || newPos.x < offsetLeft || newPos.y > Screen.height - offsetUp || newPos.y < offsetDown)
            newPos = KClamp(newPos);

        img.transform.position = newPos;
    }

    private Vector3 KClamp(Vector3 newPos)
    {
        Vector2 center = new(Screen.width / 2, Screen.height / 2);
        float k = (newPos.y - center.y) / (newPos.x - center.x);

        if (newPos.y - center.y > 0)
        {
            newPos.y = Screen.height - offsetUp;
            newPos.x = center.x + (newPos.y - center.y) / k;
        }
        else
        {
            newPos.y = offsetDown;
            newPos.x = center.x + (newPos.y - center.y) / k;
        }

        if (newPos.x > Screen.width - offsetRight)
        {
            newPos.x = Screen.width - offsetRight;
            newPos.y = center.y + (newPos.x - center.x) * k;
        }
        else if (newPos.x < offsetLeft)
        {
            newPos.x = offsetLeft;
            newPos.y = center.y + (newPos.x - center.x) * k;
        }

        return newPos;
    }
}
