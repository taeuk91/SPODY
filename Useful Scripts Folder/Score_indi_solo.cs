using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Util;

namespace Util
{
    public class Score_indi_solo : MonoBehaviour
    {
        [Header("BackGround")]
        public GameObject _Score_Panel;

        [Header("Score Text")]
        public Text Score;

        [Header("Audio Source")]
        public AudioSource _Count_Sound;
        public AudioSource _Cheer_Sound;
        public AudioSource _Stop_Sound;


        [Header("Buttons Parent")]
        public GameObject _button_parent;
        //public GameObject _button_1;
        //public GameObject _button_2;
        //public GameObject _button_3;

        [Header("Cham")]
        public GameObject _Cham;
        public float panelPosY = - 60;
        public float panelPosX = -1100;

        // USE THIS

        private void OnEnable()
        {
            Init();
            StartAnimation();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            winSequence.Kill();
            _Cheer_Sound.Stop();
        }

        Sequence winSequence;

        public void StartAnimation()
        {
            StartCoroutine(StartAnim());

            IEnumerator StartAnim()
            {
                yield return null;

                Time.timeScale = 1f;

                winSequence = DOTween.Sequence();

                winSequence.Append(_Score_Panel.transform.DOLocalMoveY(165f, 0.2f))
                                .SetEase(Ease.OutQuad)
                            .Append(_Cham.transform.DOLocalMoveX(0f, 0.2f)
                                .SetEase(Ease.OutQuad))
                            .OnComplete(() => TextAnimation());
            }
        }


        public void TextAnimation()
        {

            StartCoroutine(ScoreFlow());

            winSequence.Append(_button_parent.transform.DOLocalMoveY(-237f, 0.2f));

            IEnumerator ScoreFlow()
            {
                float t = 0f;


                while (t < 6f)
                {
                    t += .1f;

                    int red = Random.Range(10, 99);

                    Score.text = string.Format("{0}", red);

                    yield return null;
                }
                Score.text = string.Format("{0}", Winner.red_team_score);
            }
        }
        private void Init()
        {
            Score.text = "??"; 
            _Score_Panel.transform.localPosition = new Vector2(0f, 590f);

            _button_parent.transform.localPosition = new Vector2(0f, -538f);
            //_button_1.transform.localPosition = new Vector2(-160f, 30f);
            //_button_2.transform.localPosition = new Vector2(0f, 30f);
            //_button_3.transform.localPosition = new Vector2(160f, 30f);

            _Cham.transform.localPosition = new Vector2(panelPosX, panelPosY);
        }
    }
}