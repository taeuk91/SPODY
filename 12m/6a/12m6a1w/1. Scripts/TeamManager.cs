using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Util.Usefull;
using static UnityEngine.Vector3;

namespace SPODY_12_6_1
{
    public class TryCoroutine
    {
        public void Start()
        {
            foreach (int i in Power(2, 8))
            {
                Debug.Log(i);
            }
        }
        
        public static System.Collections.Generic.IEnumerable<int> Power(int number, int exponent)
        {
            int result = 1;

            for (int i = 0; i < exponent; i++)
            {
                result = result * number;
                
                yield return result;
            }
        }
    }
    
    
    public class TeamManager : MonoBehaviour
    {
        [SerializeField] public List<GameObject> stages;
        public List<Transform> answerPos;
        public string nowAnswer = "";
        public Animator nowAnim;
        public Transform posToMove;
        [SerializeField] private Transform itemParent;
        public List<Transform> itemList;
        [SerializeField] private List<Transform> positions;
        private bool isNextStage = false;
        public List<AudioClip> clips;
        private bool isClicked = false;
        [SerializeField] private List<Transform> tempAnswerList = new List<Transform>();
        public int answerNum = 3;

        IEnumerator DestroyItems()
        {
            foreach (var item in tempAnswerList)
            {
                Destroy(item.gameObject);
            }

            isNextStage = false;

            yield return null;
        }

        private string nowStage = "";
        public void Reset()
        {
            StartCoroutine(ResetEverything());

            IEnumerator ResetEverything()
            {
                if (GameManager.Instance.isGameStarted)
                {
                    yield return PlayClip();
                    
                    yield return DestroyItems();
                }
                
                ShuffleStages();

                yield return RocateItems(itemList, positions);
            }
        }

        private IEnumerator PlayClip()
        {
            Function.ShuffleList(ref clips);

            foreach (var clip in clips)
            {
                if (clip.name.Contains(nowAnswer))
                {
                    yield return new WaitForSeconds(0.5f);
                    
                    SoundController.Instance.PlayClip(clip);

                    yield return new WaitForSeconds(clip.length);
                    break;
                }
            }
        }


        private void ShuffleStages()
        {
            foreach (var stage in stages)
                stage.SetActive(false);

            Function.ShuffleList(ref stages);
            
            while (nowAnswer == stages[0].name)
                Function.ShuffleList(ref stages);
            
            stages[0].SetActive(true);
            nowAnswer = stages[0].name;
            nowAnim = stages[0].GetComponentInChildren<Animator>();
            posToMove = stages[0].transform.GetChild(1);
            
            Function.ShuffleList(ref answerPos);
            
            if(tempAnswerList.Count > 0)
                tempAnswerList.Clear();
        }

        private IEnumerator RocateItems(List<Transform> itemlst, List<Transform> positions)
        {
            ItemTouchOff();
            
            Function.ShuffleList(ref itemlst);

            CreateItem(itemlst, tempAnswerList);
            
            for (int i = 0; i < positions.Count; i++)
            {
                tempAnswerList[i].SetParent(positions[i]);

                if (i != positions.Count - 1)
                {
                    tempAnswerList[i].transform.DOMove(positions[i].position + new Vector3(0, 0.4f), 0.5f);
                }
                else
                    tempAnswerList[i].transform.DOMove(positions[i].position + new Vector3(0, 0.4f), 0.5f)
                        .OnComplete(() =>
                        {
                            for (int j = 0; j < positions.Count; j++)
                            {
                                ItemTouchOn();
                            }
                        });

                yield return new WaitForSeconds(0.3f);
            }
        }
        
        private void CreateItem(List<Transform> from, List<Transform> tempList)
        {
            int i = 0;
            foreach (var food in from)
            {
                if (nowAnswer.Contains(food.name))
                {
                    for (int j = 0; j < answerNum; j++)
                    {
                        Transform item = Instantiate(food);
                        item.name = food.name;
                        item.GetComponent<TouchObject>().teamManager = this;
                        item.tag = "Player";

                        item.DORotate(new Vector3(0,0,10), 0.5f)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetId(GetInstanceID());
                        
                        tempList.Add(item);
                        i++;
                    }
                    break;
                }
            }
            
            foreach (var food in from)
            {
                if (i >= positions.Count)
                    break;

                if (nowAnswer[1] != food.name[1])
                {
                    Transform item = Instantiate(food);
                    item.GetComponent<TouchObject>().teamManager = this;
                    item.tag = "Player";
                    
                    item.DORotate(new Vector3(0,0,10), 0.5f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetId(GetInstanceID());

                    tempList.Add(item);
                    i++;
                }
            }

            Function.ShuffleList(ref tempList);
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