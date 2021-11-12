using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SPODY;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Util.Usefull;
using DG.Tweening;
using UnityEngine.Serialization;
using Util;
using UniRx;
using UniRx.Triggers;

namespace SPODY_12_5_3
{
    public class GameManager : SpodyManager<GameManager>
    {
        [Title("Game Manager")]
        [InfoBox("아래로는 Game Manager에서 선언한 부분입니다.")]
        private WaitForSeconds waitIntroDelay = new WaitForSeconds(.5f);
        public OpeningCharacter openingCharacter;
        
        [Header("HowToPlay--------------------")]
        public bool isHowToPlayMode = false;
        public GameObject howToPlayObj;

        [Header("In-Game--------------------")]
        public TeamManager redTeamManager;
        public TeamManager blueTeamManager;
        
        [Header("Red Team--------------------")]
        public Text redScoreTxt;
        public int redTouchCount = 0;
        public Text redNameIndication;

        [Header("Blue Team--------------------")]
        public Text blueScoreTxt;
        public int blueTouchCount = 0;
        public Text blueNameIndication;
        
        [Header("Effect--------------------")] 
        public GameObject correctAnswerEff;
        public GameObject wrongAnswerEff;
        public GameObject fireworksEff;
        public GameObject magicPuffEff;

        [Header("Sounds--------------------")]
        public AudioClip[] answerClips;
        public AudioClip getScoreClip;
        public AudioClip[] goodClips;
        public AudioClip encourageClip;
        public AudioClip windMagicSound;
        
        protected override void InitOnEnable()
        {
            redScoreTxt.text = "0";
            blueScoreTxt.text = "0";
            
            redTeamManager.Reset();
            blueTeamManager.Reset();
        }

        protected override IEnumerator INTRO()
        {
            yield return new WaitForEndOfFrame();
            Function.TouchEnable(false);
            
            openingCharacter.gameObject.SetActive(true);

            while (openingCharacter.State)
            {
                if (introSkip)
                {
                    SoundController.Instance.PauseClip();
                    yield break;
                }

                yield return null;
            }
            
            // 캐릭터 말하기 시작
            openingCharacter.SetState(SPODY_9_7_3.OpeningCharacter.TALK);

            SoundController.Instance.SetBgmVolume(.1f);
            
            // 인트로 클립 재생
            for (int index = 0; index < introClips.Length; index++)
            {
                if(introSkip)
                {
                    SoundController.Instance.PauseClip();

                    yield break;
                }
                SoundController.Instance.PlayClip(introClips[index]);
                
                //print("말하는중");
                if (isHowToPlayMode)
                {
                    yield return HowToPlay(index);
                }

                yield return new WaitForSeconds(introClips[index].length);
                yield return waitIntroDelay;
            }
            
            SoundController.Instance.SetBgmVolume(.3f);
            
            openingCharacter.SetState(OpeningCharacter.IDLE);

            while (openingCharacter.State)
            {
                if (introSkip)
                {
                    SoundController.Instance.PauseClip();
                    yield break;
                }
                yield return null;
            }
        }

        protected IEnumerator HowToPlay(int clipIndex)
        {
            Transform ball = howToPlayObj.transform.GetChild(0);
            Transform bak = howToPlayObj.transform.GetChild(1).transform;
            
            switch (clipIndex)
            {
                case 0:
                    break;
                
                // 1. 아이가 박에 공을 던지면 박에 맞는다.
                case 1:

                    
                    break;
                // 1. 박이 터진다.
                case 2:

                    break;
            }
            yield return null;
        }


       
        protected override IEnumerator INGAME()
        {
            print("게임시작");
            openingCharacter.gameObject.SetActive(false);
            isGameStarted = true;
            isHowToPlayMode = false;
            

            Function.TouchEnable(true);
            
            while (true)
            {
                if (!UpDownTimer.instance.isTimerOn)
                {
                    Ending();
                    break;
                }

                yield return null;
            }     
        }
        

        public void UpDownTeamScore(int teamScore, Text teamScoreUI)
        {
            teamScoreUI.text = teamScore.ToString();
        }

        
        protected override void Ending()
        {
            SoundController.Instance.SetBgmVolume(0.5f);
            
            StartCoroutine(EndingCoroutine());
        }
        
        public void OnEndingBtnClkEvent()
        {
            Ending();
        }
        
        public IEnumerator EndingCoroutine()
        {
            isGameStarted = false;
            isHowToPlayMode = true;
            Function.TouchEnable(false);

            //yield return Effects.CoFireworks(fireworksEff, 30, 0.1f, 0.25f);

            yield return new WaitForEndOfFrame();

            SoundController.Instance.SetBgmVolume(0f);

            yield return new WaitForSeconds(3f);

            EndingVideo();
        }
        
        protected override void InitAwake()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.E))
                .Subscribe(_ =>
                {
                    Ending();
                });
        }
        
        protected override void SkipIntro()
        {
            introSkip = true;
            openingCharacter.SetState(OpeningCharacter.IDLE);
        }
        
        #region # Unused Functions
        protected override void InitStart()
        {
        }


        protected override void SkipGame()
        {
        }
        #endregion
    }
}