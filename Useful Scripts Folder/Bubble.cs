using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util.Usefull
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject popFx;

        [Space]

        [SerializeField] private float createDuration;
        [SerializeField] private float popDuration;

        private void OnEnable()
        {
            model.SetActive(true);
            popFx.SetActive(false);

            model.transform.localScale = Vector3.zero;
            model.transform.DOScale(Vector3.one, createDuration)
                .SetEase(Ease.OutBack);
        }

        [Button]
        public void Pop()
        {
            model.transform.DOScale(Vector3.zero, popDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    popFx.SetActive(true);
                    model.SetActive(false);
                });
        }
    }
}

