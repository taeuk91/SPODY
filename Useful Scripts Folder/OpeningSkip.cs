
using UnityEngine;
using UnityEngine.Events;


namespace Util
{
    public class OpeningSkip : MonoBehaviour
    {
        public UnityEvent init;

        public UnityEvent action_pass;
        public UnityEvent action_Opening;

        private void OnEnable()
        {
            init.Invoke();

            bool isOpening = PlayerPrefs.GetInt("Opening").Equals(Contents.GetState(Contents.STATE.SKIP));

            if (isOpening)
            {
                // Pass
                action_pass.Invoke();
            }
            else
            {
                // Opening
                action_Opening.Invoke();
            }
        }



    }
}