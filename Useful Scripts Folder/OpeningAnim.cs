using System;
using System.Collections;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using DG.Tweening;

namespace Util.Usefull
{
    public class OpeningAnim : MonoBehaviour
    {
        public GameObject MapObj;
        public GameObject CharacterObj;

        public Animator popoAnimator;
        public Animator didiAnimator;

        public GameObject[] objs;
        public DOTweenAnimation fadeImage;

        public float delay;


        private void OnEnable()
        {
            MapObj.SetActive(true);
            CharacterObj.SetActive(true);

            
             foreach (var v in objs)
                    v.SetActive(false);


            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ =>
                {
                    Ending();
                })
                .AddTo(this.gameObject);

            Observable.Timer(TimeSpan.FromSeconds(delay))
                .Subscribe(_=>{
                    Ending();
                }).AddTo(this.gameObject);
                
            if(SoundController.Instance != null)
                SoundController.Instance.SetBgmVolume(.3f);
        }

        public void StartAnim()
        {
            popoAnimator.SetTrigger("Action");
            didiAnimator.SetTrigger("Action");
        }

        public void Ending()
        {
             StartCoroutine(Delay());

            IEnumerator Delay()
            {
                fadeImage.DORestartById("IN");
                yield return new WaitForSeconds(2f);
                fadeImage.DORestartById("OUT");
                
                foreach (var v in objs)
                    v.SetActive(true);

                if(SoundController.Instance != null)
                    SoundController.Instance.SetBgmVolume(.7f);

                this.gameObject.SetActive(false);
                                
                yield return null;
            }
        }

        public void PauseAnim()
        {
            popoAnimator.GetCurrentAnimatorStateInfo(0).IsName("");
        }

        
    }
}