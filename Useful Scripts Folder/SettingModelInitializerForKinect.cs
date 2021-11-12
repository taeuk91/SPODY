using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 키넥트(C2) 일 경우 게임 시작시 설정되는 세팅모델을 C드라이브의 SpodyConfig에서 받아와서 설정한다.
public class SettingModelInitializerForKinect : Singleton<SettingModelInitializerForKinect>
{
    private static bool isDataLoaded = false;

#if KINECT
    private const string SETTING_MODEL_NAME = "SettingModel.json";
#elif ASTRA
    private const string SETTING_MODEL_NAME = "SettingModel_.json";
#endif

    public void Init()
    {
        
    }
    
    private void Awake()
    {
        if (isDataLoaded)
        {
            return;
        }

        isDataLoaded = true;
            
        string json = FileInOutput.readStringFromFile("C://SpodyConfig//" + SETTING_MODEL_NAME);

        if (json != null)
        {
            FileInOutput.writeStringToFile(json, SETTING_MODEL_NAME);
            
            string licenseJson = FileInOutput.readStringFromFile("C://SpodyConfig//license.txt");
            FileInOutput.writeStringToFile(licenseJson, "license.txt");
            
            string idJson = FileInOutput.readStringFromFile("C://SpodyConfig//id.txt");
            FileInOutput.writeStringToFile(idJson, "id.txt");
            
            string diaryJson = FileInOutput.readStringFromFile("C://SpodyConfig//Diary.txt");
            FileInOutput.writeStringToFile(diaryJson, "Diary.json");
        }
        else
        {
            Debug.Log("json is null");
        }
    }

    public void SaveSetting()
    {
        string json = FileInOutput.readStringFromFile(SETTING_MODEL_NAME);
        FileInOutput.writeStringToFile(json, "C://SpodyConfig//" + SETTING_MODEL_NAME);
    }
}
