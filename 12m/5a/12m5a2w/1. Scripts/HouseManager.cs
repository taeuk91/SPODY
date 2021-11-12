using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SPODY_12_5_2
{
    // 스테이지가 바뀌면 세 집 중에 하나를 띄워주고, 구름을 작동시킨다.
    public class HouseManager : MonoBehaviour
    {
        [SerializeField] public List<GameObject> houseList;
        [SerializeField] private GameObject AnswerPanel;
        public int cnt = 0;
        public List<Transform> cloudList;
        [SerializeField] private List<Transform> cloudPosList;
        [SerializeField] private List<Vector3> cloudOriginPosList;
        [SerializeField] private List<Vector3> originHousePositions;
        [SerializeField] private List<Transform> housePositions;

        private void OnEnable()
        {
            cloudOriginPosList = new List<Vector3>(cloudPosList.Count);
            
            for (int i = 0; i < cloudList.Count; i++)
            {
                cloudOriginPosList.Add(cloudList[i].position);
            }

            int houseCnt = 0;
            foreach (var house in houseList)
            {
                originHousePositions.Add(houseList[houseCnt].transform.position);
                houseCnt++;
            }
            
            foreach (var cloud in cloudList)
                cloud.gameObject.SetActive(false);
        }

        public IEnumerator ChangeHouse()
        {
            TurnCloudOff();

            for (int i = 0; i < houseList.Count; i++)
                houseList[i].transform.position = originHousePositions[i];
            
            foreach (var house in houseList)
                house.SetActive(false);

            if (cnt != 0)
                houseList[cnt - 1].SetActive(false);

            if (cnt >= houseList.Count)
                cnt = 0;

            // Todo : 터치를 했을 때, 현재 켜져있는 집을 복사해서 그 부모를 설정해서 이동하는 것 처럼 보여준다.  
            GameManager.Instance.nowHouse = houseList[cnt].transform;
            // GameManager.Instance.nowHouse.parent = houseList[cnt].transform.parent;
            houseList[cnt].SetActive(true);
            AnswerPanel.transform.SetParent(houseList[cnt].transform);
            houseList[cnt].transform.DOMove(housePositions[0].position, 1)
                .SetEase(Ease.Linear);
            cnt++;

            yield return null;
        }   

        public void TurnCloudsOn()
        {
            foreach (var cloud in cloudList)
                cloud.gameObject.SetActive(true);

            ScaleUpSequence();
        }

        private void ScaleUpSequence()
        {
            Sequence scaleUp = DOTween.Sequence();
            
            for (int i = 0; i < cloudList.Count; i++)
            {
                scaleUp.Append(cloudList[i].GetComponent<SpriteRenderer>().DOFade(1, .3f).From(0))
                    .Join(cloudList[i].DOScale(Vector3.one * 1.5f, 0.3f).From(Vector3.zero))
                    .Join(cloudList[i].DOMove(cloudPosList[i].position, 0.3f));
            }
        }

        public void TurnCloudOff()
        {
            // Sequence mySequence = DOTween.Sequence();
            //
            // for (int i = 0; i < cloudList.Count; i++)
            // {
            //
            //     mySequence.Append(cloudList[i].GetComponent<SpriteRenderer>().DOFade(0, .3f))
            //          .Join(cloudList[i].DOScale(Vector3.zero, 0.3f))
            //          .Join(cloudList[i].DOMove(cloudOriginPosList[i], 0.3f));
            //
            // }

            // mySequence.OnComplete(() =>
            // {
            foreach (var cloud in cloudList)
                cloud.gameObject.SetActive(false);
            // });
        }
            


    }
}