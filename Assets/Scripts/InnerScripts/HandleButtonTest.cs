using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class HandleButtonTest : MonoBehaviour
{
    
    public VRTK_ControllerEvents leftController;
    public VRTK_ControllerEvents rightController;

    public Transform parentTransform;

    public GameObject taotongPrefab;

    public GameObject banshouPrefab;

    public Collider collider;

    protected int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        RegisterEvents(leftController);
        RegisterEvents(rightController);
    }


    protected virtual void RegisterEvents(VRTK_ControllerEvents events)
    {
        if (events != null)
        {
            events.TriggerClicked += GetTaotong;
        }
    }

    protected virtual void GetTaotong(object sender, ControllerInteractionEventArgs e)
    {
        GameObject prefab = null;
        for (int i = parentTransform.childCount - 1; i > 2; i--) 
        {
            Destroy(parentTransform.GetChild(i).gameObject);
        }
        switch (index)
        {
            case 0:
                collider.enabled = false;
                prefab = taotongPrefab;
                break;
            case 1:
                collider.enabled = false;
                prefab = banshouPrefab;
                break;
            case 2:
                collider.enabled = true;
                prefab = null;
                break;
        }
        if(prefab != null)
        {
            GameObject tool = Instantiate(prefab, parentTransform) as GameObject;
            tool.transform.localPosition = Vector3.zero;
        }
        index = (index + 2) % 3;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
