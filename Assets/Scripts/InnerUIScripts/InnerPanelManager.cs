/*
 * @Author: jadizhang, zc-BEAR 
 * @Date: 2021-11-27 17:11:34 
 * @Last Modified by:   jadizhang 
 * @Last Modified time: 2021-11-27 17:11:34 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InnerPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text ContentText = GameObject.Find("EntrySettingShow").transform
                                                .GetChild(0).GetChild(1).GetComponent<Text>();
        ContentText.text = string.Format("RunMode: {0:G}\nBehaviour: {1:G}\nModule: {2:D}",
                        EntrySetting.Instance.runMode,
                        EntrySetting.Instance.behaviour,
                        EntrySetting.Instance.module);

        Debug.Log(EntrySetting.Instance.runMode);
        Debug.Log(EntrySetting.Instance.behaviour);
        Debug.Log(EntrySetting.Instance.module);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
