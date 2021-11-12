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
using SPODY_8_7_3;
using UnityEngine.Serialization;
using Util;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using Random = UnityEngine.Random;

namespace SPODY_12_5_2
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
        public ParallaxScroller backGrounds;
        [SerializeField] private float bgMovingDuration = 3f;
        public Slider slider;
        public bool isTouched = false;
        [SerializeField] private List<string> questionStrKinds = new List<string>()
        {
            "연필", "풀", "가위"
        };
        
        [System.Serializable]
        public class QuetionAndSprite
        {
            public string quesion;
            public Sprite sprite;
            public GameObject obj;
        }

        public List<QuetionAndSprite> QuetionAndSprites = new List<QuetionAndSprite>();

        [SerializeField] private GameObject touchPrefab;

        [Header("Red Team--------------------")]
        public string answer;
        public List<Transform> redPositions;
        public int score = 0;
        public Text scoreTxt;

        [Header("Effect--------------------")] 
        public GameObject correctAnswerEff;
        public GameObject wrongAnswerEff;
        public GameObject fireworksEff;
        public GameObject smokeEff;


        [Header("Sounds--------------------")]
        public AudioClip[] answerClips;
        public AudioClip getScoreClip;
        public AudioClip[] goodClips;
        public AudioClip encourageClip;
        
        [Header("Property Test--------------------")]
        [NamedArray(typeof(LiquidType))]
        public Color[] liquidColors;

        [Space] public Text debug;

        public void SetDebug(string str)
        {
            debug.text = str;
        }
        public enum LiquidType
        {
            Water,
            Fuel,
            MotorOil
        };
        
        
        
        protected override void InitOnEnable()
        {
            scoreTxt.text = "0";
            
            openingCharacter.gameObject.SetActive(true);

            InitializeGame();
        }

        public HouseManager houseManager;


        // 화면 하단에 산타가 제자리에서 썰매 타고 날고 있고 배경 전체가 우에서
        // 좌로 움직이면서 문제(집)를 제시하고 정지함. 
        private Coroutine coMain;
        public Transform nowHouse;
        public void InitializeGame()
        {
            coMain = StartCoroutine(Sequence());
            
            IEnumerator Sequence()
            {
                houseManager.StartCoroutine(houseManager.ChangeHouse());
                
                yield return MoveBackground(bgMovingDuration);
                
                houseManager.TurnCloudsOn();
                
                yield return RocateAnswer();

                yield return RocateQuestions();
                
                Function.TouchEnable(true);
            }
        }
        

        [SerializeField] private Animator santaAnim;
        [SerializeField] private Animator dolphAnim;
        public int touchCnt = 0;
        IEnumerator MoveBackground(float duration)
        {
            backGrounds.isActive = true;

            dolphAnim.SetBool("isRunning", true);
            
            float currentTime = 0;

            while (true)
            {
                currentTime += Time.deltaTime;

                if (currentTime > duration)
                {
                    backGrounds.isActive = false;
                    dolphAnim.SetBool("isRunning", false);
                    backGrounds.multiplyIndex = 1;
                    
                    touchCnt = 0;
                    break;
                }
                
                // 연속 터치시 스피드 업!
                if (backGrounds.isActive && touchCnt > 1)
                {
                    backGrounds.multiplyIndex = 1; //touchCnt;
                }
                yield return null;
            }
        }

        // 문제는 1~5 중 하나의 숫자가 특정 장소에 위치
        [SerializeField] private Text questionTxt;
        [SerializeField] private Transform questionPos;
        public int answerNum;
        [SerializeField] private int minAnswerNum = 1;
        [SerializeField] private int maxAnswerNum = 5;
        IEnumerator RocateAnswer()
        {
            answerNum = Random.Range(minAnswerNum, maxAnswerNum) + 1;
            questionTxt.text = answerNum.ToString();

            yield return null;
        }
        
        // 4개의 썰매에 각각 1~4 랜덤의 선물들이 위치(총 12개의 선물을 미리 생성해둠)
        [SerializeField] private List<Transform> itemList;
        [SerializeField] private Transform itemParent;
        [SerializeField] private List<Transform> sled0PosList;
        [SerializeField] private List<Transform> sled1PosList;
        [SerializeField] private List<Transform> sled2PosList;
        [SerializeField] private List<Transform> sled3PosList;
        [SerializeField] private int minItemNum = 1;
        [SerializeField] private int maxItemNum = 4;
        [SerializeField] private List<int> numberPattern = new List<int>() {1, 2, 3, 4, 5};

        IEnumerator RocateQuestions()
        {
            int totalItemCount = 0;
            
            sled0PosList[0].parent.tag = "Player";
            sled1PosList[0].parent.tag = "Player";
            sled2PosList[0].parent.tag = "Player";
            sled3PosList[0].parent.tag = "Player";

            foreach (var item in itemList)
            {
                item.SetParent(itemParent);
                item.localPosition = Vector3.zero;
            }

            int temp = 0;
            while (true)
            {
                if (numberPattern[0] == answerNum)
                {
                    Function.ShuffleList(ref numberPattern);
                }
                else if (numberPattern[1] == answerNum)
                {
                    Function.ShuffleList(ref numberPattern);
                }
                else if (numberPattern[2] == answerNum)
                {
                    Function.ShuffleList(ref numberPattern);
                }
                else if (numberPattern[3] == answerNum)
                {
                    Function.ShuffleList(ref numberPattern);
                }
                else
                {
                    break;
                }
            }

            int whichSled = Random.Range(0, 4);
            switch (whichSled)
            {
                case 0:
                    RocateItemOnSled(sled0PosList, ref totalItemCount, answerNum);
                    RocateItemOnSled(sled1PosList, ref totalItemCount, numberPattern[0]);
                    RocateItemOnSled(sled2PosList, ref totalItemCount, numberPattern[1]);
                    RocateItemOnSled(sled3PosList, ref totalItemCount, numberPattern[2]);
                    break;
                case 1:
                    RocateItemOnSled(sled0PosList, ref totalItemCount, numberPattern[0]);
                    RocateItemOnSled(sled1PosList, ref totalItemCount, answerNum);
                    RocateItemOnSled(sled2PosList, ref totalItemCount, numberPattern[1]);
                    RocateItemOnSled(sled3PosList, ref totalItemCount, numberPattern[2]);
                    break;
                case 2:
                    RocateItemOnSled(sled0PosList, ref totalItemCount, numberPattern[0]);
                    RocateItemOnSled(sled1PosList, ref totalItemCount, numberPattern[1]);
                    RocateItemOnSled(sled2PosList, ref totalItemCount, answerNum);
                    RocateItemOnSled(sled3PosList, ref totalItemCount, numberPattern[2]);
                    break;
                case 3:
                    RocateItemOnSled(sled0PosList, ref totalItemCount, numberPattern[0]);
                    RocateItemOnSled(sled1PosList, ref totalItemCount, numberPattern[1]);
                    RocateItemOnSled(sled2PosList, ref totalItemCount, numberPattern[2]);
                    RocateItemOnSled(sled3PosList, ref totalItemCount, answerNum);
                    break;
            }
            
            yield return null;
        }

        private void RocateItemOnSled(List<Transform> sledPosList, ref int totalCount, ref int num)
        {
            int randNum = Random.Range(minItemNum, maxItemNum + 1);
            
            int tempCount = 0;
            foreach (var sledPos in sledPosList)
            {
                if (tempCount < randNum)
                {
                    itemList[totalCount].position = sledPos.position;
                    itemList[totalCount].SetParent(sledPos);
                    itemList[totalCount].localPosition = Vector3.zero;
                    itemList[totalCount].GetComponent<SpriteRenderer>().sortingOrder = randNum - tempCount;
                    
                    tempCount++;
                    totalCount++;
                }
            }
        }
        
        private void RocateItemOnSled(List<Transform> sledPosList, ref int totalCount, int answerNum)
        {
            int tempCount = 0;
            foreach (var sledPos in sledPosList)
            {
                if (tempCount < answerNum)
                {
                    itemList[totalCount].position = sledPos.position;
                    itemList[totalCount].SetParent(sledPos);
                    itemList[totalCount].localPosition = Vector3.zero;
                    
                    tempCount++;
                    totalCount++;
                }
            }
        }


        protected override IEnumerator INTRO()
        {
            yield return new WaitForEndOfFrame();
            Function.TouchEnable(false);
            
            while (openingCharacter.State)
            {
                if (introSkip)
                {
                    SoundController.Instance.PauseClip();
                    santaAnim.SetTrigger("SayHi");
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
                    santaAnim.SetTrigger("SayHi");

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
                    santaAnim.SetTrigger("SayHi");

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



        /// <summary>
        /// 기존에 스폰되었던 모든 클론들을 제거한다.
        /// </summary>
        /// <param name="teamBlankParent"> 스폰된 teamBlank 클론들의 부모 </param>
        /// <param name="teamPositions"> 스폰된 클론들의 부모 리스트 </param>
        /// <returns></returns>
        private IEnumerator ResetSpawnedObjects(Transform teamBlankParent, List<Transform> teamPositions, Transform objParent)
        {
            for (int i = 0; i < teamBlankParent.childCount; i++)
            {
                Destroy(teamBlankParent.GetChild(i).gameObject);
            }

            for (int i = 0; i < teamPositions.Count; i++)
            {
                if (teamPositions[i].childCount > 0)
                {
                    Destroy(teamPositions[i].GetChild(0).gameObject);
                }
            }

            for (int i = 0; i < objParent.childCount; i++)
            {
                Destroy(objParent.GetChild(i).gameObject);
            }

            yield return null;
        }

        // 레드팀 중간 위치 : -4.65, 2.38, 블루팀 중간 위치 : 4.65, 2.38
        // 음절 사이의 거리 : 1.45
        // 빈칸이 중간위치 기준으로 음절사이의 거리만큼 띄워서 정렬한다.
        // spacing - -4.65 + spacing
        // 수정해야함.
        private Vector3 blankCenterPos = new Vector3(-4.65f, 2.38f);
        private float blankSpacing = 1.45f;
        private IEnumerator RocateBlanks(GameObject blankObj, string answer, Transform syllableParent)
        {
            int syllableNum = answer.Length;
            Vector3 pos = new Vector3((blankSpacing * 0.5f) + -blankSpacing * (syllableNum - 1), 0);

            if (syllableNum == 1)
            {
                GameObject syllable = Instantiate(blankObj, syllableParent);
                syllable.transform.localPosition = Vector3.zero;

                Text syllableTxt = syllable.GetComponentInChildren<Text>();
                SetSyllable(syllableTxt, answer[0]);
            }
            else if (syllableNum > 1)
            {
                for (int i = 1; i <= syllableNum; i++)
                {
                    GameObject syllable = Instantiate(blankObj, syllableParent);
 
                    syllable.transform.localPosition = pos + new Vector3(blankSpacing * (i - 1), 0);

                    Text syllableTxt = syllable.GetComponentInChildren<Text>();

                    // 첫 번째 글자는 안보이게 하기
                    if (i == 1)
                    {
                        //syllableTxt.color = Color.clear;
                        syllable.transform.DOScale(1.3f, 0.5f)
                            .SetLoops(-1, LoopType.Yoyo);
                        syllable.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
                    }
                    // 첫 번째 이후 글자는 투명하게 하기
                    else
                    {
                        syllableTxt.color = new Color(1,1,1,0.5f);
                        syllable.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
                    }

                    SetSyllable(syllableTxt, answer[i - 1]);
                }
            }
            yield return null;
        }

        private void SetSyllable(Text textUI, char syllable)
        {
            textUI.text = syllable.ToString();
        }

        /// <summary>
        /// 질문 리스트(questionStrList)를 섞은 후, 리스트의 첫번째 문자열을 정답이라고 한다.
        /// </summary>
        /// <param name="List<QuetionAndSprite>"> 원하는 질문들의 리스트(ex. 연필, 풀, 가위) </param>
        /// <param name="answer"> 팀의 정답이 담기는 변수 </param>
        private void ResetAnswer(List<QuetionAndSprite> QuetionAndSprites, ref string answer, Transform objParent, string teamName)
        {
            Function.ShuffleList(ref QuetionAndSprites);
            
            while (answer == QuetionAndSprites[0].quesion)
                Function.ShuffleList(ref QuetionAndSprites);

            answer = QuetionAndSprites[0].quesion;
            GameObject obj = Instantiate(QuetionAndSprites[0].obj, objParent);
            obj.transform.DOScale(1.05f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
            
            if (teamName == "Red")
                obj.transform.position = Vector3.zero + new Vector3(-1.5f, 2);

            else
                obj.transform.position = Vector3.zero + new Vector3(7.5f, 2);
        }

        IEnumerator SetQuetionsInList(List<QuetionAndSprite> quetionAndSprites)
        {
            Function.ShuffleList(ref questionStrKinds);
            foreach (var quetionAndSprite in quetionAndSprites)
            {
                questionStrKinds.Add(quetionAndSprite.quesion);
            }
            
            yield return null;
        }

        /// <summary>
        /// 정답 프리펩을 생성하고, 그 위에 정답 텍스트를 한 음절씩 설정한다.
        /// </summary>
        /// <param name="prefab"> Spawn될 프리펩 </param>
        /// <param name="positions"> 프리펩이 Spawn 되어야 할 Positions </param>
        /// <param name="answer"> 팀별 정답 문자열 변수 </param>
        /// <param name="teamName"> 터치 오브젝트의 변수 team을 설정하기 위한 변수 </param>
        private int redTempNum = 0;
        private int blueTempNum = 0;
        private IEnumerator RocateAnswerObjects(GameObject prefab, List<Transform> positions, string answer, string teamName)
        {
            Function.ShuffleList(ref positions);
            
            char[] splitedAnswer = SplitAnswerString(answer);
            

            for (int i = 0; i < positions.Count; i++)
            {
                if (i >= splitedAnswer.Length)
                    break;

                if (positions[i].childCount == 0)
                {
                    GameObject answerSylabbleObj = Instantiate(prefab, positions[i].parent);
                    answerSylabbleObj.tag = "Player";
                    answerSylabbleObj.transform.localPosition = Vector3.zero;
                
                    answerSylabbleObj.transform.SetParent(positions[i]);
                    answerSylabbleObj.transform.DOMove(positions[i].position, 1f)
                        .OnComplete(() =>
                        {
                            answerSylabbleObj.GetComponent<BoxCollider>().enabled = true;
                        });
                }
            }

            if (teamName == "Red")
                redTempNum = positions.Count - splitedAnswer.Length;
            else if (teamName == "Blue")
                blueTempNum = positions.Count - splitedAnswer.Length;
                
            yield return null;
        }

        /// <summary>
        /// 정답이 아닌 프리펩을 생성하고, 그 위에 정답 텍스트를 한 음절씩 설정한다.
        /// </summary>
        /// <param name="prefab"> Spawn될 프리펩  </param>
        /// <param name="questionStrList"> 원하는 질문들의 리스트(ex. 연필, 풀, 가위) </param>
        /// <param name="positions"> 프리펩이 Spawn 되어야 할 Positions </param>
        /// <param name="asnwer"> 팀별 정답 문자열 변수 </param>
        /// <param name="teamName"> 터치 오브젝트의 변수 team을 설정하기 위한 변수 </param>
        private IEnumerator RocateWrongObjects(GameObject prefab, List<string> questionStrList, List<Transform> positions, string asnwer, string teamName)
        {
            //Function.ShuffleList(ref positions);

            List<char> splitedAnswer = AddCharFromStrings(questionStrList, asnwer);

            int objNum = 0;
            if (teamName == "Red")
                objNum = positions.Count - redTempNum;
            else if (teamName == "Blue")
                objNum = positions.Count - blueTempNum;
            
            for (int i = objNum; i < positions.Count; i++)
            {
                GameObject answerSylabbleObj = Instantiate(prefab,  positions[i].parent);

                if (positions[i].childCount == 0)
                {
                    answerSylabbleObj.tag = "Player";
                    answerSylabbleObj.transform.localPosition = Vector3.zero;
                
                
                    answerSylabbleObj.transform.SetParent(positions[i]);
                    answerSylabbleObj.transform.DOMove(positions[i].position, 1f)
                        .OnComplete(() =>
                        {
                            answerSylabbleObj.GetComponent<BoxCollider>().enabled = true;
                        });
                
                    Function.ShuffleList(ref splitedAnswer);
                }
            }
            yield return null;
        }

        /// <summary>
        /// answer 문자열에서 음절들을 분리하여 리스트로 만들어 그 리스트를 리턴한다.
        /// </summary>
        /// <param name="questionStrList"> 원하는 질문들의 리스트(ex. 연필, 풀, 가위) </param>
        /// <param name="answer"> 팀별 정답 문자열 변수 </param>
        /// <returns></returns>
        private List<char> AddCharFromStrings(List<string> questionStrList, string answer)
        {
            List<char> splited = new List<char>();
            List<char> totalCharList = new List<char>();

            foreach (var str in questionStrList)
            {
                if(str == answer)
                    continue;
                
                splited = str.ToList();
                totalCharList.AddRange(splited);
            }
            
            return totalCharList;
        }

        
        /// <summary>
        /// answer 문자열에서 음절들을 분리하여 배열로 만들어 그 배열을 리턴한다.
        /// </summary>
        /// <param name="answer"> 팀별 정답 문자열 변수 </param>
        /// <returns></returns>
        private char[] SplitAnswerString(string answer)
        {
            char[] syllables = answer.ToCharArray();

            return syllables;
        }
        

        protected override IEnumerator INGAME()
        {
            print("게임시작");
            santaAnim.SetTrigger("SayHi");

            openingCharacter.gameObject.SetActive(false);
            isGameStarted = true;
            isHowToPlayMode = false;
            
            // HowToPlayMode를 껐을 때
            if (!isHowToPlayMode)
            {
                // red_TeamManager.Initialization();
                // blue_TeamManager.Initialization();
            }
            else
            {
                // yield return blue_TeamManager.IESpawnKids();
                //
                // red_TeamManager.score = 0;
                // red_TeamManager.scoreUI.text = "0";
                //
                // blue_TeamManager.score = 0;
                // blue_TeamManager.scoreUI.text = "0";
            }
            
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

            endingObject.SetActive(true);
            
            yield return new WaitForSeconds(3f);

            endingObject.SetActive(false);
            
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