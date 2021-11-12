
using UniRx;
using UniRx.Triggers;

using UnityEngine;
using Sirenix.OdinInspector;


namespace Util
{
    [InfoBox("바탕화면 흐르기")]
    public class BackGroundFlow : MonoBehaviour
    {
        public float speed;

        [Title("Position")]
        public Vector3 _start_vector3;
        public Vector3 _destination_vector3;

        private Transform m_Transform;


        private Vector3 _offset;
        private void Awake()
        {
            m_Transform = GetComponent<Transform>();
            _offset = (_destination_vector3 - m_Transform.localPosition).normalized;       
        }

        private void Start() 
        {
            this.UpdateAsObservable()
                .Where(_ => Vector3.Distance(m_Transform.localPosition, _destination_vector3) <= 1f)
                .Subscribe(_ =>
                {
                    m_Transform.localPosition = _start_vector3;
                })
                .AddTo(this.gameObject);
        }

        private void Update()
        {
            m_Transform.Translate(_offset * speed * Time.deltaTime);
        }
    }
}