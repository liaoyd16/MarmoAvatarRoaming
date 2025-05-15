using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchMatcher : MonoBehaviour
{
    [SerializeField] GameObject clickpt;

    [SerializeField] GameObject plane;
    Camera m_cam;
    void Start()
    {
        m_cam = Camera.main;
    }

    Vector3 doRayCast(Vector3 point1, Vector3 point2, Plane plane)
    {
        Ray ray = new Ray(point1, point2 - point1);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            return point1 + dist * ray.direction;
        }
        else
        {
            return point1;
        }
    }

    void OnGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = m_cam.pixelHeight - currentEvent.mousePosition.y;

        point = m_cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + m_cam.pixelWidth + ":" + m_cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();

        Vector3 plane_pt = plane.transform.position;
        Vector3 plane_norm = plane.transform.rotation * Vector3.up;
        clickpt.transform.position = doRayCast(
            m_cam.transform.position,point,
            new Plane(plane_norm, plane_pt));
    }
}
