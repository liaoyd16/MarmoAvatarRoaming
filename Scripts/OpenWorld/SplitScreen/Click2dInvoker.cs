using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Click2dInvoker : MonoBehaviour
{
    public UnityEvent<TaskEvent> click2d_listener;
    void OnGUI()
    {
        Event curr_event = Event.current;
        if (curr_event.type != EventType.MouseDown && curr_event.type != EventType.TouchDown)
            return;
        
        click2d_listener.Invoke(
            new ClickDummy(curr_event.mousePosition));
    }
}
