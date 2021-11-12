using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Util.Usefull;
using Random = UnityEngine.Random;

namespace SPODY_12_5_2
{
    public class TouchObject : CharacterBehavior
    {
        private GameManager gm;
        
        public override bool IsTouch { get; set; }
        public override bool IsAnim { get; set; }
        public override Vector3 hitPoint { get; set; }

        Tween tween;
        
        // 터치를 한 썰매가 정답과 같은 수의 선물을 가지고 있으면 Good 아니면 Bad! 
        public override void Touch()
        {
            gm = GameManager.Instance;

            int presentNum = 0;
            for (int i = 0; i < transform.childCount; i++)
                presentNum += transform.GetChild(i).childCount;
            
            // 연속으로 정답이라면, 초기화 시간이 늘어나고, 속도가 빨라짐
            if(presentNum == gm.answerNum)
            {
                tag = "Untagged";
                
                Function.TouchEnable(false);
                
                #region # Effects & Sounds
                Instantiate(gm.fireworksEff, transform.position, Quaternion.identity);
                gm.correctAnswerEff.transform.position = transform.position;
                gm.correctAnswerEff.SetActive(true);
                SoundController.Instance.PlayClip(gm.goodClips[Random.Range(0, gm.goodClips.Length)]);
                #endregion

                StartCoroutine(NumberCountOnOffItem());

                if (gm.touchCnt == 0)
                {
                    
                    CreateNewHouse();
                    
                    gm.InitializeGame();
                }
                    
                
                if(gm.slider.value < gm.slider.maxValue)
                    gm.slider.value++;    
                
                ++gm.touchCnt;
                gm.scoreTxt.text = (++Winner.red_team_score).ToString();
            }
            else
            {
                #region # Effects & Sounds    
                gm.wrongAnswerEff.transform.position = transform.position;
                gm.wrongAnswerEff.SetActive(true);
                SoundController.Instance.PlayClip(gm.encourageClip);
                #endregion
            }
        }

        IEnumerator NumberCountOnOffItem()
        {
            WaitForSeconds waitTime = new WaitForSeconds(0.1f);
            
            for (int i = transform.childCount - 1 ; i >= 0; i--)
            {
                if (transform.GetChild(i).childCount > 0)
                {
                    Transform sled = transform.GetChild(i);
                    Transform item = sled.GetChild(0);
                    Transform nameIndication = item.GetChild(0);
                    Transform nameText = nameIndication.GetChild(0);
                    Text textObj = nameText.GetComponent<Text>();
                    textObj.text = (transform.childCount - i).ToString();
                    textObj.gameObject.SetActive(true);
                    
                    yield return waitTime; 
                }
            }
            
            for (int i = transform.childCount - 1 ; i >= 0; i--)
            {
                if (transform.GetChild(i).childCount > 0)
                {
                    Transform sled = transform.GetChild(i);
                    Transform item = sled.GetChild(0);
                    Transform nameIndication = item.GetChild(0);
                    Transform nameText = nameIndication.GetChild(0);
                    Text textObj = nameText.GetComponent<Text>();
                    textObj.text = (transform.childCount - i).ToString();
                    textObj.gameObject.SetActive(false);
                }
            }
        }

        private void CreateNewHouse()
        {
            Transform house = Instantiate(gm.nowHouse);
            //house.SetParent(gm.houseManager.transform.parent);
            house.position =gm.houseManager.houseList[gm.houseManager.cnt-1].transform.position;
            
            
            house.transform.DOLocalMove(house.transform.position + Vector3.right * 15, 1)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Destroy(house.gameObject);
                });

            Instantiate(gm.smokeEff, gm.houseManager.cloudList[2].position, Quaternion.identity);
        }


        #region # Unused Functions
        public override void PlayClip()
        {
            throw new System.NotImplementedException();
        }
        public override void Active(bool active)
        {
            throw new System.NotImplementedException();
        }

        public override void Move(bool isPause)
        {
            throw new System.NotImplementedException();
        }
        public override void PlayAnim(string trigger)
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}