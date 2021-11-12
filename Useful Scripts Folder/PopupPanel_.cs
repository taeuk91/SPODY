using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Util.Usefull;


namespace Util
{
    public class PopupPanel_ : MonoBehaviour
    {
        public Text text;
        public ViewManager_.VIEW RetryMode;
        public AudioSource _BGM;

        public SceneChange sceneChange;

        private void OnEnable()
        {
            Function.TouchEnable(false);

            if(_BGM)
                _BGM.DOFade(.25f, 2f);
        }

        private void OnDisable()
        {
            Function.TouchEnable(true);

            if (_BGM)
                _BGM.DOFade(1f, 2f);
        }

        public void OnClickHome()
        {
            Invoke("DoOnClickHome", 0.2f);
        }

        public void OnClickContinue()
        {
            Invoke("DoOnClickContinue", 0.2f);
        }

        public void OnRetryMode()
        {
#if EDU
           sceneChange.RestartScene();
           return;
#endif
            Invoke("DoOnClickRetry", 0.2f);
        }

        private void DoOnClickHome()
        {
            GameManager.Instance.ViewManager_.SetView(ViewManager_.VIEW.MAIN);
            gameObject.SetActive(false);
        }

        private void DoOnClickContinue()
        {
            gameObject.SetActive(false);
        }

        private void DoOnClickRetry()
        {
            Function.TouchEnable(true);
            

            GameManager.Instance.ViewManager_.SetView(RetryMode);
            
            
            gameObject.SetActive(false);
        }


    }
}