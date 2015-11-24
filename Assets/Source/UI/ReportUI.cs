﻿using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficReport.Util;
using UnityEngine;

namespace TrafficReport.Assets.Source.UI
{

    

    delegate void TypeHighlightEvent(String type);

   

    class ReportUI : UIPanel
    {

        static Color32 grey = new Color32(128, 128, 128, 128);
        static Color32 textColor = new Color32(206, 248, 0, 255);
        static Color32 hoverColor = new Color32(0, 206, 248, 255);

        UILabel usageText;

        UIPanel helpBg;

        BreakdownPanel reportBreakDown;
        BreakdownPanel highlightBreakDown;

        public event TypeHighlightEvent eventHighlightType;

        public static ReportUI Create()
        {
            GameObject go = new GameObject("TrafficReportUI");
            
            ReportUI reportUI = go.AddComponent<ReportUI>();

            UIView.GetAView().AttachUIComponent(go);


            reportUI.absolutePosition = new Vector3(0, 0, 0);            
            return reportUI;
        }

        public override void Awake()
        {
            size = new Vector2(5, 5);
            anchor = UIAnchorStyle.Top;

            helpBg = AddUIComponent<UIPanel>();
            helpBg.backgroundSprite = "GenericPanel";
            
            helpBg.color = new Color32(0, 0, 120, 200);
            helpBg.area = new Vector4(10, 65, 230, 70);
            

            usageText =  helpBg.AddUIComponent<UILabel>();
            usageText.relativePosition = new Vector2(5, 5);
            usageText.textScale = 0.6f;
            usageText.text =
				"左クリックで両方の車線の交通情報を表示\n" +
				"右クリック/Shift+右クリックで\n" +
				"片方の車線のみ表示\n" +
				"他の道路にマウスオーバーで\n" +
				"そこを通る経路の交通情報を表示\n";                


            reportBreakDown = AddUIComponent<BreakdownPanel>();
            reportBreakDown.title.text = "選択した区間";
            reportBreakDown.title.tooltip = "選択した道路区間を通る交通の内訳";
            reportBreakDown.isVisible = false;
            reportBreakDown.relativePosition = new Vector2(10, 150);
            reportBreakDown.eventHighlightType += (String s) =>
            {
                if (eventHighlightType != null)
                    eventHighlightType(s);
            };

            highlightBreakDown = AddUIComponent<BreakdownPanel>();
            highlightBreakDown.title.text = "ハイライトされた区間";
            highlightBreakDown.isVisible = false;
            highlightBreakDown.relativePosition = new Vector2(220, 150);

            base.Awake();
        }


        public void SetSelectedData(Dictionary<string,int> counts) {
            if(counts == null) {
                reportBreakDown.isVisible = false;
            }else {
                reportBreakDown.SetValues(counts);
                reportBreakDown.isVisible = true;
                
            }
        }

        public void SetHighlightData(Dictionary<String,int> counts, int totalCount)
        {
            if (counts == null || totalCount == 0 || reportBreakDown.isVisible ==  false)
            {
                highlightBreakDown.isVisible = false;
            }
            else
            {
                if (reportBreakDown.isVisible)
                {
                    highlightBreakDown.SetValues(counts);
                    highlightBreakDown.isVisible = true;
                }
            }
        }
    }
}
