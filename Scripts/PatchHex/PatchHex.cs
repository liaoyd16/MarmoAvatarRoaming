using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PatchHex : MonoBehaviour
{
    [SerializeField] float m_wid = 5;
    float EPS = 2e-1f;
    [SerializeField] float click_thres = 4;
    List<Transform> stay_spots = null;
    static ObjectPool<GameObject> spot_pool = new ObjectPool<GameObject>(
        createFunc, actionOnRelease: actionOnRelease
    );

    static GameObject createFunc()
    {
        GameObject p = Instantiate(Resources.Load<GameObject>("Prefabs/StaySpot"), Vector3.down * 100, Quaternion.identity, null);
        return p;
    }

    static void actionOnRelease(GameObject p)
    {
        p.SetActive(false);
        p.transform.SetPositionAndRotation(
            Vector3.down * 100, Quaternion.identity);
    }

    void OnDisable()
    {
        foreach (Transform spot in stay_spots)
        {
            if (spot.childCount > 0)
                spot_pool.Release(spot.GetChild(0).gameObject);
        }
    }

    void OnEnable()
    {
        if (stay_spots == null)
        {
            stay_spots = new List<Transform>();
            for (int i = 0; i < 7; i++)
            {
                string group_ = "SpotArray/Group" + i;
                GameObject groupx = transform.Find(group_).gameObject;
                for (int j = 0; j < groupx.transform.childCount; j++)
                {
                    stay_spots.Add(groupx.transform.GetChild(j));
                }
            }
        }
        foreach (Transform spot in stay_spots)
        {
            GameObject p = spot_pool.Get();
            p.transform.SetParent(spot, false);
            p.transform.SetLocalPositionAndRotation(Vector3.up * 0.8f, Quaternion.identity);
            p.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    public void getAdjList(Vector3 worldpos, Transform avatar_transform, List<Transform> buf) {
        buf.Clear();

        // not reachable / stay put
        Vector3 diff = worldpos - avatar_transform.position;
        if (diff.magnitude > m_wid * (1+EPS)) return;
        if (diff.magnitude < click_thres) return;

        // not in direction
        if (Vector3.Dot(avatar_transform.rotation * Vector3.forward, diff) < 0) return;

        for (int i = 0; i < 7; i++)
        {
            string group_ = "SpotArray/Group" + i;
            GameObject groupx = transform.Find(group_).gameObject;
            for (int j = 0; j < groupx.transform.childCount; j++)
            {
                diff = groupx.transform.GetChild(j).transform.position - worldpos;
                // Debug.Log(diff);
                if (diff.magnitude < click_thres)
                {
                    buf.Add(groupx.transform.GetChild(j).transform);
                }
            }
        }
    }
}
