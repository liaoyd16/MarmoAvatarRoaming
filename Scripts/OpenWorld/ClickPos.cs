using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class ClickPos : MonoBehaviour
{
    [SerializeField] float twinkle_duration;
    [SerializeField] float twinkle_radius;
    float twinkle_downcnt;
    public UnityEvent<Vector3> onClickVec3;
    public UnityEvent<TaskEvent> onClickPosListener;
    [SerializeField] GameObject avatar;
    WorldPatchManager worldPatchManager;

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        twinkle_downcnt = 0f;
        worldPatchManager = GameObject.Find("WorldPatchManager").GetComponent<WorldPatchManager>();
    }

    public void onClickPos(Vector3 click_pos)
    {
        transform.position = click_pos;
        twinkle_downcnt = twinkle_duration;
        StartCoroutine(Twinkle());
        // judgement: click position is available?
        try
        {
            List<Transform> adjlist = new List<Transform>();
            int npad = worldPatchManager.active_patch.padding;
            for (int i = -npad; i < npad; i++)
            {
                for (int j = -npad; j < npad; j++)
                {
                    worldPatchManager.active_patch
                        .get(i, j).GetComponent<PatchHex>().getAdjList(click_pos, avatar.transform, adjlist);
                    if (adjlist.Count > 0) break;
                }
                if (adjlist.Count > 0) break;
            }

            if (adjlist.Count > 0)
            {
                onClickVec3.Invoke(adjlist[0].position);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            onClickVec3.Invoke(click_pos);
        }

        onClickPosListener.Invoke(
            new ClickPosEvent(click_pos + worldPatchManager.offset_in_map));
    }

    IEnumerator Twinkle()
    {
        while (twinkle_downcnt > 0) {
            float scale = 1 - Mathf.Cos(
                4 * Mathf.PI * (twinkle_duration - twinkle_downcnt)
                / twinkle_duration);
            scale *= twinkle_radius;
            transform.localScale = Vector3.right * scale
                                 + Vector3.up * scale
                                 + Vector3.forward * scale;
            twinkle_downcnt -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
