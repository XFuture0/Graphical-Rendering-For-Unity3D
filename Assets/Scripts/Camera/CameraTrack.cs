using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public GameObject Target;
    private void FixedUpdate()
    {
        transform.position = Target.transform.position;
        transform.rotation = Target.transform.rotation;
    }
}
