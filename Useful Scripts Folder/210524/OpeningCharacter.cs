using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Util
{
    public class OpeningCharacter : MonoBehaviour
    {
        public const string TALK = "Talk";
        public const string IDLE = "Idle";
        public const string WALKIN = "WalkIn";
        public const string WALKBACK = "WalkBack";

        private AudioSource audioSource;
        public AudioClip[] clips;
        
        [SerializeField] private Animator animator;

        private void Start()
        {
            // ReLoad 한다면 객체를 끈다.
            if (PlayerPrefs.GetInt("ReLoad").Equals(Contents.GetState(Contents.STATE.SKIP)))
            {
                this.gameObject.SetActive(false);
            }
        }

        public bool CheckState
        {
            get;
            set;
        }
        
        public void SetState(string state)
        {
            CheckState = true;
            
            animator.Play(state);
            animator.SetTrigger(state);
            print(state);

            if(!state.Equals(TALK))
                ChangeState(state);

        }

        void ChangeState(string state)
        {
            animator.SetTrigger(state);
            CheckState = false;
            Color color = gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().DOColor(new Color(color.r, color.g, color.b, 0), 1f);
            gameObject.SetActive(false);
        }
    }
}