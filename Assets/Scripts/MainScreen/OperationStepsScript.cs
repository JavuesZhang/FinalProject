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
    public GameObject UpBtn;
    public GameObject DownBtn;
    public GameObject Step;
    public GameObject Content;
    private OperationPages obj;
    private List<GameObject> steps;

    // isOperate表示给的UI设计图的右图。默认为false，左图
    public bool isOperate = false;

    private bool lastDone = false;
    
    private int curPage = 0;

    private int doneCount = 0;

    private int maxPageIndex;

    private Color DarkGray = new Color(255, 0, 0);
    private Color Black = new Color(0, 0, 0);
    // Start is called before the first frame update
    void Start() {
        obj = ReadJson("/Data/OperationStepsData.json");
        maxPageIndex = obj.pages.Count;
        steps = new List<GameObject>();
        if(maxPageIndex > 0) {
            loadScreen(0);
        }
        checkInteractable();
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
        for (int i = 0; i < obj.pages[index].steps.Count; i++) {
            GameObject tmpStep = Instantiate(Step, Content.transform) as GameObject;
            tmpStep.GetComponentInChildren<Text>().text = (i+1).ToString() + ". " + obj.pages[index].steps[i].name;
            Debug.Log(obj.pages[index].steps[i].name);
            checkStepState(tmpStep, obj.pages[index].steps[i].done);
            tmpStep.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 - (50) * i);
            steps.Add(tmpStep);
        }

        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x, 50 * obj.pages[index].steps.Count);
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

        // 若isOperate，隐藏上下按钮
        if(isOperate) {
            UpBtn.SetActive(false);
            DownBtn.SetActive(false);
        } else {
            UpBtn.SetActive(true);
            DownBtn.SetActive(true);
        }
    }

    private void checkStepState(GameObject step, bool done) {
        var imgColor = step.GetComponentInChildren<Image>().color;
        if(isOperate) {
            if(done) {
                doneCount++;
                lastDone = true;
                // 颜色设置为灰色，隐藏√
                step.GetComponentInChildren<Text>().color = DarkGray;
                step.GetComponentInChildren<Image>().color = new Color(imgColor.r, imgColor.g, imgColor.b,0);
            } else {
                // 颜色设置为黑色，显示√，若上一个做完了，则当前step字体变大
                step.GetComponentInChildren<Text>().color = Black;
                if(lastDone) {
                   step.GetComponentInChildren<Text>().fontSize = (int)((double)step.GetComponentInChildren<Text>().fontSize * 1.5);
                }
                step.GetComponentInChildren<Image>().color = new Color(imgColor.r, imgColor.g, imgColor.b,255);
                lastDone = false;
            }
        } else {
            // 颜色设置为黑色，隐藏√
            step.GetComponentInChildren<Text>().color = Black;
            step.GetComponentInChildren<Image>().color = new Color(imgColor.r, imgColor.g, imgColor.b,0);
        }

    }
    void checkInteractable() {
        if(curPage > 0) {
            UpBtn.GetComponent<Button>().interactable = true;
        } else {
            UpBtn.GetComponent<Button>().interactable = false;
        }
        if(curPage < maxPageIndex - 1) {
            DownBtn.GetComponent<Button>().interactable = true;
        } else {
            DownBtn.GetComponent<Button>().interactable = false;
        }
    }
    public void onClickPageUp() {
        curPage = (curPage + 1 + maxPageIndex) % maxPageIndex;
        loadScreen(curPage);
        checkInteractable();
    }
    public void onClickPageDown() {
        curPage = (curPage - 1 + maxPageIndex) % maxPageIndex;
        loadScreen(curPage);
        checkInteractable();
    }
}
