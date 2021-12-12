using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObjectBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnSnapped()
    {
        Debug.Log("BaseSnapped");
    }

    public virtual void OnUnsnapped()
    {
        Debug.Log("BaseUnsnapped");
    }
}
