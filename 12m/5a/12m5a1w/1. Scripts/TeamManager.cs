using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Util.Usefull;
using static UnityEngine.Vector3;

namespace SPODY_12_5_1
{
    public class TeamManager : MonoBehaviour
    {
        [SerializeField] public List<GameObject> stages;
        [SerializeField] public int nowStage;
        public List<Transform> answerPos;
        public List<string> nowAnswer;
        [SerializeField] private Transform itemParent;
        public List<Transform> itemList;
        private bool isNextStage = false;
        public List<AudioClip> clips;
        private bool isClicked = false;
        public int answerNum = 3;
        public List<Transform> fireworksPos;

        public void Reset()
        {
            StartCoroutine(ResetEverything());

            IEnumerator ResetEverything()
            {
                if (GameManager.Instance.isGameStarted)
                {
                    //yield return PlayClip();
                    
                    //yield return DestroyItems();
                }
                
                //ShuffleStages();
                RelocateItems();

                ChagneStage();

                yield return LocateItems();

                nowStage++;
            }
        }

        private void RelocateItems()
        {
            Function.ShuffleList(ref answerPos);
            
            foreach (var pos in answerPos)
            {
                if (pos.childCount > 0)
                {
                    pos.GetChild(0).SetParent(itemParent);
                }
            }

            for (int i = 0; i < itemParent.childCount; i++)
            {
                itemParent.GetChild(i).localPosition = Vector3.zero;
                itemParent.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                itemParent.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

            }
        }

        private void ChagneStage()
        {
            foreach (var stage in stages)
                stage.gameObject.SetActive(false);

            if (nowStage >= stages.Count)
                nowStage = 0;

            if(nowStage > 0)
                stages[nowStage-1].SetActive(false);
                
            stages[nowStage].SetActive(true);

            ResetCharacter();
            stages[nowStage].transform.GetChild(0).gameObject.SetActive(true);
            stages[nowStage].transform.GetChild(1).gameObject.SetActive(false);
        }

        private void ResetCharacter()
        {
            GameObject kid1 = stages[nowStage].transform.GetChild(0).gameObject;
            GameObject kid2 = stages[nowStage].transform.GetChild(1).gameObject;
            
            kid1.SetActive(true);
            kid2.SetActive(false);

            for (int i = 0; i < kid2.transform.childCount; i++)
            {
                kid2.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (stages[nowStage].name.Contains("Inside"))
            {
                Transform icicleGroup = stages[nowStage].transform.GetChild(2);

                for (int i = 0; i < icicleGroup.childCount; i++)
                {
                    icicleGroup.GetChild(i).gameObject.SetActive(true);
                }
            }

        }


        private void ShuffleStages()
        {
            foreach (var stage in stages)
                stage.SetActive(false);

            Function.ShuffleList(ref stages);

            stages[0].SetActive(true);
            
            Function.ShuffleList(ref answerPos);
        }

        private List<Transform> tempList = new List<Transform>();
        private IEnumerator LocateItems()
        {
            ItemTouchOff();

            AddAnswerInStrList();
            
            List<Transform> items = CheckAnswer(itemList);

            MoveItemsToPos(items, answerPos);
            
            yield return new WaitForSeconds(0.3f);
            
            ItemTouchOn();
        }

        private void MoveItemsToPos(List<Transform> items, List<Transform> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (positions[i].childCount == 0)
                {
                    items[i].SetParent(positions[i]);
                    items[i].localPosition = Vector3.zero;
                }
            }
        }

        private void AddAnswerInStrList()
        {
            Transform standingKid = stages[nowStage].transform.GetChild(1);
            
            nowAnswer.Clear();

            for (int i = 0; i < standingKid.childCount; i++)
            {
                nowAnswer.Add(standingKid.GetChild(i).name);
            }        
        }

        private List<Transform> CheckAnswer(List<Transform> itemlst)
        {
            // AnserList에 이름이 있다면 3개 pos에 들어갈 아이템을 먼저 tempList에 넣는다.
            tempList.Clear();
            
            for (int i = 0; i < itemlst.Count; i++)
            {
                for (int j = 0; j < nowAnswer.Count; j++)
                {
                    if (nowAnswer[j] == itemlst[i].name)
                    {
                        tempList.Add(itemlst[i]);
                    }
                }
            }

            #region itemList안의 아이템과 tempList안의 아이템의 이름을 비교, 포함되는 것들만 넣는다. 
            // var result = itemlst
            //     .Where(x => tempList.Count(s => x.name.Contains(s.name)) != 0)
            //     .ToList();
            #endregion

            #region itemList안의 아이템과 tempList안의 아이템을, 포함되지 않는 것들만 넣는다.
            IEnumerable<Transform> differenceQuery =  
                itemlst.Except(tempList);

            List<Transform> tempList2 = differenceQuery.ToList();

            for (int i = 0; i < 2; i++)
            {
                tempList.Add(tempList2[i]);    
            }
            #endregion

            return tempList;
        }

        private void ItemTouchOn()
        {
            foreach (var item in itemList)
                item.tag = "Player";
        }
        
        private void ItemTouchOff()
        {
            foreach (var item in itemList)
                item.tag = "Untagged";
        }
    }
}