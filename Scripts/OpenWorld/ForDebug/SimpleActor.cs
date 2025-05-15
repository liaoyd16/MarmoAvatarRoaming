using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleActor : MonoBehaviour
{
 [SerializeField] Vector3 speed;
    [SerializeField] float max_speed;
    [SerializeField] float acceleration = 3f, friction = 1f;
    static readonly float EPS = 1e-1f;

    void Start()
    {
        speed = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float velodelta = 0, fricdelta = 0;
        if (Input.GetKey(KeyCode.UpArrow)) velodelta += Time.deltaTime * acceleration;
        if (Input.GetKey(KeyCode.DownArrow)) velodelta -= Time.deltaTime * acceleration;
        if (speed.x > 0) fricdelta -= Time.deltaTime * friction;
        else if (speed.x < 0) fricdelta += Time.deltaTime * friction;

        // speed.x += velodelta + fricdelta;
        if (velodelta == 0f)
        {
            if (Mathf.Sign(speed.x) * Mathf.Sign(fricdelta + speed.x) <= 0) speed.x = 0;
            else speed.x += fricdelta;
        }
        else speed.x += velodelta;

        velodelta = 0; fricdelta = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) velodelta += Time.deltaTime * acceleration;
        if (Input.GetKey(KeyCode.RightArrow)) velodelta -= Time.deltaTime * acceleration;
        if (speed.z > 0) fricdelta -= Time.deltaTime * friction;
        else if (speed.z < 0) fricdelta += Time.deltaTime * friction;

        if (velodelta == 0f)
        {
            if (Mathf.Sign(speed.z) * Mathf.Sign(fricdelta + speed.z) <= 0) speed.z = 0;
            else speed.z += fricdelta;
        }
        else speed.z += velodelta;

        if (speed.magnitude > max_speed) speed = max_speed * speed.normalized;
    }

    void FixedUpdate()
    {
        transform.position += speed * Time.deltaTime;
        if (speed.magnitude > EPS)
            transform.LookAt(speed + transform.position);
    }
}
