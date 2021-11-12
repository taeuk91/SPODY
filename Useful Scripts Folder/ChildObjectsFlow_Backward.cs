using System.Collections;

using UnityEngine;
using UnityEngine.Events;


namespace Util
{
    public class ChildObjectsFlow_Backward : MonoBehaviour
    {
        [Header("# Higher the number, Slower the speed")]
        public float speed;

        // child list index
        private GameObject[] _child_objects;
        private int index;

        Transform m_Transform;

        public UnityEvent unityEvent;

        private void Awake()
        {
            m_Transform = GetComponent<Transform>();
            GetChild();
        }

        private void OnEnable()
        {
            StartCoroutine(Flow());
        }

        private void OnDisable() {
            StopAllCoroutines();

            for (int i = 0; i < m_Transform.childCount; i++)
            {
                _child_objects[i].SetActive(i == m_Transform.childCount-1);
            }
        }

        // public bool IsFlow() // False면 끝난 거.
        // {
        //     return IndexCondition();
        // }

        #region ★Flow
        private IEnumerator Flow()
        {
            index = 0;

            for (int i = 0; i < m_Transform.childCount; i++)
            {
                _child_objects[i].SetActive(false);
            }
            
            int amount = 0;

            while (this)
            {
                if (index >= _child_objects.Length-1)
                {
                    amount = -1;
                }
                else if(index <= 0)
                {
                    amount = 1;
                }


                _child_objects[index].SetActive(true);
                yield return new WaitForSeconds(speed);

                _child_objects[index].SetActive(false);

                index += amount;
            }
            unityEvent.Invoke();
            
            _child_objects[index-1].SetActive(true);
            
        }


        // private bool IndexCondition()
        // {
        //     if (isLoop) return true;

        //     if (index >= _child_objects.Length) return false;
        //     else return true;
        // }

        private void GetChild()
        {
            _child_objects = new GameObject[m_Transform.childCount];

            for(int i=0; i<m_Transform.childCount; i++)
            {
                _child_objects[i] = m_Transform.GetChild(i).gameObject;
            }
        }

        #endregion

    }
}