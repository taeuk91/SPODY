using System.Collections;

using UnityEngine;
using UnityEngine.Video;

using UniRx;
using UniRx.Triggers;

using Util.Usefull;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Util
{
    public class VideoController : MonoBehaviour
    {
        [Title("VideoController")]
        [InfoBox(
            "아래에서 게임모드를 설정해 주세요.")]
        enum GameMode
        {
            SingleMode,
            ChallengeMode
        }
        [SerializeField] private GameMode gameMode = GameMode.SingleMode;

        private static VideoController instance;
        public static VideoController Instance => instance;

        [Header("Video Player")]
        public VideoPlayer m_VideoPlayer;

        [Header("Clip")]
        public VideoClip Opening;
        public VideoClip Ending;
        public VideoClip ExerciseSong;
        [SerializeField] private GameObject buttons; 


        public SceneChange sceneChange;
        public DOTweenAnimation fadeImage;
        public GameObject[] objs;

        public float delayTime;
        private bool isSkip = false;


        private void Awake()
        {
            instance = this;
            buttons = GameObject.Find("Song Buttons");
        }

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => IsPlaying() && Input.GetKeyDown(KeyCode.Space) && !m_VideoPlayer.clip.Equals(Ending))
                .Subscribe(_ =>
                {
                    if (isSkip)
                        return;

                    isSkip = true;

                    StartCoroutine(Delay());

                    IEnumerator Delay()
                    {
                        StopCoroutine(playVoice);
                        fadeImage.DORestartById("IN");
                        yield return new WaitForSeconds(1.7f);
                        fadeImage.DORestartById("OUT");
                        
                        foreach (var v in objs)
                            v.SetActive(true);
                        SetEnable(false);

                        isSkip = false;
                    }
                })
                .AddTo(this.gameObject);

            this.UpdateAsObservable()
                .Where(_ => IsPlaying() && Input.GetKeyDown(KeyCode.Space) && m_VideoPlayer.clip.Equals(Ending))
                .Subscribe(_ =>
                {
                    if (isSkip)
                        return;

                    isSkip = true;

                    sceneChange.CallScene("Launcher");
                })
                .AddTo(this.gameObject);
        }

        public void PlayClip(VideoClip clip)
        {
            StartCoroutine(Play());

            IEnumerator Play()
            {
                if (SoundController.Instance != null)
                {
                    SoundController.Instance.SetBgmVolume(0f);
                }

                yield return null;

                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(2f);
                fadeImage.DORestartById("OUT");

                foreach (var v in objs)
                    v.SetActive(false);

                m_VideoPlayer.clip = clip;
                m_VideoPlayer.Play();

                yield return new WaitForSeconds(1f);
                yield return new WaitUntil(() => IsPlaying());

                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(2f);
                fadeImage.DORestartById("OUT");

                foreach (var v in objs)
                    v.SetActive(true);
                SetEnable(false);
            }
        }

        Coroutine playVoice;
        public void StartVideo()
        {
            playVoice = StartCoroutine(Delay());

            foreach (var v in objs)
                v.SetActive(false);

            IEnumerator Delay()
            {
                yield return null;
                PlayIntro();
                yield return new WaitForSeconds(delayTime);

                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(1.7f);
                fadeImage.DORestartById("OUT");

                foreach (var v in objs)
                    v.SetActive(true);

                SetEnable(false);
            }
        }

        public void PlayIntro()
        {
            if (SoundController.Instance != null)
            {
                SoundController.Instance.SetBgmVolume(.3f);
            }

            m_VideoPlayer.clip = Opening;
            m_VideoPlayer.Play();
        }

        
        private PopupPanel_ PopupPanel_;
        private PopupPanel_ vs_Result_panel;
        public void PlayExerciseSong()
        {
            if(SoundController.Instance != null)
            {
                SoundController.Instance.SetBgmVolume(0f);
            }
            
            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                UIManager uiManager = GameObject.FindObjectOfType<UIManager>();

#if EDU
                PopupPanel_ = uiManager.singlePanelEdu;
                PopupPanel_.gameObject.SetActive(false);
                vs_Result_panel = uiManager.versusPanelEdu;
                vs_Result_panel.gameObject.SetActive(false);
#endif
                
                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(1.8f);
                
                SoundController.Instance.SetBgmVolume(.2f);
                
                yield return new WaitForSeconds(.2f);
                fadeImage.DORestartById("OUT");
                
                m_VideoPlayer.clip = ExerciseSong;
                m_VideoPlayer.Play();
                BtnsUp();
            }
        }

        public void PlayExerciseSongInLauncher()
        {
            if (SoundController.Instance != null)
            {
                SoundController.Instance.SetBgmVolume(0f);
            }

            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                UIManager uiManager = GameObject.FindObjectOfType<UIManager>();
                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(1.8f);

                SoundController.Instance.SetBgmVolume(.2f);

                yield return new WaitForSeconds(.2f);
                fadeImage.DORestartById("OUT");

                m_VideoPlayer.clip = ExerciseSong;
                m_VideoPlayer.Play();
                BtnsUp();
            }
        }

        public void BtnsUp()
        {
            StartCoroutine(StartAnim());

            IEnumerator StartAnim()
            {
                yield return null;

                Time.timeScale = 1f;

                buttons.SetActive(true);
                buttons.transform.DOLocalMoveY(-237f, 0.6f)
                    .SetDelay(.2f);
            }
        }

        public void PlayEnding()
        {
            if(SoundController.Instance != null)
            {
                SoundController.Instance.SetBgmVolume(0f);
            }
            
            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(1.8f);
                SoundController.Instance.SetBgmVolume(.2f);
                Function.TouchEnable(false);
                
                switch (gameMode)
                {
                    case GameMode.SingleMode:
                        GameManager.Instance.ViewManager_.SetPopupGameEnd("", ViewManager_.VIEW.MAIN);
                        break;
                    case GameMode.ChallengeMode:
                        Util.GameManager.Instance.ViewManager_.SetPopupVsGameEnd(ViewManager_.VIEW.GAME_TIMEVS);
                        break;
                }

                
                foreach (var v in objs)
                    v.SetActive(false);

                // m_VideoPlayer.clip = Ending;
                // m_VideoPlayer.Play();

                yield return new WaitForSeconds(.2f);
                fadeImage.DORestartById("OUT");

                // yield return new WaitForSeconds(.7f);
                // SetEnable(false);
                //sceneChange.CallScene("Launcher");
            }
        }

        public void SetEnable(bool isEnable)
        {
            m_VideoPlayer.clip = null;
            m_VideoPlayer.enabled = isEnable;
        }

        public bool IsPlaying()
        {
            return m_VideoPlayer.isPlaying;
        }


        public void Replay()
        {
            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return null;
                fadeImage.DORestartById("In");
                yield return new WaitForSeconds(2f);

                foreach (var v in objs)
                {
                    v.SetActive(fadeImage);
                }
                yield return null;
                fadeImage.DORestartById("Out");
            }
        }


    }
}