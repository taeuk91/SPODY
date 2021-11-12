using System.Collections;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using DG.Tweening;

using log4net;
using UnityEngine.SceneManagement;


namespace Util.Usefull
{
    public class OpeningAnimation : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SPODY_7_7_4.GameManager));

        public GameObject MapObj;
        public GameObject CharacterObj;
        
        public Animator characterAnimator;


        public GameObject[] objs;
        public AudioClip titleClip;
        public AudioClip introClip;

        public AudioClip[] narrationClip;
        public AudioClip effectClip;


        public DOTweenAnimation fadeImage;

        private CharacterLineUp characterLineUp;


        bool isSkip = false;
        private void Awake()
        {
            if(PlayerPrefs.GetInt("ReLoad").Equals(Contents.GetState(Contents.STATE.SKIP)))
            {
                foreach (var v in objs)
                {
                    v.SetActive(true);
                }

                if (SoundController.Instance != null)
                {
                    SoundController.Instance.SetBgmVolume(1f);
                }

                this.gameObject.SetActive(false);

                return;
            }
            characterLineUp = GetComponent<CharacterLineUp>();

            MapObj.SetActive(true);
            CharacterObj.SetActive(true);
            
            foreach (var v in objs)
                    v.SetActive(false);

            opening = StartCoroutine(OpeningAnim());

            // this.UpdateAsObservable()
            //     .Where(_ => Input.GetKeyUp(KeyCode.Space) && !isSkip)
            //     .Subscribe(_ =>
            //     {
            //         isSkip = true;
            //         Ending();
            //     })
            //     .AddTo(this.gameObject);

            if (SoundController.Instance != null)
            {
                SoundController.Instance.SetBgmVolume(.25f);
            }
        }

        Coroutine opening;
        private IEnumerator OpeningAnim()
        {
            yield return null;
            characterLineUp.StartCoroutine(characterLineUp.LetsWalk());

            if(SoundController.Instance != null)
            {
                if (SoundController.Instance.IsPlaying())
                {
                    SoundController.Instance.PauseClip();
                }
            }
            SoundController.Instance.PlayClip(titleClip);
            yield return new WaitForSeconds(2.0f);
            
            SoundController.Instance.PlayClip(introClip);
            yield return new WaitForSeconds(introClip.length);

            characterAnimator.SetBool("IsSpeak", true);

            var length = narrationClip.Length;
            
            for(int i=0; i<length; i++)
            {
                SoundController.Instance.PlayClip(narrationClip[i]);
                yield return new WaitForSeconds(narrationClip[i].length);
            }

            yield return null;
            characterAnimator.SetBool("IsOutro", true);

            SoundController.Instance.PlayClip(effectClip);
            yield return new WaitForSeconds(.5f);

            Ending();
        }

        public void Ending()
        {
            if (opening != null)
            {
                StopCoroutine(opening);
            }

            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(2f);
                fadeImage.DORestartById("OUT");
                
                foreach (var v in objs)
                    v.SetActive(true);

                // 현재 씬을 가져옴
                Log.Info("[Intro]현재 Scene:" + SceneManager.GetActiveScene().name);


                if (SoundController.Instance != null)
                    SoundController.Instance.SetBgmVolume(.7f);

                this.gameObject.SetActive(false);
                                
                yield return null;
            }
        }

        public void PauseAnim()
        {
            //popoAnimator.GetCurrentAnimatorStateInfo(0).IsName("");
        }

    }

}