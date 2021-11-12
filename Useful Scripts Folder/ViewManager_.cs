using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Util.Usefull;


using Sirenix.OdinInspector;
using UnityEditor;

namespace Util
{
    public struct Winner
    {
        public enum TEAM : int
        {
            RED_TEAM, BLUE_TEAM, BOTH_TEAM,
        }

        public static int red_team_score { get; set; }
        public static int blue_team_score { get; set; }
    }

    public class ViewManager_ : MonoBehaviour
    {
        [Title("Touch Image")]
        public GameObject rootObject;
        public GameObject hitEffectImage;

        private PopupPanel_ PopupPanel_;
        private PopupPanel_ vs_Result_panel;

        [Title("If You Don't Use Vs\"Mode\", Just Keep Empty")]
        public PopupPanel_ vs_Result_panel_right;
        public PopupPanel_ vs_Result_Draw;

        [Title("Get Touch Coordinate")]
        public bool check_for_touch_coordinate = false;

        [Title("One / Two Camera")]
        public bool isOneCamera = true;
        [SerializeField]private Camera nowCam;
        [SerializeField]private Camera redCamera;
        [SerializeField]private Camera blueCamera;
        

        private void Awake()
        {
            GameManager.Instance.ViewManager_ = this;
        }

        private void OnEnable()
        {
            UIManager uiManager = GameObject.FindObjectOfType<UIManager>();

#if EDU
            PopupPanel_ = uiManager.singlePanelEdu;
            vs_Result_panel = uiManager.versusPanelEdu;
#elif EXP
            
            PopupPanel_ = uiManager.singlePanelExp;
            vs_Result_panel = uiManager.versusPanelExp;
#endif
        }

        private void Start()
        {
            Function.SensorReset();
            Function.SensorInit(new UnityAction<int, int>(BallClick));

            switch (isOneCamera)
            {
                case true:
                    nowCam = Camera.main;
                    break;
            }
        }

        public Ray ray;
        public RaycastHit hit;
        private void BallClick(int x, int y)
        {
            if (hitEffectImage == null || rootObject.activeSelf == false)
            {
                return;
            }

            var obj = Instantiate(hitEffectImage, rootObject.transform);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);

            switch (isOneCamera)
            {
                case true:
                    nowCam = Camera.main;
                    break;
                case false:

#if ASTRA
                    if (rect.anchoredPosition.x < Screen.width / 2)
#elif KINECT
                    if (rect.anchoredPosition.x < -320)
#endif
                    {
                        nowCam = redCamera;
                    }
                    else
                    {
                        nowCam = blueCamera;
                    }
                    break;
            }
            
            ray = nowCam.ScreenPointToRay(rect.position);
            
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(nowCam.transform.position, ray.direction * 100.0f, Color.red, 3.0f);

                    var charBehavior = hit.collider.GetComponent<CharacterBehavior>();

                    if (isOneCamera)
                    {
                        charBehavior.hitPoint = hit.point;
                    }
                    else
                    {
                        charBehavior.hitPoint = ray.direction;
                    }
                    
                    charBehavior.Touch();
                    
                    obj.SetActive(false);
                }
            }

            if (check_for_touch_coordinate)
            {
                GameManager.Instance.SetBallPosition(obj.transform.localPosition);
            }
        }


        public enum VIEW
        {
            MAIN,
            TIME,
            TIMEVS,
            GAME_TIME,
            GAME_TIMEVS,
            GAME_INFI,  // 없으면, 그냥 빼놔.. 굳이 채우지 말고..
        }

        [Title("MAIN, TIME, TIMEVS / GAME_TIME, GAME_TIMEVS, GAME_INFI")]
        public GameObject[] ViewList;

        public void SetPopupGameEnd(string score, VIEW view)
        {
            rootObject.SetActive(false);
            PopupPanel_.RetryMode = view;
            vs_Result_panel.RetryMode = view;
            PopupPanel_.gameObject.SetActive(true);
            PopupPanel_.text.text = score;
            PopupPanel_.text.transform.GetChild(0).GetComponent<Text>().text = score;
        }

        public void SetPopupPanelOff()
        {
            PopupPanel_.gameObject.SetActive(false);
        }

        public void SetPopupVsGameEnd(VIEW view)
        {
            rootObject.SetActive(false);
            PopupPanel_.RetryMode = view;
            vs_Result_panel.RetryMode = view;

            if (vs_Result_panel_right)
            {
                if(Winner.red_team_score > Winner.blue_team_score)
                {
                    vs_Result_panel.gameObject.SetActive(true);
                }
                else if(Winner.red_team_score < Winner.blue_team_score)
                {
                    vs_Result_panel_right.gameObject.SetActive(true);
                }
                else
                {
                    vs_Result_Draw.gameObject.SetActive(true);
                }
            }
            else
            {
                vs_Result_panel.gameObject.SetActive(true);
            }
        }

        private void SetViewMainAndSub(int view, bool visible)
        {
            if (ViewList[view] != null)
                ViewList[view].SetActive(visible);
        }

        public void SetView(VIEW view)
        {
            switch (view)
            {
                case VIEW.GAME_TIME:
                case VIEW.GAME_TIMEVS:
                case VIEW.GAME_INFI:
                    SetViewMainAndSub((int)view, false);
                    GameManager.Instance.DataManager_.InitGame();
                    PopupPanel_.RetryMode = view;
                    vs_Result_panel.RetryMode = view;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < ViewList.Length; i++)
            {
                SetViewMainAndSub(i, false);
            }

            for (int i = 0; i < ViewList.Length; i++)
            {
                if (i.Equals((int)view))
                {
                    SetViewMainAndSub(i, true);
                    break;
                }
            }
        }

        // select menu
        public void OnClickMainTime()
        {
            SetView(VIEW.TIME);
        }

        public void OnClickMainTimeVs()
        {
            SetView(VIEW.TIMEVS);
        }

        public void OnClickHome()
        {
            rootObject.SetActive(false);
            SetView(VIEW.MAIN);
        }

        // Time (Solo Version)
        public void OnClickGame()
        {
            SetView(VIEW.GAME_INFI);
        }

        // Time (Single Version)
        public void OnClickInputTime(int time)
        {
            if (time > 0)
            {
                GameManager.Instance.DataManager_.TimeMax = time;
            }

            SetView(VIEW.GAME_TIME);
        }

        // Time (Vs Version)
        public void OnClickInputTimeVs(int time)
        {
            GameManager.Instance.DataManager_.TimeMax = time;
            SetView(VIEW.GAME_TIMEVS);
        }
    }
}