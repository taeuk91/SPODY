using System.Collections;

using UnityEngine;
using UnityEngine.Events;



namespace Util.Usefull
{
    public class DelayEvent : MonoBehaviour
    {
        [Header("#Input Events")]
        public UnityEvent _QuickEvent;
        public UnityEvent _DelayEvent;

        [Header("#Input Delay Time")]
        public float delayTime;
        
        private bool isClick = false;


        public void DoEvent()
        {
            if(isClick)
                return;

            isClick = true;

            StartCoroutine(Do());
            
            IEnumerator Do()
            {
                _QuickEvent.Invoke();
                yield return new WaitForSeconds(delayTime);

                _DelayEvent.Invoke();
                isClick = false;
            }
        }


    }
}