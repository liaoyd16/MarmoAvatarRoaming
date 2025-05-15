using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonView : MonoBehaviour
{
    [SerializeField] GameObject avatar_to_follow;

    [SerializeField] Vector3 backoff;
    [SerializeField] Quaternion base_rotation;

    void Update()
    {
        transform.rotation = avatar_to_follow.transform.rotation * base_rotation;
        transform.position = avatar_to_follow.transform.rotation*backoff + avatar_to_follow.transform.position;
    }
}
