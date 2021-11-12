
using UnityEngine;

namespace Util
{
    public class Scale_Change : MonoBehaviour
    {
        [Header(":)")]        
        public Vector3 _start_scale;
        public Vector3 _desti_scale;


        public void SetScale()
        {
            transform.localScale = _desti_scale;
        }

        public void ReturnScale()
        {
            transform.localScale = _start_scale;
        }
    }
}