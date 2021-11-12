using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Util;
using Util.Usefull;
using Random = UnityEngine.Random;

namespace SPODY_12_5_1
{
    public class TouchObject : CharacterBehavior
    {
        public TeamManager teamManager;
        private GameManager gm;
        
        public override bool IsTouch { get; set; }
        public override bool IsAnim { get; set; }
        public override Vector3 hitPoint { get; set; }

        public override void Touch()
        {
            gm = GameManager.Instance;

            GameObject kid1 = teamManager.stages[teamManager.nowStage - 1].transform.GetChild(0).gameObject;
            GameObject kid2 = teamManager.stages[teamManager.nowStage - 1].transform.GetChild(1).gameObject;

            Transform icicleGroup = new RectTransform();
            if (teamManager.stages[teamManager.nowStage - 1].name.Contains("Inside"))
            {
                icicleGroup = teamManager.stages[teamManager.nowStage - 1].transform.GetChild(2);
            }
            
            kid1.SetActive(false);
            kid2.SetActive(true);

            bool isAnswer = false;
            for (int i = 0; i < kid2.transform.childCount; i++)
            {
                string childName = kid2.transform.GetChild(i).name;
                if (name == childName)
                {
                    kid2.transform.GetChild(i).gameObject.SetActive(true);

                    if (teamManager.stages[teamManager.nowStage - 1].name.Contains("Inside"))
                    {
                        icicleGroup.GetChild(i).gameObject.SetActive(false);
                    }

                    isAnswer = true;
                }
            }
            
            if (isAnswer)
            {
                #region # Effects & Sounds
                Instantiate(gm.fireworksEff, transform.position, Quaternion.identity);
                gm.correctAnswerEff.transform.position = transform.position;
                gm.correctAnswerEff.SetActive(true);
                SoundController.Instance.PlayClip(gm.goodClips[Random.Range(0, gm.goodClips.Length)]);
                #endregion
                
                if (teamManager.name.Contains("Red"))
                {
                    gm.redTouchCount++;
                    tag = "Untagged";
                    GetComponent<SpriteRenderer>().enabled = false;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    
                    if (gm.redTouchCount >= teamManager.nowAnswer.Count)
                    {
                        gm.redTouchCount = 0;

                        StartCoroutine(CoDelayReset());
                        
                        IEnumerator CoDelayReset()
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Instantiate(gm.fireworksEff, teamManager.fireworksPos[i].position, Quaternion.identity);
                                yield return new WaitForSeconds(0.1f);
                            }
                            
                            yield return new WaitForSeconds(1);
                            teamManager.Reset();
                        }
                    }

                    gm.redScoreTxt.text = (++Winner.red_team_score).ToString();
                }
                else if (teamManager.name.Contains("Blue"))
                {
                    gm.blueTouchCount++;
                    tag = "Untagged";
                    GetComponent<SpriteRenderer>().enabled = false;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    
                    if (gm.blueTouchCount >= teamManager.nowAnswer.Count)
                    {
                        gm.blueTouchCount = 0;

                        StartCoroutine(CoDelayReset());
                        
                        IEnumerator CoDelayReset()
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Instantiate(gm.fireworksEff, teamManager.fireworksPos[i].position, Quaternion.identity);
                                yield return new WaitForSeconds(0.1f);
                            }
                            
                            yield return new WaitForSeconds(1);
                            teamManager.Reset();
                        }
                    }

                    gm.blueScoreTxt.text = (++Winner.blue_team_score).ToString();
                }

               
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


        public override void PlayClip()
        {
            throw new System.NotImplementedException();
        }
        
        #region # Unused Functions
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