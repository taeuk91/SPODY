
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Util.Usefull;

namespace Util
{
    public class HeartShape : MonoBehaviour
    {
        [Header("HeartShape")]
        public Transform m_Transform;

        [Header("heart Sort")]
        public Image[] hearts;
        public AudioClip effectClip;

        private int index;
        private int length;
        
        private void OnEnable()
        {
            Init();
        }

        void Init()
        {
            foreach (var v in hearts)
                v.color = new Color(1f, 1f, 1f, 0f);

            index = 0;
            length = hearts.Length;
        }

        public void AddHeart()
        {
            SoundController.Instance.PlayClip(effectClip);
            hearts[index].DOColor(Color.white, .5f)
                .SetEase(Ease.InOutFlash);

            index++;
            if (index >= length)
                index = 0;
        }

        public void ResetHeart()
        {
            Invoke(nameof(Init), 1f);
        }

    }
}