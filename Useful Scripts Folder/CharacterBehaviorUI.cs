using UnityEngine;
using Util.Astra_;

namespace Util
{
    public abstract class CharacterBehaviorUI : MonoBehaviour, ITouchClick
    {
        public abstract bool IsTouch { get; set; }
        public abstract bool IsAnim { get; set; }
        public abstract Vector3 hitPoint { get; set; }

        public abstract void Move(bool isPause);
        public abstract void Active(bool active);
        public abstract void PlayAnim(string trigger);

        public abstract void OnBallClick(float bx, float by);
        public abstract void PlayClip();
    }
}