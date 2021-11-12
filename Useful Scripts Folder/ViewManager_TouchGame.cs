using UnityEngine;
using UnityEngine.Events;
using System;

using Util;
using Util.Usefull;
using Util.Astra_;

using UniRx;
using UniRx.Triggers;

namespace Sports
{
    public class ViewManager_TouchGame : MonoBehaviour
    {
        // public Text BlueTeamScoreText;
        [Header("touch:0, Sea:1, Forest:2")]
        public int Mode;

        // Start is called before the first frame update
        public GameObject RootObject;


        GameObject touchImagePrefab;
        GameObject soapImagePrefab;
        GameObject flowerImagePrefab;

        GameObject hitEffectImage;
#pragma warning disable IDE0052 // 읽지 않은 private 멤버 제거
        GameObject hitEffectImageSetting;
#pragma warning restore IDE0052 // 읽지 않은 private 멤버 제거

        public GameObject PoupExit;
        public GameObject PopupGameExit;

        public SoundManager soundManager;
        public GameObject TimerPanel;

        [Header("0: TouchImage, 1: SoapImage, 2: FlowerImage")]
        public int Menu = -1;


        void Awake()
        {
            touchImagePrefab = Resources.Load("TouchImage") as GameObject;
            soapImagePrefab = Resources.Load("SoapImage") as GameObject;
            flowerImagePrefab = Resources.Load("FlowerImage") as GameObject;

            hitEffectImageSetting = Resources.Load("HitImage") as GameObject;

            switch (Menu)
            {
                case 0:
                    hitEffectImage = touchImagePrefab;
                    break;
                case 1:
                    hitEffectImage = soapImagePrefab;
                    break;
                case 2:
                    hitEffectImage = flowerImagePrefab;
                    break;
                default:
                    hitEffectImage = touchImagePrefab;
                    break;
            }

            GameManager.Instance.ViewManager_TouchGame = this;
            soundManager = GetComponent<SoundManager>();
        }

        void Start()
        {
            Function.SensorInit(new UnityAction<int, int>(BallClick));
            Invoke("CheckView", .1f);


            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.K))
                .Subscribe(_ =>
                {
                    switch (Mode)
                    {
                        case 0:
                            OnClickMainHAND();
                            break;
                        case 1:
                            OnClickMainSEA();
                            break;
                        default:
                            return;
                    }
                })
                .AddTo(gameObject);
        }

        public void CheckView()
        {
            for (int i = 0; i < Environment.GetCommandLineArgs().Length; i++)
            {
                //Debug.Log("a:"+ Environment.GetCommandLineArgs()[i]);
                if (Environment.GetCommandLineArgs()[i].Equals("-game1"))
                {
                    SetView(VIEW.SEA);
                    return;
                }

                if (Environment.GetCommandLineArgs()[i].Equals("-game2"))
                {
                    SetView(VIEW.HAND);
                    return;
                }

                if (Environment.GetCommandLineArgs()[i].Equals("-game3"))
                {
                    SetView(VIEW.FLOWER);
                    return;
                }

            }
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

        public enum VIEW
        {
            SETUP,
            MAIN, // 터치 게임 선택, 바다 청소, 손세균 꽃과 나비
            SEA,
            HAND,
            FLOWER,
            GAME_SEA,
            GAME_SEA_VS,
            GAME_HAND,
            GAME_HAND_VS,
            GAME_FLOWER,
        }

        public GameObject[] ViewList;

        void SetViewMainAndSub(int view, bool visible)
        {
            if (ViewList[view] != null)
            {
                ViewList[view].SetActive(visible);
            }
        }

        VIEW NowView = VIEW.MAIN;
        public void SetView(VIEW view)
        {

            switch (view)
            {

                case VIEW.GAME_HAND:
                case VIEW.GAME_HAND_VS:
                    hitEffectImage = soapImagePrefab;
                    break;
                case VIEW.GAME_FLOWER:
                    hitEffectImage = flowerImagePrefab;
                    break;
                default:
                    hitEffectImage = touchImagePrefab;
                    break;
            }

            //Debug.Log("SetView:" + view.ToString());
            switch (view)
            {
                case VIEW.GAME_SEA:
                case VIEW.GAME_SEA_VS:
                case VIEW.GAME_HAND:
                case VIEW.GAME_HAND_VS:
                case VIEW.GAME_FLOWER:
                    SetViewMainAndSub((int)view, false);

                    break;
                default:
                    break;
            }

            for (int i = 0; i < ViewList.Length; i++)
            {
                if (i.Equals((int)view))
                {
                    SetViewMainAndSub(i, true);
                }
                else
                {
                    SetViewMainAndSub(i, false);
                }
            }

            NowView = view;

            switch (view)
            {
                case VIEW.MAIN:
                    soundManager.PlayBGM(0);
                    break;
                case VIEW.SEA:
                    //case VIEW.GAME_SEA:
                    //case VIEW.GAME_SEA_VS:
                    soundManager.PlayBGM(1);
                    break;

                case VIEW.HAND:
                    //case VIEW.GAME_HAND:
                    //case VIEW.GAME_HAND_VS:
                    soundManager.PlayBGM(2);
                    break;

                case VIEW.FLOWER:
                    //case VIEW.GAME_FLOWER:
                    soundManager.PlayBGM(3);
                    break;
            }
        }

        public void OnClickMainSEA()
        {
            SetView(VIEW.SEA);
        }

        public void OnClickMainHAND()
        {
            SetView(VIEW.HAND);
        }
        public void OnClickMainFLOWER()
        {
            SetView(VIEW.FLOWER);
        }

        public void OnClickMainSetup()
        {
            SetView(VIEW.SETUP);
        }

        public void OnClickHome()
        {
            //TouchManager.Instance.enabled = true;
            SetView(VIEW.MAIN);
        }

        public void OnClickMainGAME_SEA()
        {
            SetView(VIEW.GAME_SEA);
        }

        public void OnClickMainGAME_SEA_VS()
        {
            SetView(VIEW.GAME_SEA_VS);
        }

        public void OnClickMainGAME_HAND()
        {
            SetView(VIEW.GAME_HAND);
        }

        public void OnClickMainGAME_HAND_VS()
        {
            SetView(VIEW.GAME_HAND_VS);
        }

        public void OnClickMainGAME_FLOWER()
        {
            SetView(VIEW.GAME_FLOWER);
        }

        bool IsEnding = false;
        public void Exit()
        {
            if (IsEnding.Equals(true))
                return;

            IsEnding = true;

            Invoke("Quit", 2.0f);

        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Quit_to_Home()
        {
            LoadingSceneManager.LoadScene("Launcher");
        }

        public void Regame()
        {
            SetView(NowView);
        }
    }
}