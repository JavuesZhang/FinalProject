using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhiJiaRotator : MonoBehaviour
{
    public Transform rotationRef;

    public bool isEngine;
    
    private Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.localEulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float refAngle = rotationRef.localEulerAngles.z;
        transform.localEulerAngles = new Vector3(isEngine ? -refAngle : refAngle, origin.y, origin.z);
    }
}
