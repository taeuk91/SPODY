using UnityEngine;

using DG.Tweening;



namespace Util.Usefull
{
    public class RotateEffect_ : MonoBehaviour
    {
        [Header("IS LOOP MOTION?")]
        public bool isLoop = false;


        [Range(0f, 15f)]
        public float rotAngle;

        [Range(0f, 1f)]
        public float speed;

        public enum Direction
        {
            LEFT, RIGHT
        }

        public Direction _direction;

        private string id;

        private Transform m_Transform;

        //private bool stop = false;

        private void Awake()
        {
            m_Transform = GetComponent<Transform>();
            //stop = false;
        }

        private void OnEnable()
        {
            if (isLoop)
                RotateStart(-1, _direction);
        }

        private void OnDisable() 
        {
            if(this.gameObject.activeSelf)
                RotateBack();
        }

        /// <summary>
        /// #ROTATE START!!
        /// </summary>
        /// <param name="loopAmount">if you input -1, endless Loop</param>
        /// <param name="direction">Set Rotate Direction</param>
        public void RotateStart(int loopAmount, Direction direction)
        {
            Vector3 startAngle, targetAngle = Vector3.zero;

            if(direction.Equals(Direction.LEFT))
            {
                startAngle = new Vector3(0f, 0f, -rotAngle);
                targetAngle = new Vector3(0f, 0f, rotAngle);
            }
            else if(direction.Equals(Direction.RIGHT))
            {
                startAngle = new Vector3(0f, 0f, rotAngle);
                targetAngle = new Vector3(0f, 0f, -rotAngle);
            }
            else
            {
                return;
            }

            m_Transform.DOLocalRotate(startAngle, speed * .5f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        id = "Rot" + this.gameObject.name + Time.time;
                        m_Transform.DOLocalRotate(targetAngle, speed)
                            .SetEase(Ease.Linear)
                            .SetLoops(loopAmount, LoopType.Yoyo)
                            .SetId(id)
                            .OnComplete(() =>
                            {
                                RotateBack();
                            });
                    });

        }

        public void RotateBack()
        {
            DOTween.Pause(id);
            m_Transform.DOLocalRotate(Vector3.zero, speed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    this.enabled = false;
                });
        }

    }
}