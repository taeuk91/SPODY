
using UniRx;
using UniRx.Triggers;

using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public class Mute : MonoBehaviour
    {
        public Slider _slider;
        private float value;

        private void Awake()
        {
            this.UpdateAsObservable()
                .Where(_ => value != _slider.value)
                .Subscribe(_ =>
                {
                    value = _slider.value;

                    AudioListener.volume = value;

                    //for (int i = 0; i < _audios.Length; i++)
                    //{
                    //    _audios[i].volume = value;
                    //}
                })
                .AddTo(this.gameObject);
        }

    }
}