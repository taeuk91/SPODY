using System.Collections;

using UnityEngine;

using DG.Tweening;

namespace Util
{
    public class ChildrenParticle : MonoBehaviour
    {
        private GameObject[] _particle;
        private Vector3[] _vector3;
#pragma warning disable IDE0052 // 읽지 않은 private 멤버 제거
        private RectTransform _rect;
#pragma warning restore IDE0052 // 읽지 않은 private 멤버 제거

        private void Awake()
        {
            _particle = new GameObject[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                _particle[i] = transform.GetChild(i).gameObject;
            }
            _rect = _particle[0].GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (_particle == null)
                return;

            StartCoroutine(ParticleEffect());
        }

        IEnumerator ParticleEffect()
        {
            yield return null;

            _vector3 = new Vector3[8]
            {
                new Vector3(0f,100f),
                new Vector3(70.7f, 70.7f),
                new Vector3(100f,0f),
                new Vector3(70.7f, -70.7f),

                new Vector3(0f,-100f),
                new Vector3(-70.7f, -70.7f),
                new Vector3(-100f, 0f),
                new Vector3(-70.7f, 70.7f),
            };

            for (int i = 0; i < 8; i++)
            {
                Vector3 pos = this.transform.position + _vector3[i];
                _particle[i].transform.DOMove(pos, 1.7f, true)
                    .SetEase(Ease.OutFlash);

                _particle[i].transform.DOScale(0, 1.5f)
                    .SetEase(Ease.InBack);

                _particle[i].transform.DOShakeRotation(1.5f, 45f, 5, 45f, true);


            }

            yield return new WaitForSeconds(1.5f);

            for (int i = 0; i < transform.childCount; i++)
            {
                //_particle[i].GetComponent<RectTransform>() = _rect;
            }
            this.gameObject.SetActive(false);
        }


    }


}