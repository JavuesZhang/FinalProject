﻿using Doozy.Engine.UI;
using Framework;
using Fxb.CMSVR;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CPTTS
{
    public class TaskMainMenuView : PadViewBase
    {
        public Button[] menuMapBtns;

        public UIView taskView;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            for (var i = 0; i < menuMapBtns.Length; i++)
            {
                var btn = menuMapBtns[i];

                btn.onClick.AddListener(() =>
                {
                    EntrySetting.Instance.behaviour = (Enums.Behaviour)i;
                    taskView.Show();
                });
            }

            doozyView.Show(true);
        }
    }
}