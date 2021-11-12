using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Util;
using Util.Usefull;
using Random = UnityEngine.Random;

namespace SPODY_12_6_1
{
    public class TouchObject : CharacterBehavior
    {
        public TeamManager teamManager;
        private GameManager gm;
        
        public override bool IsTouch { get; set; }
        public override bool IsAnim { get; set; }
        public override Vector3 hitPoint { get; set; }

        Tween tween;
        public override void Touch()
        {
            gm = GameManager.Instance;

            if (teamManager.nowAnswer.Contains(name))
            {
                #region # Effects & Sounds
                Instantiate(gm.fireworksEff, transform.position, Quaternion.identity);
                gm.correctAnswerEff.transform.position = transform.position;
                gm.correctAnswerEff.SetActive(true);
                SoundController.Instance.PlayClip(gm.goodClips[Random.Range(0, gm.goodClips.Length)]);
                #endregion

                if (name.Contains("산_눈썰매") || name.Contains("산_스키") )
                {
                    teamManager.nowAnim.SetTrigger("is산");
                    Vector3 originKidPos = teamManager.nowAnim.transform.position;
                    teamManager.nowAnim.transform.DOMove(teamManager.posToMove.position, 1)
                        .OnComplete(()=> teamManager.nowAnim.transform.position = originKidPos);
                }
                else if (name.Contains("얼음_썰매"))
                {
                    teamManager.nowAnim.SetTrigger("is얼음");
                    Vector3 originKidPos = teamManager.nowAnim.transform.position;
                    teamManager.nowAnim.transform.DOMove(teamManager.posToMove.position, 1)
                        .OnComplete(()=> teamManager.nowAnim.transform.position = originKidPos);
                }
                else if (name.Contains("얼음_팽이"))
                {
                    teamManager.nowAnim.SetTrigger("is얼음");
                }
                
                if (teamManager.name.Contains("Red"))
                {
                    gm.redTouchCount++;
                    tag = "Untagged";
                    GetComponent<SpriteRenderer>().enabled = false;
                    
                    if (gm.redTouchCount >= teamManager.answerNum)
                    {
                        gm.redTouchCount = 0;

                        StartCoroutine(CoDelayReset());
                        IEnumerator CoDelayReset()
                        {
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
                    

                    if (gm.blueTouchCount >= teamManager.answerNum)
                    {
                        Function.ShuffleList(ref teamManager.clips);
                        foreach (var clip in teamManager.clips)
                        {
                            if (clip.name.Contains(name))
                            {
                                SoundController.Instance.PlayClip(clip);
                                break;
                            }
                        }

                        if (gm.blueTouchCount >= teamManager.answerPos.Count)
                        {
                            gm.blueTouchCount = 0;

                            StartCoroutine(CoDelayReset());
                            IEnumerator CoDelayReset()
                            {
                                yield return new WaitForSeconds(1);
                                teamManager.Reset();
                            }
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