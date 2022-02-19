using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class RecordError {
    public int index { get; set; }
    public string description { get; set; }
    public double punishment { get; set; }
}
public class RecordSingleSection {
    public string title { get; set; }
    public int totalScore { get; set; }
    public List<string> records { get; set; }
    public List<RecordError> errors { get; set; }
}
public class RecordSinglePage {
    public string title { get; set; }
    public List<RecordSingleSection> sections { get; set; }
}
public class RecordPages {
    public List<RecordSinglePage> pages { get; set; }
}

public class RecordTableScript : MonoBehaviour
{

    public GameObject Content;
    public GameObject PageTitle;
    public GameObject RecordTitle;
    public GameObject RecordContent;
    public GameObject RecordError;
    public GameObject ScoreContent;
    private RecordPages obj;
    private int maxPageIndex;
    private List<GameObject> records;

    // Start is called before the first frame update
    void Start()
    {
        obj = ReadJson("/Data/RecordStepsData.json");
        maxPageIndex = obj.pages.Count;
        records = new List<GameObject>();
        loadScreen(0);
    }

    private RecordPages ReadJson(string str) {
        StreamReader StreamReader = new StreamReader(Application.dataPath + str);
        JsonReader js = new JsonReader(StreamReader);
        return JsonMapper.ToObject<RecordPages>(js);
    }

    // 暴露的接口

    // ！！请在load前先写好error信息！
    public void insertError(int pageIndex, int sectionIndex, int stepIndex, string description, double punishment) {
        RecordError err = new RecordError();
        err.index = stepIndex;
        err.description = description;
        err.punishment = punishment;
        obj.pages[pageIndex].sections[sectionIndex].errors.Add(err);
    }
    public void loadScreen(int index) {
        // initialize
        foreach(var item in records) {
            Destroy(item);
        }

        PageTitle.GetComponent<Text>().text = obj.pages[index].title;
        // 用于计数现在多少条，起始一条为offset
        int recordCount = 1;
        double score = 100;
        var sections = obj.pages[index].sections;
        foreach(var section in sections) {
            // section的标题
            GameObject tmpTitle = Instantiate(RecordTitle, Content.transform) as GameObject;
            foreach(var item in tmpTitle.GetComponentsInChildren<Text>()) {
                if(item.name == "Title") {
                    item.text = section.title;
                }
                if(item.name == "TotalScore") {
                    item.text = "(" + section.totalScore + "分)";
                }  
            }
            foreach(Transform t in tmpTitle.GetComponentsInChildren<Transform>()) {
                if(t.name == "TotalScore") {
                    t.GetComponent<RectTransform>().anchoredPosition = new Vector2(70 + section.title.Length * 18, t.GetComponent<RectTransform>().anchoredPosition.y);
                }  
            }
            tmpTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
            records.Add(tmpTitle);
            recordCount++;

            // section的内容
            foreach(var content in section.records) {
                GameObject tmpContent = Instantiate(RecordContent, Content.transform) as GameObject;
                tmpContent.GetComponentInChildren<Text>().text = content;
                tmpContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                records.Add(tmpContent);
                recordCount++;
            }

            // section的error
            foreach(var err in section.errors) {
                GameObject tmpError = Instantiate(RecordError, Content.transform) as GameObject;
                foreach(var item in tmpError.GetComponentsInChildren<Text>()) {
                    if(item.name == "Title") {
                        item.text = err.index.ToString() + ". " + err.description;
                    }
                    if(item.name == "Score") {
                        item.text = err.punishment.ToString();
                        score += err.punishment;
                    }  
                }
                tmpError.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                records.Add(tmpError);
                recordCount++;
            }
        }
        // 总成绩
        ScoreContent.GetComponent<Text>().text = score.ToString();
    }
}
