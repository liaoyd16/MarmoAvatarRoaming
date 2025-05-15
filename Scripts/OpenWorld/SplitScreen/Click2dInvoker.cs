using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Click2dInvoker : MonoBehaviour
{
    public UnityEvent<TaskEvent> click2d_listener;
    [SerializeField] Vector2 upleft, botright;

    void Start()
    {
        RectTransform area = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        area.GetWorldCorners(corners);
        Vector3 temp = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
        upleft.Set(temp.x, Screen.height - temp.y);
        temp = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        botright.Set(temp.x, Screen.height - temp.y);
    }

    void OnGUI()
    {
        Event curr_event = Event.current;
        if (curr_event.type != EventType.MouseDown && curr_event.type != EventType.TouchDown)
            return;

        if (!((upleft.x < curr_event.mousePosition.x && curr_event.mousePosition.x < botright.x)
              && (upleft.y < curr_event.mousePosition.y && curr_event.mousePosition.y < botright.y))
        )
            return;

        click2d_listener.Invoke(
            new ClickDummy(curr_event.mousePosition));
    }
}
