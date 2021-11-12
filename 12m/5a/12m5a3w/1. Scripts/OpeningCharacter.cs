using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DG.Tweening;

namespace SPODY_12_5_3
{
    public class OpeningCharacter : MonoBehaviour
    {
        public const string TALK = "Talk";
        public const string IDLE = "Idle";
        private AudioSource audioSource;
        
        [SerializeField] private Animator animator;

        private void Start()
        {
            // ReLoad 한다면 객체를 끈다.
            if (PlayerPrefs.GetInt("ReLoad").Equals(Contents.GetState(Contents.STATE.SKIP)))
            {
                this.gameObject.SetActive(false);
            }
        }

        public bool State
        {
            get;
            set;
        }
        
        public void SetState(string state)
        {
            State = true;
            //animator.Play(state);
            animator.SetTrigger(state);
            print(state);
            
            if(!state.Equals(TALK)) ChangeState(state);
        }

        void ChangeState(string state)
        {
            animator.SetTrigger(state);
            
            State = false;
            
            Color color = gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().DOColor(new Color(color.r, color.g, color.b, 0), 1f);
            gameObject.SetActive(false);
        }
    }
}