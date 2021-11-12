using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{

    public class ScrollAnim : MonoBehaviour
    {
        [Header("UI")]
        public Image m_Image;
        public Sprite[] sprites;

        [SerializeField]
        private float wait;
        private int index;
        private int length;
        private List<Sprite> spriteList = new List<Sprite>();


        private void OnEnable()
        {
            INIT();

            StartCoroutine(FlowImages());
        }

        void INIT()
        {
            length = sprites.Length;
            for (int i = 0; i < length; i++)
            {
                spriteList.Add(sprites[i]);
            }

            index = 0;
        }


        public IEnumerator FlowImages()
        {
            yield return null;

            while(this.enabled)
            {
                m_Image.sprite = sprites[index];

                index++;
                if (index >= length)
                    index = 0;

                yield return new WaitForSeconds(wait);
            }
            
        }


    }
}