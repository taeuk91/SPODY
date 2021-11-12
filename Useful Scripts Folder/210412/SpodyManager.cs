using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Util.Usefull;
using UnityEngine.SceneManagement;
using log4net;

namespace SPODY
{

    public abstract class SpodyManager<T> : MonoBehaviour where T : SpodyManager<T>
    {
        #region Singleton Codes..

        protected static T instance = null;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }

            InitAwake();
        }

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(MonoBehaviour));

        public const string FADE_IN = "IN";
        public const string FADE_OUT = "OUT";

        protected WaitForSeconds fadeDelay = new WaitForSeconds(2f);

        #region Inspector View

        [Title("Spody Manager")]
        [InfoBox(
            "아래로는 SpodyManager에서 선언한 부분입니다.")]
        //enum GameMode
        //{
        //    SingleMode,
        //    ChallengeMode
        //}

        //[SerializeField] private GameMode gameMode = GameMode.SingleMode;

        [SerializeField]
        [Tooltip("터치 가능한 콜라이더들을 넣어주세요.")] protected List<BoxCollider> chrColliders = new List<BoxCollider>();

        [Space]

        [SerializeField]
        [Tooltip("[UI Canvas] - [ViewManager] - [MP4 Component] 안에 있는 ForFade를 넣어주세요.")] protected DOTweenAnimation fadeImage;

        [Space]

        [SerializeField]
        [Tooltip("인트로 대사 관련된 클립들을 넣어주세요.")] protected AudioClip[] introClips;

        [Space]

        [SerializeField] protected GameObject endingObject;

        [Title("Skip Button")]
        [SerializeField] protected GameObject skipButton;
        [SerializeField] protected GameObject endingButton;
        
        [Title("CountDown"), SerializeField] protected Countdown countDown;
        public bool isGameStarted = false;
        [Title("UpdownTimer"), SerializeField] protected UpDownTimer updownTimer;
        public int totalScoreInThisGame;
        public int savedHighestScore;
        [SerializeField] protected GameObject scoreUI;
        
        #endregion
        protected delegate void SkipButton();
        protected event SkipButton SkipButtonHandler;
        protected bool introSkip = false;

        protected Coroutine introCoroutine;
        protected Coroutine inGameCoroutine;

        protected Coroutine sequenceCoroutine;

        protected abstract void InitAwake();

        protected abstract void InitStart();

        protected abstract void InitOnEnable();

        protected virtual void OnEnable()
        {
            if (PlayerPrefs.GetInt("ReLoad").Equals(Contents.GetState(Contents.STATE.SKIP)))
            {
                Invoke(nameof(DoSkip), .1f);
            }
            else
            {
                sequenceCoroutine = StartCoroutine(Sequence(false));
            }
            
            // 현재 씬에 따른 최고 스코어를 가져옴
            savedHighestScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name);
            Log.Info("[In-Game]현재 Scene:" + SceneManager.GetActiveScene().name + "/ 최근점수:" + savedHighestScore);
            
            UpDownTimer upDownTimer = FindObjectOfType<UpDownTimer>();

            Winner.blue_team_score = 0;
            Winner.red_team_score = 0;

            if (upDownTimer != null)
            {
                this.updownTimer = upDownTimer;
            }

            InitOnEnable();
        }

        protected void Start()
        {
            InitStart();
        }

        public void DoSkip()
        {
            if (introCoroutine != null)
            {
                isGameStarted = true;
                
                SkipButtonHandler();

                StopCoroutine(sequenceCoroutine);
            }

            sequenceCoroutine = StartCoroutine(Sequence(true));
        }

        protected virtual IEnumerator Sequence(bool introSkip)
        {
            if (!introSkip)
            {
                SkipButtonHandler = SkipIntro;
                
                skipButton.SetActive(true);
                endingButton.SetActive(false);
                
                yield return introCoroutine = StartCoroutine(INTRO());
            }

            if (PlayerPrefs.GetInt("ReLoad").Equals((int)Contents.STATE.ACTION))
            {
                yield return StartCoroutine(FadeInOut());
            }
            
            skipButton.SetActive(false);
            endingButton.SetActive(true);

            // 카운트 다운 시작 및 대기
            countDown.gameObject.SetActive(true);
            yield return null;
            yield return new WaitUntil(() => !countDown.IsPlay);

            SkipButtonHandler = SkipGame;
            
            if(updownTimer != null)
                updownTimer.isTimerOn = true;
            
            yield return inGameCoroutine = StartCoroutine(INGAME());
            
            Ending();
        }

        protected abstract IEnumerator INTRO();

        /// <summary>
        /// 210416 WooSon // 
        /// yield return StartCoroutine(PlayIntroClips()); 이런식으로 쓸 것
        /// </summary>
        protected virtual IEnumerator PlayIntroClips()
        {
            WaitForSeconds waitPlayDelay = new WaitForSeconds(.5f);

            for (int clipIndex = 0; clipIndex < introClips.Length; clipIndex++)
            {
                
                SoundController.Instance.PlayClip(introClips[clipIndex]);

                float time = 0;
                while(time < introClips[clipIndex].length)
                {
                    if (introSkip) yield break;

                    time += Time.deltaTime;
                    yield return null;
                }

                yield return waitPlayDelay;
            }
        }
        
        protected abstract IEnumerator INGAME();

        protected abstract void Ending();

        protected abstract void SkipIntro();

        protected abstract void SkipGame();
        
        public virtual void SkipButon()
        {
            SkipButtonHandler();
        }

        #region Util

        #region Fade

        public void FadeIn()
        {
            fadeImage.DORestartById(FADE_IN);
        }

        public void FadeOut()
        {
            fadeImage.DORestartById(FADE_OUT);
        }

        /// <summary>
        /// 210412 WooSon // 
        /// yield return StartCoroutine(FadeInOut()); 이런식으로 쓸 것
        /// </summary>
        public IEnumerator FadeInOut()
        {
            FadeIn();
            yield return fadeDelay;
            FadeOut();
        }

        #endregion

        private void SaveScore()
        {
            // Turn on Score UI & Save Highest Score
            if (scoreUI != null)
            {
                scoreUI.SetActive(true);
            }

            if (totalScoreInThisGame > savedHighestScore)
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, totalScoreInThisGame);
                print("Now Highest Score : " + PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, totalScoreInThisGame));
                
                Log.Info("[End]현재 Scene:" + SceneManager.GetActiveScene().name + " / 최고점수:" + totalScoreInThisGame);
            }
        }
        
        protected void EndingVideo()
        {
            SaveScore();
            VideoController.Instance.SetEnable(true);
            VideoController.Instance.PlayEnding();
        }

        public void AddCollider(BoxCollider collider)
        {
            chrColliders.Add(collider);
        }

        public void RemoveAllCollider()
        {
            chrColliders.Clear();
        }

        public void TouchEnable(bool canTouch)
        {
            if(chrColliders.Count < 1)
            {
                return;
            }

            foreach (var chr in chrColliders)
            {
                if (chr.transform.CompareTag("Player"))
                    chr.enabled = canTouch;
            }
        }

        #endregion
    }
}

