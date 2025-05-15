using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskEvent
{
    public abstract string Repr();
}

public class ClickPosEvent : TaskEvent
{
    Vector3 clickpos;
    public ClickPosEvent(Vector3 clickpos_)
    {
        this.clickpos = clickpos_;
    }
    public override string Repr()
    {
        return string.Format("marmoset click pos at {0}", clickpos);
    }
}

public class ClickDummy : TaskEvent
{
    Vector2 clickpos;
    public ClickDummy(Vector2 clickpos_)
    {
        this.clickpos = clickpos_;
    }
    public override string Repr()
    {
        return string.Format("marmoset click upper panel at {0}", clickpos);
    }
}

public class EventCollector : MonoBehaviour
{
    SocketController socketController;

    void Start()
    {
        socketController = GetComponent<SocketController>();
    }

    public void onTaskEvent(TaskEvent e)
    {
        socketController.SendSocket(e.Repr());
    }
}
