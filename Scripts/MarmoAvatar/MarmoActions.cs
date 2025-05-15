using System;
using System.Collections.Generic;
using UnityEngine;

public class MarmoActions : MonoBehaviour
{
    // misc
    static float DIST_EPS = 5e-2f;
    [SerializeField]
    AnimationClip turnleft_clip,
                                   turnright_clip,
                                   leapl_clip,
                                   leapr_clip;
    float turn_duration, leap_duration;

    // animator stuff 
    Animator ac;
    Dictionary<string, int> layer_of = new Dictionary<string, int>() { { "Main", 0 } };

    // variable for turning
    float angular_speed, angle_remaining;

    // variable for leaping
    [SerializeField] float raw_leap_dist = 6, raw_leap_velo;
    Vector3 spd_leap, dest;


    void Start()
    {
        ac = GetComponent<Animator>();
        turn_duration = Mathf.Min(turnleft_clip.length, turnright_clip.length);
        leap_duration = Mathf.Min(leapl_clip.length, leapr_clip.length);

        angle_remaining = 0f;

        spd_leap = Vector3.zero;
        dest = transform.position;

        raw_leap_velo = raw_leap_dist / leap_duration;
    }

    public void onNewDestination(Vector3 xyz)
    {
        // todo: yield until returned to state "idle"

        dest = xyz; // might not be true if not go in straight line

        // calculate angle, number of leaps
        Vector3 displace = new Vector3(dest.x, transform.position.y, dest.z) - transform.position;

        // angle: consider both displacement and self orientation
        float orient_angle = transform.eulerAngles.y;
        Vector3 displace_rotate = Quaternion.AngleAxis(-orient_angle, Vector3.up) * displace;
        angle_remaining = (float)Math.Atan2(displace_rotate.x, displace_rotate.z) * Mathf.Rad2Deg;

        ac.SetFloat("turn_angle", angle_remaining);

        // rotating speed, time
        angular_speed = angle_remaining / turn_duration;

        // set number of leaps & speed vector for fixed update
        spd_leap = getNextLeapSpeed();
        if (spd_leap.magnitude > 0)
        {
            ac.SetInteger("num_leaps", 1);
        }
        else
        {
            ac.SetInteger("num_leaps", 0);
        }
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = ac.GetCurrentAnimatorStateInfo(layer_of["Main"]);
        if (info.IsName("turnleft") || info.IsName("turnright"))
        {
            float angle_delta = angular_speed * Time.deltaTime;
            if (angle_remaining * (angle_remaining - angle_delta) > 0) // angle_remaining would not change sign
            {
                transform.Rotate(Vector3.up, angle_delta);
                angle_remaining -= angle_delta;
            }
        }
        else if (info.IsName("leapl") || info.IsName("leapr"))
        {
            Vector3 pos_delta = spd_leap * Time.deltaTime;
            if ((transform.position - dest).magnitude > (transform.position + pos_delta - dest).magnitude)
            {
                transform.position += pos_delta;
            }
            else
            {
                ac.SetInteger("num_leaps", 0);
            }
        }
    }

    void Update()
    {
        if (ac.GetCurrentAnimatorStateInfo(layer_of["Main"]).IsName("temporary"))
        {
            spd_leap = getNextLeapSpeed();
        }
    }

    Vector3 getNextLeapSpeed()
    {
        Vector3 displace = dest - transform.position;

        if (displace.magnitude > DIST_EPS)
        {
            return raw_leap_velo * displace.normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
