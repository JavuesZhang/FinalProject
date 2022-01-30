using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class OperationStep {
    public string name { get; set; }
    public bool done { get; set; }
}
public class OperationSinglePage {
    public string title { get; set; }
    public string description { get; set;}
    public List<OperationStep> steps { get; set; }
}
public class OperationPages {
    public List<OperationSinglePage> pages { get; set; }
}
public class OperationStepsScript : MonoBehaviour
{
    public GameObject Step;
    public GameObject Content;
    private OperationPages obj;
    private List<GameObject> steps;
    
    private int curPage = 0;

    private int doneCount = 0;

    private int maxPageIndex;
    // Start is called before the first frame update
    void Start() {
        obj = ReadJson("/Data/OperationStepsData.json");
        maxPageIndex = obj.pages.Count;
        steps = new List<GameObject>();
    }

    private OperationPages ReadJson(string str) {
        StreamReader StreamReader = new StreamReader(Application.dataPath + str);
        JsonReader js = new JsonReader(StreamReader);
        return JsonMapper.ToObject<OperationPages>(js);
    }

    public void loadScreen(int index) {
        // initialize
        foreach(var item in steps) {
            Destroy(item);
        }
        doneCount = 0;

        // load new steps
        for (int i = 0; i < obj.pages[index].steps.Count;i++) {
            GameObject tmpStep = Instantiate(Step, Content.transform) as GameObject;
            tmpStep.GetComponentInChildren<Text>().text = (i+1).ToString() + ". " + obj.pages[index].steps[i].name;
            if(obj.pages[index].steps[i].done) {
                doneCount++;
            } else {
                var tmpTxtColor = tmpStep.GetComponentInChildren<Text>().color;
                tmpStep.GetComponentInChildren<Image>().color = new Color(tmpTxtColor.r, tmpTxtColor.g, tmpTxtColor.b,(float)0.5);
                var tmpImgColor = tmpStep.GetComponentInChildren<Image>().color;
                tmpStep.GetComponentInChildren<Image>().color = new Color(tmpImgColor.r, tmpImgColor.g, tmpImgColor.b,0);
            }
            tmpStep.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 - (50) * i);
            steps.Add(tmpStep);
        }

        foreach(var item in GetComponentsInChildren<Text>()) {
            if(item.name == "Title") {
                item.text = obj.pages[index].title;
            }
            if(item.name == "Description") {
                item.text = obj.pages[index].description;
            }
            if(item.name == "DoneCount") {
                item.text = doneCount.ToString() + "/" + obj.pages[index].steps.Count.ToString();
            }
        }
    }

    public void onClickPageUp() {
        curPage = (curPage + 1 + maxPageIndex) % maxPageIndex;
        loadScreen(curPage);
    }
    public void onClickPageDown() {
        curPage = (curPage - 1 + maxPageIndex) % maxPageIndex;
        loadScreen(curPage);
    }
}
