using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
public class Images {
    public string path { get; set; }
    public int x { get; set; } = -150;
    public int y { get; set; } = 0;
    public int width { get; set; } = 300;
    public int height { get; set; } = 400;
}

public class Texts {
    public string path { get; set; }
    public int x { get; set; } = 150;
    public int y { get; set; } = 0;
    public int width { get; set; } = 300;
    public int height { get; set; } = 400;
}

public class SinglePage {
    public string title { get; set; }
    public List<Images> images { get; set; }
    public List<Texts> texts { get; set; }
}
public class Pages {
    public List<SinglePage> pages { get; set; }
}
public class MainScreenScript : MonoBehaviour
{
    public GameObject txtTemplate;
    public GameObject imgTemplate;

    private Pages obj;
    private int curPage = 0;
    private List<GameObject> txts;
    private List<GameObject> imgs;
    // Start is called before the first frame update
    void Start()
    {
        obj = ReadJson("/Data/MainScreenData.json");
        txts = new List<GameObject>();
        imgs = new List<GameObject>();
    }

    public void loadScreen(int index) {
        foreach(var item in txts) {
            Destroy(item);
        }
        foreach(var item in imgs) {
            Destroy(item);
        }
        foreach(var item in GetComponentsInChildren<Text>()) {
            if(item.name == "Title") {
                item.text = obj.pages[index].title;
            }
        }
        foreach(var text in obj.pages[index].texts) {
            GameObject tmpTxt = Instantiate(txtTemplate, transform) as GameObject;
            tmpTxt.GetComponent<Text>().text = ReadText(text.path);
            tmpTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(text.x, text.y);
            tmpTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(text.width, text.height);
            txts.Add(tmpTxt);
        }
        foreach(var image in obj.pages[index].images) {
            GameObject tmpImg = Instantiate(imgTemplate, transform) as GameObject;
            tmpImg.GetComponent<Image>().sprite = ReadImage(image.path);
            tmpImg.GetComponent<RectTransform>().anchoredPosition = new Vector2(image.x, image.y);
            tmpImg.GetComponent<RectTransform>().sizeDelta = new Vector2(image.width, image.height);
            imgs.Add(tmpImg);
        }
    }

    private Pages ReadJson(string str) {
        StreamReader StreamReader = new StreamReader(Application.dataPath + str);
        JsonReader js = new JsonReader(StreamReader);
        return JsonMapper.ToObject<Pages>(js);
    }

    private string ReadText(string str) {
        string s = "";
        StreamReader StreamReader = new StreamReader(Application.dataPath + str);
        string line;
        while((line = StreamReader.ReadLine()) != null) {
            s += line + '\n';
        }
        return s;
    }

    private Sprite ReadImage(string str) {
        Sprite sp = Resources.Load(str, typeof(Sprite)) as Sprite;
        Debug.Log(sp);
        return sp;
    }

    public void onClickPageUp() {
        curPage = (curPage + 1 + obj.pages.Count) % obj.pages.Count;
        loadScreen(curPage);
    }
    public void onClickPageDown() {
        curPage = (curPage - 1 + obj.pages.Count) % obj.pages.Count;
        loadScreen(curPage);
    }
}
