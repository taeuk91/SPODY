using UnityEngine;


namespace Util
{
    public abstract class CharacterBehavior : MonoBehaviour
    {        
        public abstract bool IsTouch { get; set; }
        public abstract bool IsAnim { get; set; }
        public abstract Vector3 hitPoint { get; set; }

        public abstract void Move(bool isPause);
        public abstract void Active(bool active);
        public abstract void PlayAnim(string trigger);

        public abstract void Touch();
        public abstract void PlayClip();
    }
}