using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.ArtificialBased;

public class TaotongSnapObject : SnapObjectBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnSnapped()
    {
        Debug.Log("TaotongSnapped");

        
        transform.gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        foreach (Collider collider in transform.gameObject.GetComponentsInChildren<SphereCollider>())
        {
            collider.enabled = true;
        }
        // transform.gameObject.GetComponentInChildren<VRTK_ArtificialRotator>().enabled = true;
        // transform.gameObject.SetActive(true);
        DestroyImmediate(transform.gameObject.GetComponent<VRTK_InteractableObject>());
        transform.gameObject.AddComponent<VRTK_ArtificialRotator>();
        ChangeHierarchy();
    }

    protected void ChangeHierarchy()
    {
        Transform luosiTransform = transform.parent.parent;
        transform.SetParent(luosiTransform.parent);
        foreach (Transform t in luosiTransform)
        {
            Destroy(t.gameObject);
        }
        luosiTransform.SetParent(transform);
    }
}
