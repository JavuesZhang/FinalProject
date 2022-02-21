﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using Doozy.Engine.UI;
using Framework;
using Fxb.CPTTS;
using Doozy.Engine;


namespace Fxb.CMSVR
{
    public class RecordTableScript : MonoBehaviour
    {

        public GameObject Content;
        public GameObject PageTitle;
        public GameObject RecordTitle;
        public GameObject RecordContent;
        public GameObject RecordError;
        public GameObject ScoreContent;
        private int maxPageIndex;
        private List<GameObject> records;
        IRecordModel recordModel;
        ITaskModel taskModel;
        private void RecordMessage(PrepareTaskMessage msg) {
            recordModel = World.Get<IRecordModel>();
            taskModel = World.Get<ITaskModel>();
            loadScreen();
        }
        void Awake() {
            Message.AddListener<PrepareTaskMessage>(RecordMessage);
        }
        void Start() {
            records = new List<GameObject>();
        }
        // 暴露的接口

        // // ！！请在load前先写好error信息！
        // public void insertError(int pageIndex, int sectionIndex, int stepIndex, string description, double punishment) {
        //     RecordError err = new RecordError();
        //     err.index = stepIndex;
        //     err.description = description;
        //     err.punishment = punishment;
        //     obj.pages[pageIndex].sections[sectionIndex].errors.Add(err);
        // }
        public void loadScreen() {
            // initialize
            foreach(var item in records) {
                Destroy(item);
            }
            var curPage = taskModel.GetData()[0];
            PageTitle.GetComponent<Text>().text = curPage.taskTitle;
            // 用于计数现在多少条，起始一条为offset
            int recordCount = 1;
            double score = 100;
            var stepGroups = curPage.stepGroups;
            foreach(var stepGroup in stepGroups) {
                // section的标题
                GameObject tmpTitle = Instantiate(RecordTitle, Content.transform) as GameObject;
                foreach(var item in tmpTitle.GetComponentsInChildren<Text>()) {
                    if(item.name == "Title") {
                        item.text = taskModel.GetStepGroupDescription(stepGroup.id);
                    }
                    if(item.name == "TotalScore") {
                        item.text = "(" + stepGroup.score + "分)";
                    }  
                }
                foreach(Transform t in tmpTitle.GetComponentsInChildren<Transform>()) {
                    if(t.name == "TotalScore") {
                        t.GetComponent<RectTransform>().anchoredPosition = new Vector2(70 + taskModel.GetStepGroupDescription(stepGroup.id).Length * 18, t.GetComponent<RectTransform>().anchoredPosition.y);
                    }  
                }
                tmpTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                records.Add(tmpTitle);
                recordCount++;

                // section的内容
                // Debug.Log("gsd" + taskModel.GetStepGroupDescription(stepGroup.id));
                foreach(var item in taskModel.GetChildStepIDs(stepGroup.id)) {
                    GameObject tmpContent = Instantiate(RecordContent, Content.transform) as GameObject;
                    tmpContent.GetComponentInChildren<Text>().text = recordModel.FindRecord(item).Title;
                    tmpContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                    records.Add(tmpContent);
                    recordCount++;
                }


                // section的error
                foreach(var record in taskModel.GetChildStepIDs(stepGroup.id)) {
                    // GameObject tmpError = Instantiate(RecordError, Content.transform) as GameObject;
                    // foreach(var item in tmpError.GetComponentsInChildren<Text>()) {
                    //     if(item.name == "Title") {
                    //         item.text = err.index.ToString() + ". " + err.description;
                    //     }
                    //     if(item.name == "Score") {
                    //         item.text = err.punishment.ToString();
                    //         score += err.punishment;
                    //     }  
                    // }
                    // tmpError.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                    // records.Add(tmpError);
                    // recordCount++;
                    Debug.Log("gsd" + recordModel.GetRecordAllErrors(record));
                }
            }
            // 总成绩
            ScoreContent.GetComponent<Text>().text = score.ToString();
        }
    
    }
}