using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
public class Images {
    public string path { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class Texts {
    public string path { get; set; }
    public int x { get; set; }
    public int y { get; set; }
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
    private Pages obj;
    private int curPage = 0;
    // Start is called before the first frame update
    void Start()
    {
        obj = ReadJson("/Data/MainScreenData.json");
    }

    public void loadScreen(int index) {
        Debug.Log(obj.pages[index].title);
        transform.GetComponentInChildren<Text>().text = obj.pages[index].title;
    }

    private Pages ReadJson(string str)
    {
        StreamReader StreamReader = new StreamReader(Application.dataPath + str);
        JsonReader js = new JsonReader(StreamReader);
        return JsonMapper.ToObject<Pages>(js);
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
