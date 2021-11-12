using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System.Collections;

using UnityEngine.Video;

namespace Util
{
    public class VsResultAnim : MonoBehaviour
    {
        //2021-08-19 SW  변경사항 점수판 R,B로 구분
        [Header("BackGround")]
        public GameObject _Red_backGround_panel;
        public GameObject _Blue_backGround_panel;
        public GameObject _Score_Panel_G;
        public GameObject _Score_Panel_R;
        public GameObject _Score_Panel_B;

        [Header("Score Text")]
        public Text _Red_Team_Score;
        public Text _Blue_Team_Score;

        public GameObject _Blind_Panel;
        public GameObject _Red_Team_Win_Panel;
        public GameObject _Blue_Team_Win_Panel;
        public GameObject _Both_Team_Draw_Panel;

        [Header("Audio Source")]
        public AudioSource _Count_Sound;
        public AudioSource _Cheer_Sound;
        public AudioSource _Stop_Sound;

        public AudioSource _Win_Voice;
        public AudioSource _Lose_Voice;

        //2021-08-19 SW  변경사항 캐릭터 이미지 우승만 남기고 나머지 지움
        [Header("Characters")]
        public Image _Red_team_character_image;
        public Image _Blue_team_character_image;
        //public GameObject _draw_character;


        [Header("Character Sprite, " +
                "0:Idle, 1:Win, 2:Lose")]
        public Sprite[] _Red_team_sprite;
        public Sprite[] _Blue_team_sprite;

        [Header("Buttons Parent")]
        public GameObject _button_parent;

        [Header("Score Image set")]
        public GameObject R_image;
        public GameObject B_image;

        [Header("Video Player")]
        public VideoPlayer m_VideoPlayer;

        [Header("Clip")]
        public VideoClip Ending;

        [Header("Back_G")]
        public GameObject BG;

        //2021-08-19 SW 변경사항 승패모션 없음
        //[Header("Win Result")]
        //public WinResult _winResult_Red;
        //public WinResult _winResult_Blue;

        //public GameObject _crown_red;
        //public GameObject _spotLight_red;

        //public GameObject _crown_blue;
        //public GameObject _spotLight_blue;

        //[Header("Lose Result")]
        //public GameObject _Dark_Image_red;
        //public GameObject _Dark_Image_blue;

        [Header("Cham")]
        public GameObject _Cham;


        // USE THIS
        private void OnEnable()
        {
            Init();
            StartAnimation();
            BG.SetActive(true);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            winSequence.Kill();
            _Cheer_Sound.Stop();
        }

        Sequence winSequence;

        public void StartAnimation()
        {

            StartCoroutine(StartAnim());

            IEnumerator StartAnim()
            {
                yield return null;

                Time.timeScale = 1f;

                winSequence = DOTween.Sequence();

                winSequence.Append(_Red_backGround_panel.transform.DOLocalMoveX(0f, 0.6f)
                                .SetEase(Ease.OutQuad))
                        .Join(_Blue_backGround_panel.transform.DOLocalMoveX(0f, 0.6f)
                                .SetEase(Ease.OutQuad))
                        .Append(_Score_Panel_G.transform.DOLocalMoveY(0f, .2f))
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() => {
                                    TextAnimation();
                                });
            }
        }

        
        public void TextAnimation()
        {
            
            StartCoroutine(ScoreFlow());

            _button_parent.transform.DOLocalMoveY(-250f, .2f);

            IEnumerator ScoreFlow()
            {
                float t = 0f;


                while(t < 6f)
                {
                    t += .1f;

                    int red = Random.Range(10, 99);
                    int blue = Random.Range(10, 99);

                    _Red_Team_Score.text = string.Format("{0}", red);
                    _Blue_Team_Score.text = string.Format("{0}", blue);

                    yield return null;
                }

                _Red_Team_Score.text = string.Format("{0}", Winner.red_team_score);
                _Blue_Team_Score.text = string.Format("{0}", Winner.blue_team_score);

                if (Winner.red_team_score >= 10)
                {
                    _Red_Team_Score.transform.localPosition = new Vector2(-62f, 0f);
                }
                else if (Winner.red_team_score < 10)
                {
                    _Red_Team_Score.transform.localPosition = new Vector2(-115f, 0f);
                }
                if (Winner.blue_team_score >= 10)
                {
                    _Blue_Team_Score.transform.localPosition = new Vector2(-7f, 0f);
                }
                else if (Winner.blue_team_score < 10)
                {
                    _Blue_Team_Score.transform.localPosition = new Vector2(-62f, 0f);
                }

                // # ADD
                yield return new WaitForSeconds(.7f);
                StartCoroutine(CharacterAnimation());
            }
        }

        private IEnumerator CharacterAnimation()
        {
            yield return new WaitForSeconds(.5f);

            if (Winner.red_team_score > Winner.blue_team_score)
            {
                int redTeam = 1;
                int blueTeam = 2;
                

                // RED TEAM WIN!!!
                _Stop_Sound.Play();
                _Blind_Panel.SetActive(true);
                R_image.transform.DOScale(new Vector3(1f, 1f, 0f), 0.2f);
                _Red_Team_Win_Panel.SetActive(true);
                _Red_Team_Win_Panel.transform.DOScale(Vector3.one, 0.2f)
                    .SetEase(Ease.OutBack);
                winSequence.Append(_Cham.transform.DOLocalMoveX(0f, 0.2f)
                                .SetEase(Ease.OutQuad));


                yield return new WaitForSeconds(.4f);

                _Lose_Voice.Play();
                //_Dark_Image_blue.transform.DOLocalMoveY(0f, 1f)
                    //.SetEase(Ease.OutQuad);

                //yield return new WaitForSeconds(.1f);
                //_Blue_team_character_image.sprite = _Blue_team_sprite[blueTeam];

                
                // Win Effect
                //yield return new WaitForSeconds(.8f);
                //_crown_red.SetActive(true);
                //yield return new WaitForSeconds(.1f);
                //_spotLight_red.SetActive(true);
                //_Win_Voice.Play();

                //yield return new WaitForSeconds(.1f);
                //_winResult_Red.StartSpreadPapers();
                
                //yield return new WaitForSeconds(.1f);
                //_Cheer_Sound.Play();

                // Button Movement

                while (this.enabled)
                {
                    //_Red_team_character_image.sprite = _Red_team_sprite[redTeam];
                    //yield return new WaitForSeconds(1.2f);

                    _Red_team_character_image.sprite = _Red_team_sprite[0];
                    yield return new WaitForSeconds(.4f);
                }
            }
            else if(Winner.red_team_score < Winner.blue_team_score)
            {
                int redTeam = 2;
                int blueTeam = 1;
 
                // RED TEAM WIN!!!
                 _Stop_Sound.Play();
                _Blind_Panel.SetActive(true);
                B_image.transform.DOScale(new Vector3(1f, 1f, 0f), 0.2f);
                _Blue_Team_Win_Panel.SetActive(true);
                _Blue_Team_Win_Panel.transform.DOScale(Vector3.one, 0.2f)
                    .SetEase(Ease.OutBack);
                winSequence.Append(_Cham.transform.DOLocalMoveX(0f, 0.2f)
                                .SetEase(Ease.OutQuad));

                yield return new WaitForSeconds(.4f);

                _Lose_Voice.Play();
                //_Dark_Image_red.transform.DOLocalMoveY(0f, 1f)
                   // .SetEase(Ease.OutQuad);

                //yield return new WaitForSeconds(.1f);
                //_Red_team_character_image.sprite = _Red_team_sprite[redTeam];


                // Win Effect
                //yield return new WaitForSeconds(.8f);
                //_crown_blue.SetActive(true);
                //yield return new WaitForSeconds(.1f);
                //_spotLight_blue.SetActive(true);
                //_Win_Voice.Play();
                //yield return new WaitForSeconds(.1f);
                //_winResult_Blue.StartSpreadPapers();
                //yield return new WaitForSeconds(.1f);
                //_Cheer_Sound.Play();

                // Button Movement

                while (this.enabled)
                {
                    //_Blue_team_character_image.sprite = _Blue_team_sprite[blueTeam];
                    //yield return new WaitForSeconds(1.2f);

                    _Blue_team_character_image.sprite = _Blue_team_sprite[0];
                    yield return new WaitForSeconds(.4f);                    
                }
            }
            else
            {
                // _Blue_team_character_image.sprite = _Blue_team_sprite[0];
                // _Red_team_character_image.sprite = _Red_team_sprite[0];
                //_draw_character.SetActive(true);
                R_image.transform.DOScale(new Vector3(1f, 1f, 0f), 0.2f);
                B_image.transform.DOScale(new Vector3(1f, 1f, 0f), 0.2f);
                _Both_Team_Draw_Panel.SetActive(true);
                _Both_Team_Draw_Panel.transform.DOScale(Vector3.one, 0.2f)
                    .SetEase(Ease.OutBack);

                _Blind_Panel.SetActive(true);
            }

            
        }
        private void Init()
        {
            _Red_team_character_image.sprite = _Red_team_sprite[0];
            _Blue_team_character_image.sprite = _Blue_team_sprite[0];
            //_draw_character.SetActive(false);
            _Red_Team_Score.text = "??";
            _Blue_Team_Score.text = "??";

            _Red_Team_Score.transform.localPosition = new Vector2(-62f, 0f);
            _Blue_Team_Score.transform.localPosition = new Vector2(-7f, 0f);

            //_crown_red.SetActive(false);
            //_crown_blue.SetActive(false);

            //_spotLight_red.SetActive(false);
            //_spotLight_blue.SetActive(false);

            _Red_backGround_panel.transform.localPosition = new Vector2(-640f, 0f);
            _Blue_backGround_panel.transform.localPosition = new Vector2(640f, 0f);
            //_Dark_Image_red.transform.localPosition = new Vector2(-340f, 768f);
            //_Dark_Image_blue.transform.localPosition = new Vector2(340f, 768f);
            _Score_Panel_G.transform.localPosition = new Vector2(0f, 0f);
            _button_parent.transform.localPosition = new Vector2(0f, -450f);

            _Blind_Panel.SetActive(false);
            _Red_Team_Win_Panel.transform.localScale = Vector3.zero;
            _Blue_Team_Win_Panel.transform.localScale = Vector3.zero;
            _Both_Team_Draw_Panel.transform.localScale = Vector3.zero;

            R_image.transform.DOScale(new Vector3(0.7f,0.7f,0f),0.1f);
            B_image.transform.DOScale(new Vector3(0.7f, 0.7f, 0f), 0.1f);

            _Cham.transform.localPosition = new Vector2(-1100f, -92f);
        }


    }
}