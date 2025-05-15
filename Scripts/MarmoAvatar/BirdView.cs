using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BirdView : MonoBehaviour
{
    Quaternion cam_rotation;
    GameObject m_camera_obj;
    Camera m_camera;
    [SerializeField] GameObject agent;
    [SerializeField] Vector3 cam_backoff;
    GameObject m_plane_obj;
    public Vector3 click_pt { get; private set; }
    public UnityEvent<Vector3> onClickListeners;

    [SerializeField] RawImage ui_panel;

    void Start()
    {
        cam_rotation = Quaternion.Euler(45, -135, 0);
        m_camera_obj = transform.Find("BirdViewCam").gameObject;
        m_camera = m_camera_obj.GetComponent<Camera>();

        m_plane_obj = transform.Find("Plane").gameObject;

        m_camera_obj.transform.localRotation = cam_rotation;
        m_camera_obj.transform.localPosition = cam_backoff;
    }

    Vector3 doRayCast(Vector3 from_pt, Vector3 to_pt, Plane plane)
    {
        Ray ray = new Ray(from_pt, to_pt - from_pt);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            return from_pt + dist * ray.direction;
        }
        else
        {
            Debug.LogWarning("error in ray case");
            return from_pt;
        }
    }

    void Update()
    {
        transform.position = agent.transform.position;
    }

    Vector2 calcMousePos(Vector2 event_pos, out bool click_succ)
    {
        if (ui_panel)
        {
            Debug.Log(m_camera.pixelHeight);
            float yratio =  (Screen.height - event_pos.y) / ui_panel.rectTransform.rect.height;
            click_succ = (yratio < 1 && yratio > 0);
            return new Vector2(
                event_pos.x,
                m_camera.pixelHeight * yratio);
        }
        else {
            click_succ = true;
            return new Vector2(
                event_pos.x,
                m_camera.pixelHeight - event_pos.y);
        }
    }

    void OnGUI()
    {
        Event curr_event = Event.current;
        if (curr_event.type != EventType.MouseDown && curr_event.type != EventType.TouchDown)
            return;
        bool click_succ;
        Vector2 mouse_pos = calcMousePos(curr_event.mousePosition, out click_succ);
        if (!click_succ) return;

        Vector3 pt = m_camera.ScreenToWorldPoint(
            new Vector3(mouse_pos.x, mouse_pos.y, 1));

        if (m_camera.orthographic)
        {
            click_pt = doRayCast(
                pt, pt + m_camera_obj.transform.rotation * Vector3.forward,
                new Plane(m_plane_obj.transform.rotation * Vector3.up,
                        m_plane_obj.transform.position));
        }
        else
        {
            click_pt = doRayCast(
                m_camera_obj.transform.position, pt,
                new Plane(m_plane_obj.transform.rotation * Vector3.up,
                        m_plane_obj.transform.position)
            );
        }
        

        onClickListeners.Invoke(click_pt);
    }
}
