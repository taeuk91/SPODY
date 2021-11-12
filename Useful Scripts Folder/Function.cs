using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using DG.Tweening;
using Util.Astra_;
using log4net;

namespace Util.Usefull
{
    public struct Function
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Function));

        /// <param name="length">배열 총 길이</param>
        /// <param name="min">작은 수!</param>
        /// <param name="max">가장 큰 수!</param>
        public static int[] GetRandomInt(int length, int min, int max)
        {
            int[] randomArray = new int[length];
            bool isSame;

            for (int i = 0; i < length; ++i)
            {
                while (true)
                {
                    randomArray[i] = UnityEngine.Random.Range(min, max);
                    isSame = false;

                    for (int j = 0; j < i; ++j)
                    {
                        if (randomArray[j].Equals(randomArray[i]))
                        {
                            isSame = true;
                            break;
                        }
                    }
                    if (!isSame) break;
                }
            }
            return randomArray;
        }

        /// <param name="_array">정렬 할 정수 배열</param>
        public static int[] BubbleSort(int[] _array)
        {
            int[] array = new int[_array.Length];
            Array.Copy(_array, array, _array.Length);

            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = 0; j < array.Length - 1 - i; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        int hold = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = hold;
                    }
                }
            }
            return array;
        }

        /// <param name="list">섞을 리스트 변수, ref로 넘겨줄 것</param>
        public static void ShuffleList<T>(ref List<T> list)
        {
            int random1;
            int random2;

            T tmp;

            for (int i = 0; i < list.Count; ++i)
            {
                random1 = UnityEngine.Random.Range(0, list.Count);
                random2 = UnityEngine.Random.Range(0, list.Count);

                tmp = list[random1];
                list[random1] = list[random2];
                list[random2] = tmp;
            }
        }

        //210316 - WooSon : 배열 셔플 메소드 추가
        /// <param name="array">섞을 배열 변수, ref로 넘겨줄 것</param>
        public static void ShuffleArray<T>(ref T[] array)
        {
            int random1;
            int random2;

            T tmp;

            for (int index = 0; index < array.Length; ++index)
            {
                random1 = UnityEngine.Random.Range(0, array.Length);
                random2 = UnityEngine.Random.Range(0, array.Length);

                tmp = array[random1];
                array[random1] = array[random2];
                array[random2] = tmp;
            }
        }
        //210316 - WooSon -- 

        /// <param name="sentences">글자들이 들어갈 Queue타입 자료구조</param>
        /// <param name="dialogue_text">화면에 보여줄 Text타입 변수</param>
        /// <param name="dialogue">Queue에 입력할 "Dialogue"타입</param>
        public static void InitDialogue(Queue<string> sentences, Text dialogue_text, Dialogue dialogue)
        {
            sentences.Clear();
            dialogue_text.text = "";


            foreach (var sentence in dialogue.sentences)
                sentences.Enqueue(sentence);

            //print(sentences.Peek());
        }

        /// <param name="sentences">화면에 띄울 글자들이 들어있는 Queue타입 자료구조</param>
        /// <param name="dialogue_text">화면에 보여줄 Text타입 변수</param>
        public static int DisplayDialogue(Queue<string> sentences, Text dialogue_text)
        {
            if (sentences.Count.Equals(0))
            {
                // Stop Dialogue Parents;
                //Debug.Log("Dialogue Clear");
                return -1;
            }

            string sentence = sentences.Dequeue();

            dialogue_text.text = "";
            dialogue_text.DOText(sentence, sentence.Length * .05f, true, ScrambleMode.None)
                .SetId("dialogue")
                .SetEase(Ease.Flash);

            return sentence.Length;
        }

        /// <summary>
        /// 인덱스 증가 함수
        /// </summary>
        /// <param name="current">current Index</param>
        /// <param name="max">max Index</param>
        /// <returns>current >= max</returns>
        public static bool IncreaseIndex(ref int current, int max)
        {
            current++;

            if(current >= max)
            {
                current = 0;
                return true;
            }

            return false;
        }

        #region Transform2Vector 

        /// <summary>
        /// Dotween transform.DOPath에 쓰이는 Vector[] 구하기 위해 구현. <br/><br/>
        /// Transform List -(변환)-> Vector3[]
        /// </summary>
        /// <param name="points">변환 시켜 줄 Transform 리스트</param>
        /// <returns></returns>
        public static Vector3[] GetPathVectors(List<Transform> points)
        {
            Vector3[] vectors = new Vector3[points.Count];

            int index = 0;
            foreach(Transform transform in points)
            {
                vectors[index++] = transform.position; 
            }

            return vectors;
        }

        /// <summary>
        /// Dotween transform.DOPath에 쓰이는 Vector[] 구하기 위해 구현. <br/><br/>
        /// Transform[] -(변환)-> Vector3[]
        /// </summary>
        /// <param name="points">변환 시켜 줄 Transform[]</param>
        /// <returns></returns>
        public static Vector3[] GetPathVectors(Transform[] points)
        {
            Vector3[] vectors = new Vector3[points.Length];

            int index = 0;
            foreach (Transform transform in points)
            {
                vectors[index++] = transform.position;
            }

            return vectors;
        }

        #endregion

        #region Kinect v2 ☜☞ Astra Orrbec

        public static void TouchEnable(bool isEnable)
        {
#if ASTRA
            TouchManager.Instance.enabled = isEnable;

#elif KINECT
            KinectTouchManager.Instance.enabled = isEnable;

#endif
        }

        public static void SensorReset()
        {
#if ASTRA
            TouchManager.Instance.Reset();
            Log.Debug("센서(Astra) 리셋");

#elif KINECT
            KinectTouchManager.Instance.Reset();
            Log.Debug("센서(Kinect) 리셋");

#endif
        }

        public static Vector2 GetLastHit()
        {
#if ASTRA
            return TouchManager.Instance.LastHit;
#elif KINECT
            return KinectTouchManager.Instance.lastHit;
#endif
        }

        public static void SensorInit(UnityAction<int, int> callback)
        {
#if ASTRA
            TouchManager.Instance.Init(false, callback);
            Log.Debug("센서(Astra) 초기화");
#elif KINECT
            KinectTouchManager.Instance.Init(false, callback);
            Log.Debug("센서(Kinect) 초기화");
#endif
        }

        public static void InitControlUI(ref RectTransform rect)
        {
#if ASTRA
            // 아스트라
            rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(1280f, 720f);
            rect.anchoredPosition = new Vector2(0f, 0f);
#elif KINECT
            // 키넥트
            rect.pivot = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(1920f, 1080f);
            rect.anchoredPosition = new Vector2(0f, 0f);
#endif
        }


        public static void InitSetting(ref RectTransform TopLeft, ref RectTransform TopRight, ref RectTransform BottomLeft, ref RectTransform BottomRight)
        {
#if ASTRA
            // 아스트라
            TopLeft.localPosition = new Vector2(Astra_.ModelManager.Instance.settingModel.TopLeftX, Astra_.ModelManager.Instance.settingModel.TopLeftY);
            TopRight.localPosition = new Vector2(Astra_.ModelManager.Instance.settingModel.TopRightX, Astra_.ModelManager.Instance.settingModel.TopRightY);
            BottomLeft.localPosition = new Vector2(Astra_.ModelManager.Instance.settingModel.BottomLeftX, Astra_.ModelManager.Instance.settingModel.BottomLeftY);
            BottomRight.localPosition = new Vector2(Astra_.ModelManager.Instance.settingModel.BottomRightX, Astra_.ModelManager.Instance.settingModel.BottomRightY);
#elif KINECT
            // 키넥트
            TopLeft.localPosition = new Vector2(ModelManager.Instance.settingModel.TopLeftX * Config.SCREEN_WIDTH / Config.COLOR_WIDTH, ModelManager.Instance.settingModel.TopLeftY * Config.SCREEN_HEIGHT / Config.COLOR_HEIGHT);
            TopRight.localPosition = new Vector2(ModelManager.Instance.settingModel.TopRightX * Config.SCREEN_WIDTH / Config.COLOR_WIDTH, ModelManager.Instance.settingModel.TopRightY * Config.SCREEN_HEIGHT / Config.COLOR_HEIGHT);
            BottomLeft.localPosition = new Vector2(ModelManager.Instance.settingModel.BottomLeftX * Config.SCREEN_WIDTH / Config.COLOR_WIDTH, ModelManager.Instance.settingModel.BottomLeftY * Config.SCREEN_HEIGHT / Config.COLOR_HEIGHT);
            BottomRight.localPosition = new Vector2(ModelManager.Instance.settingModel.BottomRightX * Config.SCREEN_WIDTH / Config.COLOR_WIDTH, ModelManager.Instance.settingModel.BottomRightY * Config.SCREEN_HEIGHT / Config.COLOR_HEIGHT);
#endif
        }

        public static void SetSettingValue(int TopLeftX, int TopLeftY, int TopRightX, int TopRightY,
            int BottomLeftX, int BottomLeftY, int BottomRightX, int BottomRightY)
        {
#if ASTRA
            // 아스트라
            Astra_.ModelManager.Instance.settingModel.TopLeftX = TopLeftX;
            Astra_.ModelManager.Instance.settingModel.TopLeftY = TopLeftY;
            Astra_.ModelManager.Instance.settingModel.TopRightX = TopRightX;
            Astra_.ModelManager.Instance.settingModel.TopRightY = TopRightY;

            Astra_.ModelManager.Instance.settingModel.BottomLeftX = BottomLeftX;
            Astra_.ModelManager.Instance.settingModel.BottomLeftY = BottomLeftY;
            Astra_.ModelManager.Instance.settingModel.BottomRightX = BottomRightX;
            Astra_.ModelManager.Instance.settingModel.BottomRightY = BottomRightY;

            Astra_.ModelManager.Instance.SaveSettingModel();
#elif KINECT
            // 키넥트 
            ModelManager.Instance.settingModel.TopLeftX = TopLeftX * Config.COLOR_WIDTH / Config.SCREEN_WIDTH;
            ModelManager.Instance.settingModel.TopLeftY = TopLeftY * Config.COLOR_HEIGHT / Config.SCREEN_HEIGHT; 
            ModelManager.Instance.settingModel.TopRightX = TopRightX * Config.COLOR_WIDTH / Config.SCREEN_WIDTH; 
            ModelManager.Instance.settingModel.TopRightY = TopRightY * Config.COLOR_HEIGHT / Config.SCREEN_HEIGHT; 

            ModelManager.Instance.settingModel.BottomLeftX = BottomLeftX * Config.COLOR_WIDTH / Config.SCREEN_WIDTH; 
            ModelManager.Instance.settingModel.BottomLeftY = BottomLeftY * Config.COLOR_HEIGHT / Config.SCREEN_HEIGHT; 
            ModelManager.Instance.settingModel.BottomRightX = BottomRightX * Config.COLOR_WIDTH / Config.SCREEN_WIDTH; 
            ModelManager.Instance.settingModel.BottomRightY = BottomRightY * Config.COLOR_HEIGHT / Config.SCREEN_HEIGHT; 

            ModelManager.Instance.SaveSettingModel();
#endif
        }

        public static void SaveSettingMovel()
        {
#if ASTRA
            Astra_.ModelManager.Instance.SaveSettingModel();
#elif KINECT
            ModelManager.Instance.SaveSettingModel();
#endif

            SettingModelInitializerForKinect.Instance.SaveSetting();

        }

        #endregion

        /// <summary>
        /// Time Set
        /// </summary>
        /// <param name="second">Input Second</param>
        /// <returns></returns>
        public static string GetTime(int second)
        {
            int MIN = second / 60;
            int SEC = second % 60;

            string timeStr = string.Format("{0:00}:{1:00}", MIN, SEC);
            return timeStr;
        }
        
        static public T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }
    }
}