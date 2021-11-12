using UnityEngine;
using UnityEngine.UI;

namespace Kids
{
    public class PopupPanel : MonoBehaviour
    {
        public Text MsgText;
       // public ViewManager.VIEW RetryMode;
        public GameObject TimePanel;
        public GameObject TimeVsPanel;

        public void OnClickHome()
        {
            Invoke("DoOnClickHome", 0.5f);
        }

        public void DoOnClickHome()
        {
            //GameManager.Instance.ViewManager_Kids.SetView(ViewManager.VIEW.MAIN);
            gameObject.SetActive(false);
        }

        public void OnClickContinue()
        {
            Invoke("DoOnClickcontinue", 0.5f);
        }

        public void DoOnClickcontinue()
        {
            gameObject.SetActive(false);
        }

        public void OnClickRetry()
        {
            //GameManager.Instance.ViewManager.SetView(RetryMode);
            //gameObject.SetActive(false);
            Invoke("DoOnClickRetry", 0.5f);
        }

        public void DoOnClickRetry()
        {
            //GameManager.Instance.ViewManager_Kids.SetView(RetryMode);
            gameObject.SetActive(false);
        }
    }
}