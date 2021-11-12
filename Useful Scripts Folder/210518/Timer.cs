using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util.Usefull;

namespace SPODY
{
    public class Timer : MonoBehaviour
    {
        public static Timer Instance;

        [SerializeField] private Text timerText;
        [SerializeField] private int gameTime;

        private int currentTime;

        private Coroutine timeCoroutine;

        private bool timeActive = true;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            ResetTimer();
            if(timeCoroutine != null) StopCoroutine(timeCoroutine);
        }

        private void ResetTimer()
        {
            currentTime = gameTime;
            timerText.text = Function.GetTime(currentTime);
        }

        public void StartTimer()
        {
            timeCoroutine = StartCoroutine(TimeCoroutine());
        }

        private IEnumerator TimeCoroutine()
        {
            while (!TimeOver())
            {
                TickTok();

                while(IsPause())
                {
                    yield return null;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        public void TimeActive(bool value)
        {
            timeActive = value;
        }

        public bool HalfTime()
        {
            return currentTime <= gameTime/2;
        }

        private bool IsPause()
        {
            return !timeActive;
        }

        private void TickTok()
        {
            currentTime -= 1;
            timerText.text = Function.GetTime(currentTime);
        }

        public bool TimeOver()
        {
            if (currentTime <= 0) return true;
            return false;
        }

    }
}


