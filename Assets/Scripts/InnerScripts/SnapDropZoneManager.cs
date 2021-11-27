/*
 * @Author: jadizhang, zc-BEAR 
 * @Date: 2021-11-27 17:11:25 
 * @Last Modified by:   jadizhang 
 * @Last Modified time: 2021-11-27 17:11:25 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapDropZoneManager : MonoBehaviour
{
    public void OnComponentEntered(GameObject Component){
        Component.GetComponent<Rigidbody>().useGravity = false;
        Debug.Log("OnComponentEntered");
    }

    public void OnComponentExited(GameObject Component){
        Component.GetComponent<Rigidbody>().useGravity = true;
        Debug.Log("OnComponentExited");
    }
    public void OnComponentSnapped(){
        Debug.Log("OnComponentSnapped");
    }

    public void OnComponentUnsnapped(){
        Debug.Log("OnComponentSnapped");
    }
}
