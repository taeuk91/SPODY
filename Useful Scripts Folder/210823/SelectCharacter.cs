using DG.Tweening;
using SPODY.Team;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util.Usefull.Game
{
    public class SelectCharacter : CharacterBehavior
    {
        public Team team;

        public bool isAnswer;

        private SelectGame myController;
        public SelectGame Controller
        {
            get
            {
                return myController;
            }

            set
            {
                myController = value;
            }
        }

        [SerializeField] private Vector3 characterScale;

        [SerializeField] private GameObject answerFx;
        [SerializeField] private GameObject wrongFx;

        #region CharacterBehavior

        public override bool IsTouch { get; set; }
        public override bool IsAnim { get; set; }
        public override Vector3 hitPoint { get; set; }

        protected void OnEnable()
        {
            IsTouch = false;

            transform.localScale = Vector3.zero;

            answerFx.SetActive(false);
            wrongFx.SetActive(false);
        }

        public override void Active(bool active)
        {
            DOTween.Kill(GetInstanceID());

            Vector3 scale = active ? characterScale : Vector3.zero;
            Ease ease = active ? Ease.OutBack : Ease.InBack;

            transform.DOScale(scale, .5f)
            .SetEase(ease)
            .SetId(GetInstanceID())
                .OnComplete(() =>{
                    if (!active)
                    {
                        gameObject.SetActive(false);
                    }
                });
        }

        public override void Move(bool isPause)
        {

        }

        public override void PlayAnim(string trigger)
        {

        }

        public override void PlayClip()
        {

        }

        public override void Touch()
        {
            if (isAnswer && IsTouch) return;

            IsTouch = true;

            answerFx.SetActive(false);
            wrongFx.SetActive(false);

            answerFx.SetActive(isAnswer);
            wrongFx.SetActive(!isAnswer);

            if (isAnswer)
            {
                Controller.UpScore();
                Active(false);
            }

        }

        #endregion
    }
}


