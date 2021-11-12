using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Util.Usefull;


namespace Util
{
    public class ControlPanel : MonoBehaviour
    {
        public RectTransform TopLeftImage;
        public RectTransform TopRightImage;
        public RectTransform BottomLeftImage;
        public RectTransform BottomRightImage;
        public Image[] LockOn;
        public SelectPanel menu;


        private RectTransform movementImage;

        public GameObject ControlUI;

        private void OnEnable()
        {
            menu.enabled = false;
            // SetRawImage();
            Setting();
            // ㅋㅋㅋ
            if (GameManager.Instance.ViewManager != null)
                GameManager.Instance.ViewManager.InitSetting();

            ControlUI.SetActive(true);
        }

        private void OnDisable()
        {
            menu.enabled = true;
            movementImage = null;

            Function.SensorReset();

            ControlUI.SetActive(false);

            for(int i=0; i<LockOn.Length; i++)
            {
                LockOn[i].enabled = false;
            }

            this.gameObject.SetActive(false);
        }

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => movementImage != null)
                .Subscribe(_ => ImageMovement(movementImage))
                .AddTo(gameObject);


            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
                .Subscribe(_ => Move1())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
                .Subscribe(_ => Move2())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
                .Subscribe(_ => Move3())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
                .Subscribe(_ => Move4())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha5))
                .Subscribe(_ => OnSaveButton())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.R))
                .Subscribe(_ => ResetObjects())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ =>
                {
                    this.gameObject.SetActive(false);
                    menu.gameObject.SetActive(true);
                })
                .AddTo(gameObject);
        }
        
        public void Move1()
        {             
            for (int i = 0; i < 5; i++)
            {
                if (i.Equals(0)) LockOn[i].enabled = true;
                else LockOn[i].enabled = false;
            }
            movementImage = TopLeftImage;
#if UNITY_EDITOR
            //print(movementImage.position + ", " + movementImage.anchoredPosition);
#endif
        }

        public void Move2()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i.Equals(1)) LockOn[i].enabled = true;
                else LockOn[i].enabled = false;
            }

            movementImage = TopRightImage;
        }

        public void Move3()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i.Equals(2)) LockOn[i].enabled = true;
                else LockOn[i].enabled = false;
            }

            movementImage = BottomLeftImage;
        }

        public void Move4()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i.Equals(3)) LockOn[i].enabled = true;
                else LockOn[i].enabled = false;
            }

            movementImage = BottomRightImage;
        }

        IEnumerator DelaySave()
        {
            yield return new WaitForSeconds(0.3f);

            LockOn[4].enabled = false;
        }

        void ImageMovement(RectTransform pos)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                pos.localPosition += new Vector3(0f, 3.0f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                pos.localPosition += new Vector3(-3.0f, 0f);
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                pos.localPosition += new Vector3(0f, -3.0f);
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                pos.localPosition += new Vector3(3.0f, 0f);
            }
        }

        private void ResetObjects()
        {
            BottomLeftImage.localPosition = new Vector3(256f, 192f);
            BottomRightImage.localPosition = new Vector3(768f, 192f);
            TopLeftImage.localPosition = new Vector3(256f, 576f);
            TopRightImage.localPosition = new Vector3(768f, 576f);

            for (int i = 0; i < 4; i++)
            {
                LockOn[i].enabled = false;
            }

            movementImage = null;
            Save();
        }

        // 0, 0, 1920,0
        // 0, -1080, 1920,-1080

        public void Setting()
        {
            Function.InitSetting(ref TopLeftImage, ref TopRightImage, ref BottomLeftImage, ref BottomRightImage);
        }

        

        public void OnSaveButton()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i.Equals(4)) LockOn[i].enabled = true;
                else LockOn[i].enabled = false;
            }
            movementImage = null;

            Save();
            StartCoroutine(DelaySave());
        }

        private void Save()
        {
            Function.SetSettingValue((int)TopLeftImage.localPosition.x, (int)TopLeftImage.localPosition.y, (int)TopRightImage.localPosition.x, (int)TopRightImage.localPosition.y,
                (int)BottomLeftImage.localPosition.x, (int)BottomLeftImage.localPosition.y, (int)BottomRightImage.localPosition.x, (int)BottomRightImage.localPosition.y);

            Function.SaveSettingMovel();
        }
    }
}