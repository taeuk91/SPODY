using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System.Collections;

namespace Util
{
    public class VsResultAnim_Old : MonoBehaviour
    {
        [Header("BackGround")]
        public GameObject _Red_backGround_panel;
        public GameObject _Blue_backGround_panel;
        public GameObject _Score_Panel;

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
        

        [Header("Characters")]
        public Image _Red_team_character_image;
        public Image _Blue_team_character_image;
        public GameObject _draw_character;


        [Header("Character Sprite, " +
                "0:Idle, 1:Win, 2:Lose")]
        public Sprite[] _Red_team_sprite;
        public Sprite[] _Blue_team_sprite;

        [Header("Buttons Parent")]
        public GameObject _button_parent;

        [Header("Win Result")]
        public WinResult _winResult_Red;
        public WinResult _winResult_Blue;

        public GameObject _crown_red;
        public GameObject _spotLight_red;
        
        public GameObject _crown_blue;
        public GameObject _spotLight_blue;
        
        [Header("Lose Result")]
        public GameObject _Dark_Image_red;
        public GameObject _Dark_Image_blue;



        // USE THIS

        private void OnEnable()
        {
            Init();
            StartAnimation();
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

                winSequence.Append(_Red_backGround_panel.transform.DOLocalMoveX(0f, 1.3f)
                                .SetDelay(.2f)
                                .SetEase(Ease.OutQuad))
                        .Join(_Blue_backGround_panel.transform.DOLocalMoveX(0f, 1.3f)
                                .SetDelay(.2f)
                                .SetEase(Ease.OutQuad))
                        .Append(_Score_Panel.transform.DOLocalMoveY(0f, .6f))
                                .SetEase(Ease.OutQuad)
                                .OnComplete(()=> TextAnimation())
                                .PrependInterval(.3f);
            }
        }

        
        public void TextAnimation()
        {
            
            StartCoroutine(ScoreFlow());

            IEnumerator ScoreFlow()
            {
                float t = 0f;


                while(t < 6f)
                {
                    t += .1f;

                    int red = Random.Range(10, 99);
                    int blue = Random.Range(10, 99);

                    _Red_Team_Score.text = string.Format("빨강팀: {0}", red);
                    _Blue_Team_Score.text = string.Format("파랑팀: {0}", blue);

                    yield return null;
                }

                _Red_Team_Score.text = string.Format("빨강팀: {0}", Winner.red_team_score);
                _Blue_Team_Score.text = string.Format("파랑팀: {0}", Winner.blue_team_score);


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
                _Red_Team_Win_Panel.SetActive(true);
                _Red_Team_Win_Panel.transform.DOScale(Vector3.one, 1f)
                    .SetEase(Ease.OutBack);


                yield return new WaitForSeconds(.4f);

                _Lose_Voice.Play();
                _Dark_Image_blue.transform.DOLocalMoveY(0f, 1f)
                    .SetEase(Ease.OutQuad);

                yield return new WaitForSeconds(.1f);
                _Blue_team_character_image.sprite = _Blue_team_sprite[blueTeam];

                
                // Win Effect
                yield return new WaitForSeconds(.8f);
                _crown_red.SetActive(true);
                yield return new WaitForSeconds(.1f);
                _spotLight_red.SetActive(true);
                _Win_Voice.Play();

                yield return new WaitForSeconds(.1f);
                _winResult_Red.StartSpreadPapers();
                
                yield return new WaitForSeconds(.1f);
                _Cheer_Sound.Play();

                // Button Movement
                _button_parent.transform.DOLocalMoveY(-250f, .8f)
                    .SetDelay(1.3f);

                while (this.enabled)
                {
                    _Red_team_character_image.sprite = _Red_team_sprite[redTeam];
                    yield return new WaitForSeconds(1.2f);

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
                _Blue_Team_Win_Panel.SetActive(true);
                _Blue_Team_Win_Panel.transform.DOScale(Vector3.one, 1f)
                    .SetEase(Ease.OutBack);


                yield return new WaitForSeconds(.4f);

                _Lose_Voice.Play();
                _Dark_Image_red.transform.DOLocalMoveY(0f, 1f)
                    .SetEase(Ease.OutQuad);

                yield return new WaitForSeconds(.1f);
                _Red_team_character_image.sprite = _Red_team_sprite[redTeam];


                // Win Effect
                yield return new WaitForSeconds(.8f);
                _crown_blue.SetActive(true);
                yield return new WaitForSeconds(.1f);
                _spotLight_blue.SetActive(true);
                _Win_Voice.Play();
                yield return new WaitForSeconds(.1f);
                _winResult_Blue.StartSpreadPapers();

                

                yield return new WaitForSeconds(.1f);
                _Cheer_Sound.Play();

                // Button Movement
                _button_parent.transform.DOLocalMoveY(-250f, .8f)
                    .SetDelay(1.3f);

                while (this.enabled)
                {
                    _Blue_team_character_image.sprite = _Blue_team_sprite[blueTeam];
                    yield return new WaitForSeconds(1.2f);

                    _Blue_team_character_image.sprite = _Blue_team_sprite[0];
                    yield return new WaitForSeconds(.4f);                    
                }
            }
            else
            {
                // _Blue_team_character_image.sprite = _Blue_team_sprite[0];
                // _Red_team_character_image.sprite = _Red_team_sprite[0];
                _draw_character.SetActive(true);
                _Both_Team_Draw_Panel.SetActive(true);
                _Both_Team_Draw_Panel.transform.DOScale(Vector3.one, 1f)
                    .SetEase(Ease.OutBack);

                _Blind_Panel.SetActive(true);
                _button_parent.transform.DOLocalMoveY(-250f, .8f)
                    .SetDelay(1.0f);
            }

            
        }

        private void Init()
        {
            _Red_team_character_image.sprite = _Red_team_sprite[0];
            _Blue_team_character_image.sprite = _Blue_team_sprite[0];
            _draw_character.SetActive(false);
            _Red_Team_Score.text = "빨강팀: ??";
            _Blue_Team_Score.text = "파랑팀: ??";

            _crown_red.SetActive(false);
            _crown_blue.SetActive(false);

            _spotLight_red.SetActive(false);
            _spotLight_blue.SetActive(false);
            
            _Red_backGround_panel.transform.localPosition = new Vector2(-640f, 0f);
            _Blue_backGround_panel.transform.localPosition = new Vector2(640f, 0f);
            _Dark_Image_red.transform.localPosition = new Vector2(-340f, 768f);
            _Dark_Image_blue.transform.localPosition = new Vector2(340f, 768f);
            _Score_Panel.transform.localPosition = new Vector2(0f, 150f);
            _button_parent.transform.localPosition = new Vector2(0f, -450f);

            _Blind_Panel.SetActive(false);
            _Red_Team_Win_Panel.transform.localScale = Vector3.zero;
            _Blue_Team_Win_Panel.transform.localScale = Vector3.zero;
            _Both_Team_Draw_Panel.transform.localScale = Vector3.zero;
        }


    }
}