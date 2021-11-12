
using UnityEngine;


namespace Util.Usefull
{

    public class ControlUI : MonoBehaviour
    {
        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            Function.InitControlUI(ref m_RectTransform);
        }

        private void OnEnable() 
        {
            for (int i = 0; i < m_RectTransform.childCount; i++)
            {
                Destroy(m_RectTransform.GetChild(i).gameObject);
            }
        }


    }
}