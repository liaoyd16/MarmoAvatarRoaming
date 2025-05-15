using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ClickPos : MonoBehaviour
{
    [SerializeField] float twinkle_duration;
    [SerializeField] float twinkle_radius;
    float twinkle_downcnt;
    public UnityEvent<TaskEvent> onClickPosListener;

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        twinkle_downcnt = 0f;
    }

    public void onClickPos(Vector3 click_pos)
    {
        transform.position = click_pos;
        twinkle_downcnt = twinkle_duration;
        StartCoroutine(Twinkle());
        onClickPosListener.Invoke(new ClickPosEvent(click_pos));
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
