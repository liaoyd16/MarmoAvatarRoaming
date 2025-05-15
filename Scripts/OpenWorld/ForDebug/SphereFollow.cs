using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereFollow : MonoBehaviour
{
    [SerializeField] GameObject birdview_obj;
    BirdView birdview;
    void Start()
    {
        birdview = birdview_obj.GetComponent<BirdView>();
    }

    void Update()
    {
        transform.position = birdview.click_pt;
    }
}
