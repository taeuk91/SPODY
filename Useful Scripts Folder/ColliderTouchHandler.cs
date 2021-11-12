using UnityEngine;
using UnityEngine.Events;

namespace Util
{
    public class ColliderTouchHandler : CharacterBehavior
    {
        public UnityEvent touchEvent;

        public override bool IsTouch { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override bool IsAnim { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override Vector3 hitPoint { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override void Active(bool active)
        {
            throw new System.NotImplementedException();
        }

        public override void Move(bool isPause)
        {
            throw new System.NotImplementedException();
        }

        public override void PlayAnim(string trigger)
        {
            throw new System.NotImplementedException();
        }

        public override void PlayClip()
        {
            throw new System.NotImplementedException();
        }

        public override void Touch()
        {
            touchEvent.Invoke();
        }
    }
}