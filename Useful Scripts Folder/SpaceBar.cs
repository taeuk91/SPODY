using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Util.Usefull;
using DG.Tweening;

using UniRx;
using UniRx.Triggers;

namespace Util
{
    public class SpaceBar : MonoBehaviour
    {
        [Range(0f, 3f), SerializeField]
        float speed;

        [Header("Components")]
        public DelayEvent _event;
        public DOTweenAnimation tweenAnim;
        public Image spaceBarImg;
        public AudioClip clip;

        private Transform m_Transform;

        [Header("Do or Not")]
        public OpeningAnimation opening;

        private bool isClick = false;
        private bool FillAble = false;

        private void OnEnable()
        {
            isClick = false;
            spaceBarImg.fillAmount = 0f;
        }

        // DOTWEEN Animation의 "IN"에 포함
        // OnComplete될 때 실행되는 메소드
        public void Active()
        {
            FillAble = true;
            m_Transform = GetComponent<Transform>();

            m_Transform.DOScale(m_Transform.localScale.x * 1.05f, .6f)
                .SetEase(Ease.InQuad)
                .SetLoops(-1, LoopType.Yoyo);


            this.UpdateAsObservable()
                .Where(_ => spaceBarImg.fillAmount >= 1f && !isClick)
                .Subscribe(_ =>
                {
                    isClick = true;
                    tweenAnim.DORestartById("OUT");

                    if (SoundController.Instance != null)
                    {
                        SoundController.Instance.PauseClip();
                    }

                    SoundController.Instance.PlayClip(clip);

                    if (opening != null)
                    {
                        Invoke(nameof(ActiveOpening), .6f);
                    }
                    if (_event != null)
                    {
                        Invoke(nameof(ActiveEvent), .6f);
                    }
                }).AddTo(this);
        }

        void ActiveOpening()
        {
            opening.Ending();
        }

        void ActiveEvent()
        {
            _event._QuickEvent.Invoke();
        }

        private void Update()
        {
            if(isClick)
                return;

            if (!FillAble)
                return;

            if(Input.GetKey(KeyCode.Space))
            {
                spaceBarImg.fillAmount += speed * Time.deltaTime;
            }
            else
            {
                spaceBarImg.fillAmount -= speed * .4f * Time.deltaTime;

                if(spaceBarImg.fillAmount < 0f)
                {
                    spaceBarImg.fillAmount = 0f;
                    return;
                }
            }
            
        }
    }
}