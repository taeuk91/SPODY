using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using DG.Tweening;

using System.IO;
using System.Linq;
using System.Collections.Generic;
using log4net;
// using Util.Astra_;

namespace Util
{
    
    public struct Contents
    {
        
        public enum Sort 
        {
            NONE = -1, _5세, _6세, _7세,
        }

        public enum STATE 
        {
            NONE = -1, ACTION, SKIP, 
        }

        public static Sort _sort;

        public static int GetSort(Sort sort)=> (int) sort;

        public static int GetState(STATE state) => (int)state;

    }

    // 센서도 여기서 해
    public class SceneChange : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SceneChange));
        
        [Header("For Fade Background Image")]
        public Image _background_for_fade;

        PlayTime _time;

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            _time = new PlayTime();


            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.L))
                .Subscribe(_ =>
                {
                    Log.Info(SceneManager.GetActiveScene().name + "에서 L버튼 눌림");
                    CallScene("Launcher");
                })
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.P))
                .Subscribe(_ => 
                {
                    Log.Info(SceneManager.GetActiveScene().name + "에서 P버튼 눌림");
                    PlayerPrefs.SetInt("ReLoad",  Contents.GetState(Contents.STATE.ACTION));
                    LoadingSceneManager1.LoadScene(SceneManager.GetActiveScene().buildIndex);
                })
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.O))
                .Subscribe(_ =>
                {
                    Log.Info(SceneManager.GetActiveScene().name + "에서 O버튼 눌림");
                    PlayerPrefs.SetInt("ReLoad", Contents.GetState(Contents.STATE.SKIP));
                    LoadingSceneManager1.LoadScene(SceneManager.GetActiveScene().buildIndex);
                })
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.I))
                .Subscribe(_ => 
                {
#if UNITY_EDITOR

#endif
                    Application.Quit();

                })
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.T))
                .Subscribe(_ =>
                {
                    Time.timeScale = 1f;
                })
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_=>{
                    if(_background_for_fade)
                        _background_for_fade.color = Color.clear;
                });
#if UNITY_EDITOR
            this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.UpArrow))
            .Subscribe(_ =>
            {
                Time.timeScale += 1f;
            });

            this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.DownArrow))
            .Subscribe(_ =>
            {
                Time.timeScale -= 1f;
            });
#endif

            StartCoroutine(FadeIn());
            //DOVirtual.DelayedCall(0f, () => StartCoroutine(FadeIn()), true);
        }

        private void OnEnable()
        {
            Util.Usefull.Function.SensorReset();
        }

        public void CallScene(string SceneName)
        {
            if(SceneName.Equals("Launcher"))
            {
                PlayerPrefs.SetInt("Opening", Contents.GetState(Contents.STATE.SKIP));
            }
            
            PlayerPrefs.SetInt("ReLoad", Contents.GetState(Contents.STATE.ACTION));
            StartCoroutine(FadeOut());
            //SceneManager.LoadScene(SceneName);

            string path = "Diary.json";
            string origin = File.ReadAllText("Diary.json");
            string str = origin + "\n";

            char index = SceneName.ElementAt(2);


            // 국어 0, 영어 1, 탐구 2, 신체 3, 위생 4

#if UNITY_EDITOR
            print("Launcher index: " + PlayerPrefs.GetInt("Launcher"));
#endif

            switch (index)
            {
                case '5':
                    str += "→ " + "\"5세 콘텐츠\" \"<" + SceneName + ">\" 시작: " + _time.GetTime();
                    PlayerPrefs.SetInt("Launcher", Contents.GetSort(Contents.Sort._5세));
                    break;
                case '6':
                    str += "→ " + "\"6세 콘텐츠\" \"<" + SceneName + ">\" 시작: " + _time.GetTime();
                    PlayerPrefs.SetInt("Launcher", Contents.GetSort(Contents.Sort._6세));
                    break;
                case '7':
                    str += "→ " + "\"7세 콘텐츠\" \"<" + SceneName + ">\" 시작: " + _time.GetTime();
                    PlayerPrefs.SetInt("Launcher", Contents.GetSort(Contents.Sort._7세));
                    break;
                case 'u'://la 'u' ncher
                    str += "→ " + "\"콘텐츠 종료 시간: " + _time.GetTime();
                    break;
                default:
                    break;

            }

            File.WriteAllText(path, str);

            IEnumerator FadeOut()
            {
                if(_background_for_fade)
                {
                    _background_for_fade.raycastTarget = true;

                    yield return new WaitForSeconds(.2f);

                    _background_for_fade.DOColor(Color.black, .7f)
                        .SetEase(Ease.InCirc);

                    yield return new WaitForSeconds(1f);
                    LoadingSceneManager.LoadScene(SceneName);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    LoadingSceneManager.LoadScene(SceneName);
                }

                yield return null;
            }
        }

        IEnumerator FadeIn()
        {
            if (_background_for_fade)
            {
                _background_for_fade.color = Color.black;

                yield return new WaitForSeconds(.2f);

                Tween fade = _background_for_fade.DOColor(Color.clear, .7f)
                    .SetEase(Ease.OutQuad);
                // DOVirtual.DelayedCall(0f, ()=> fade, true);

                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }


        public void NoLoading(string SceneName)
        {
            PlayerPrefs.SetInt("ReLoad", Contents.GetState(Contents.STATE.SKIP));
            StartCoroutine(FadeOut());

#if true // 211022 - WooSon // 변경 사유 : 모기를 잡아라. 인트로에서 벗어날 수 가 없음.
            /* BEFORE ->> SceneName = SceneManager.GetActiveScene().name */
            SceneName = SceneName.Equals(SceneManager.GetActiveScene().name) ? SceneManager.GetActiveScene().name : SceneName;
#endif
            
            IEnumerator FadeOut()
            {
                if (_background_for_fade)
                {
                    _background_for_fade.raycastTarget = true;

                    yield return new WaitForSeconds(.2f);

                    _background_for_fade.DOColor(Color.black, .3f)
                        .SetEase(Ease.InCirc);

                    yield return new WaitForSeconds(1f);
                    LoadingSceneManager1.LoadScene(SceneName);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    LoadingSceneManager1.LoadScene(SceneName);
                }

            }
        }

        public void RestartScene()
        {
            PlayerPrefs.SetInt("ReLoad", Contents.GetState(Contents.STATE.ACTION));
            StartCoroutine(FadeOut());

            IEnumerator FadeOut()
            {
                if (_background_for_fade)
                {
                    _background_for_fade.raycastTarget = true;

                    yield return new WaitForSeconds(.2f);

                    _background_for_fade.DOColor(Color.black, .3f)
                        .SetEase(Ease.InCirc);

                    yield return new WaitForSeconds(1f);
                    
                    LoadingSceneManager1.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    LoadingSceneManager1.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }

            }
        }

        public void GetRead()
        {
            string path = "Diary.json";
            var origin = File.ReadLines(path);
            string SensorName = string.Join(System.Environment.NewLine, origin);
            
            switch (SensorName.First())
            {
                case 'K':
                    //PlayerPrefs.SetString("SENSOR", "KINECT");
                    //print(SensorName);
                    break;
                case 'A':
                    //PlayerPrefs.SetString("SENSOR", "ASTRA");
                    //print(SensorName);
                    break;
                default:
                    //print("ERROR");
                    break;
            }
            //PlayerPrefs.Save();
        }

        
        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("ReLoad", Contents.GetState(Contents.STATE.NONE));
            PlayerPrefs.SetInt("Opening", Contents.GetState(Contents.STATE.ACTION));
            PlayerPrefs.SetInt("Launcher", Contents.GetSort(Contents.Sort.NONE));

            PlayerPrefs.Save();
        }


    }
}