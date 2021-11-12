using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Util;
using Util.Astra_;

using Util.Usefull;

namespace Sports
{
    public class ViewManager : MonoBehaviour
    {
        public Text BlueTeamScoreText;


        public GameObject RootObject;
        public GameObject SetRoot;

        public GameObject hitEffectImage;
        GameObject hitEffectImageSetting;

        public PopupPanel popupPanel;
        public GameObject PoupExit;

        public PopupPanel leftPanel;
        public PopupPanel rightPanel;

        public SoundManager soundManager;

        public GameObject goalPrefab;

        public GameObject goalPrefab1;
        public GameObject goalPrefab2;
        public GameObject goalPrefab3;

        public GameObject goalRight2Prefab;
        public GameObject goalRight3Prefab;



        public GameObject TimerPanel;

        string sensorName = "";

        public bool isMain = false;
        
        //211026 태욱 : EDU모드일 때 엔딩패널 켜기
        [SerializeField] private GameObject singleModePanel;
        [SerializeField] private GameObject vsModePanel;

        private void OnEnable()
        {
            singleModePanel.SetActive(false);
            vsModePanel.SetActive(false);
        }

        void Awake()
        {
            Config.SCREEN_WIDTH = Screen.width;
            Config.SCREEN_HEIGHT = Screen.height;

            if(SettingModelInitializerForKinect.Instance == null)
            {
                SettingModelInitializerForKinect.Instance.Init();
            }

            GameManager.Instance.ViewManager = this;
            soundManager = GetComponent<SoundManager>();
            
            AddressableManager.Instance.Init();
        }

        void Start()
        {
            Function.SensorInit(new UnityAction<int, int>(BallClick));

            if (isMain)
            {
                Function.SensorInit(new UnityAction<int, int>(BallClickSetting));
            }

            hitEffectImageSetting = Resources.Load("HitImage") as GameObject;
        }

        public void Init()
        {
            Function.SensorInit(new UnityAction<int, int>(BallClick));
        }

        public void InitSetting()
        {
            Function.SensorInit(new UnityAction<int, int>(BallClickSetting));
        }


        void BallClick(int x, int y)
        {
            if (hitEffectImage == null || RootObject.activeSelf == false)
            {
                return;
            }

            var obj = Instantiate(hitEffectImage, RootObject.transform);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);

        }


        public ControlPanel_ _control;
        void BallClickSetting(int x, int y)
        {
            if (hitEffectImageSetting== null)
            {
                return;
            }

            GameObject go = Instantiate(hitEffectImageSetting, RootObject.transform);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, y);
            
            //UnityEngine.Debug.Log("BallClickSetting:" + go.transform.position.x + "," + go.transform.position.y);
            // 이 값 전송.
            _control.DefinePosition(rt.localPosition);

        }

        public enum VIEW
        {
            SETUP,
            MAIN, // 골 시간, 대결
            GOAL,
            TIME,
            TIMEVS,
            GAME_GOAL,
            GAME_TIME,
            GAME_TIMEVS,
        }

        // 8개
        // 0: SettingModel
        // 1: MainPanel
        // 2: GoalPanel
        // 3: TimePanel
        // 4: TimeVSPanel
        // 5: GameGoalPanel
        // 6: GameTimePanel
        // 7: GameTimeVSPanel in ViewList
        public GameObject[] ViewList;

        public void SetPopupGameEnd(string text, VIEW view)
        {
            RootObject.SetActive(false);

#if EDU
            singleModePanel.SetActive(true);

            string tempStr = Regex.Replace(text, @"\D", "");
            int tempScore = 0;
            if (!int.TryParse(tempStr, out tempScore))
            {
                Debug.LogWarning(string.Format("int로 변경 실패 : {0}", text));
            }
            
            Winner.red_team_score = tempScore;
#elif EXP
            StartCoroutine(Delay());
            popupPanel.RetryMode = view;
            popupPanel.gameObject.SetActive(true);
            popupPanel.MsgText.text = text;
            popupPanel.MsgText.transform.GetChild(0).GetComponent<Text>().text = text;
#endif

        }

        public void SetPopupVsGameEnd(VIEW view, bool Left)
        {
            RootObject.SetActive(false);
#if EDU
            vsModePanel.SetActive(true);
#elif EXP

            StartCoroutine(Delay());
            popupPanel.RetryMode = view;

            if (Left)
            {
                leftPanel.gameObject.SetActive(true);
            }
            else
            {
                rightPanel.gameObject.SetActive(true);
            }
#endif
        }

        // .1초 딜레이
        IEnumerator Delay()
        {
            // 아스트라
            // 키넥트
            Function.TouchEnable(false);
            yield return new WaitForSeconds(.1f);
            Function.TouchEnable(true);
        }

        IEnumerator SetViewMainAndSub_(int view, bool visible)
        {
            if (ViewList[view]== null)
            {
                yield return null;
            }

            // View List, MainTitle UI Check.. Left, Middle, Right
            if (view < 5)
            {
                ViewList[view].SetActive(visible);
            }

            else
            {
                TimerPanel.SetActive(visible);
                ViewList[view].SetActive(visible);

                yield return new WaitUntil(() => TimerPanel.GetComponent<TimerPanel>().setValue < 0.5f || !visible);

                StopCoroutine(SetViewMainAndSub_(view, visible));

                //if (ViewList[view] != null)
                //{
                //    ViewList[view].SetActive(visible);
                //    // StopCoroutine(SetViewMainAndSub_(view, visible));
                //}
            }
        }

        void SetViewMainAndSub(int view, bool visible)
        {
            //if ( View3DList[view]!=null)
            //{
            //    View3DList[view].SetActive(visible);
            //}

            if (ViewList[view] != null)
            {
                ViewList[view].SetActive(visible);
            }
        }


        public void SetView(VIEW view)
        {
            switch (view)
            {
                case VIEW.GAME_GOAL:
                case VIEW.GAME_TIME:
                case VIEW.GAME_TIMEVS:
                    // StartCoroutine(SetViewMainAndSub_((int)view, false));
                    SetViewMainAndSub((int)view, false);

                    GameManager.Instance.DataManager.InitGame();
                    popupPanel.RetryMode = view;
                    RootObject.SetActive(true);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < ViewList.Length; i++)
            {
                if (i.Equals((int)view))
                {
                    StartCoroutine(SetViewMainAndSub_(i, true));
                    // SetViewMainAndSub(i, true);
                }
                else
                {
                    // StartCoroutine(SetViewMainAndSub_(i, false));
                    SetViewMainAndSub(i, false);
                }
            }
        }

        public void OnClickMainGoal()
        {
            SetView(VIEW.GOAL);
        }

        public void OnClickMainTime()
        {
            SetView(VIEW.TIME);
        }
        public void OnClickMainTimeVS()
        {
            SetView(VIEW.TIMEVS);
        }

        public void OnClickMainSetup()
        {
            SetView(VIEW.SETUP);
        }

        //////////////////////////////////

        public void OnClick(int goal)
        {
            GameManager.Instance.DataManager.GoalMax = goal;
            SetView(VIEW.GAME_GOAL);
        }

        public void OnClickGoalAll()
        {
            GameManager.Instance.DataManager.GoalMax = -1;
            SetView(VIEW.GAME_GOAL);
        }

        public void OnClickHome()
        {
            RootObject.SetActive(false);
            SetView(VIEW.MAIN);
        }


        public void OnClickGoalBack()
        {
            SetView(VIEW.MAIN);
        }

        /////////////

        public void OnClickTime(int time)
        {
            GameManager.Instance.DataManager.TimeMax = time;
            SetView(VIEW.GAME_TIME);
        }

        public void OnClickTimeVs(int time)
        {
            GameManager.Instance.DataManager.TimeMax = time;
            SetView(VIEW.GAME_TIMEVS);
        }


        public void OnClickGoal()
        {
            //obj[index].transform.GetChild(0).gameObject.SetActive(true);
            //return;
            GameObject go = Instantiate(goalPrefab, RootObject.transform);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Function.GetLastHit();
            Destroy(go, 1.0f);
        }

        public void OnClickGoalPrefab2()
        {
            GameObject go = Instantiate(goalPrefab2, RootObject.transform);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Function.GetLastHit();
            Destroy(go, 1.0f);
        }

        public void OnRightClickGoalPrefab2()
        {
            GameObject go = Instantiate(goalRight2Prefab, RootObject.transform);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Function.GetLastHit();
            Destroy(go, 1.0f);
        }

        public void OnClickGoalPrefab3()
        {
            GameObject go = Instantiate(goalPrefab3, RootObject.transform);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Function.GetLastHit();
            Destroy(go, 1.0f);
        }

        public void OnRightClickGoalPrefab3()
        {
            GameObject go = Instantiate(goalRight3Prefab, RootObject.transform);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Function.GetLastHit();
            Destroy(go, 1.0f);
        }

        bool IsEnding = false;
        public void Exit()
        {
            if (IsEnding.Equals(true))
                return;

            IsEnding = true;

            Invoke("Quit", 0.8f);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void QuitToTitle()
        {
            LoadingSceneManager.LoadScene("Launcher");
        }

        public void StartSoccerScene()
        {
            LoadingSceneManager.LoadScene("Soccer");
        }

        public void StartBaseScene()
        {
            LoadingSceneManager.LoadScene("Baseball");
        }

        public void StartBasketScene()
        {
            LoadingSceneManager.LoadScene("Basketball");
        }

        public void StartTouchScene()
        {
            LoadingSceneManager.LoadScene("TouchGame");
        }

        //public void OnClickGoalBallNo()
        //{
        //    GameManager.Instance.DataManager.RedTeamScore += 0;
        //    BlueTeamScoreText.text = GameManager.Instance.DataManager.RedTeamScore.ToString();
        //}

        //public void OnClickGoalBall2()
        //{
        //    GameManager.Instance.DataManager.RedTeamScore += 2;
        //    BlueTeamScoreText.text = GameManager.Instance.DataManager.RedTeamScore.ToString();
        //}

        //public void OnClickGoalBall3()
        //{
        //    GameManager.Instance.DataManager.RedTeamScore += 3;
        //    BlueTeamScoreText.text = GameManager.Instance.DataManager.RedTeamScore.ToString();
        //}
    }
}