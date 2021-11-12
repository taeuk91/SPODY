using System;
using UnityEngine;
using UnityEngine.UI;


    public class UpDownTimer : MonoBehaviour
    {
        public static UpDownTimer instance;
        public bool isTimerOn = false;
        public bool isUpCounter = true;
        public int secondsSet = 0;
        public int Minute { get; set; }
        public int Second { get; set; }
        private int minuite;
        private float seconds;
        int sec;
        public int totalSec = 0;

        private string timeText;
        private Text text;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            
            text = transform.GetChild(0).GetComponent<Text>();

            if (isUpCounter)
            {
                minuite = 0;
                seconds = 0;
            }
            else
            {
                minuite = secondsSet / 60; // 130초 -> 130/60 = 2
                seconds = secondsSet % 60; // 130%60 = 10
            }
            
            timeText = minuite.ToString("00") + ":" + sec.ToString("00");
        }


        private void Update()
        {
            if(!isTimerOn)
                return;
            
            if (isUpCounter)
            {
                seconds += Time.deltaTime * 1.05f;
                
                if (seconds >=  60.0f)
                {
                    seconds -= 60.0f;
                    minuite ++;
                }
            }
            else
            {
                seconds -= Time.deltaTime * 1.05f;

                if (seconds <=  0f)
                {
                    seconds += 60.0f;
                    if(minuite > 0)
                        minuite --;
                }
                
                if ((int)seconds == 0 && minuite == 0)
                {
                    text.text = "00:00";
                    isTimerOn = false;
                }
            }
            

            sec = (int)seconds;

            timeText = minuite.ToString("00") + ":" + sec.ToString("00");
            text.text = timeText;
            
            Minute = minuite;
            Second = sec;

            totalSec = minuite * 60 + sec;
        }
    }


