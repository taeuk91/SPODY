using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Util;
using Util.Usefull;
using Random = UnityEngine.Random;

namespace SPODY_12_5_3
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

            if (name.Contains(teamManager.nowAnswer))
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

                    TurnOutLineOnOff();

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
                    
                    TurnOutLineOnOff();

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

        private void TurnOutLineOnOff()
        {
            GameObject upOutline =
                teamManager.stages[0].transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            upOutline.SetActive(true);
            upOutline.transform.DOScale(1.3f, 0.5f)
                .SetLoops(4, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    upOutline.SetActive(false);
                });
                    
            GameObject outline = transform.GetChild(0).gameObject;
            outline.SetActive(true);
            // tween = outline.GetComponent<SpriteRenderer>().DOFade(0, 0.3f)
            //     .SetLoops(4, LoopType.Yoyo)
            //     .OnComplete(() =>
            //     {
            //         outline.SetActive(false);
            //     });
            // tween.Restart();
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