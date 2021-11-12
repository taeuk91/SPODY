using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Util.Usefull;
using DG.Tweening;

namespace SPODY
{
    public abstract class MatchGamePattern<T> : MonoBehaviour where T : MatchGamePattern<T>
    {
        [Title("Match Game Pattern")]
        [InfoBox("Match Game Pattern Variables")]
        
        #region # Variables
        [SerializeField] protected List<GameObject> stages;
        public List<Transform> answerPos;
        public string nowAnswer = "";
        private string nowStage = "";
        [SerializeField] private Transform itemParent;
        public List<Transform> itemList;
        [SerializeField] protected  List<Transform> positions;
        private bool isNextStage = false;
        public List<AudioClip> clips;
        private bool isClicked = false;
        [SerializeField] private List<Transform> tempAnswerList = new List<Transform>();
        public int answerNum = 3;
        #endregion
        
        #region # Singleton Codes..

        protected static T instance = null;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region # Life Functions
        protected void Awake()
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }

            InitAwake();
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            InitOnEnable();
        }
        #endregion

        
        
        #region # Abstract Functions
        protected abstract void InitAwake();
        protected abstract void InitOnEnable();
        
        #endregion

        #region # Virtual Functions
        protected virtual IEnumerator DestroyItems()
        {
            foreach (var item in tempAnswerList)
            {
                Destroy(item.gameObject);
            }

            isNextStage = false;

            yield return null;
        }
        
        public void Reset()
        {
            StartCoroutine(ResetEverything());

            IEnumerator ResetEverything()
            {
                yield return PlayClip();
                        
                yield return DestroyItems();

                ShuffleStages();

                yield return RocateItems(itemList, positions);
            }
        }
        
        protected IEnumerator PlayClip()
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
        
        protected virtual void ShuffleStages()
        {
            foreach (var stage in stages)
                stage.SetActive(false);
            
            Function.ShuffleList(ref stages);
            while (nowAnswer == stages[0].name)
                Function.ShuffleList(ref stages);
                
            stages[0].SetActive(true);
            nowAnswer = stages[0].name;
                
            Function.ShuffleList(ref answerPos);
                
            if(tempAnswerList.Count > 0)
                tempAnswerList.Clear();
        }
        
        protected  IEnumerator RocateItems(List<Transform> itemlst, List<Transform> positions)
        {
            ItemTouchOff();

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
                        //item.GetComponent<TouchObject>().teamManager = this;
                        item.tag = "Player";
                            
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
            
                if (!nowAnswer.Contains(food.name))
                {
                    Transform item = Instantiate(food);
                    //item.GetComponent<TouchObject>().teamManager = this;
                    item.tag = "Player";
            
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
        #endregion
    }

}
