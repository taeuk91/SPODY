using UnityEngine;

namespace Util
{
    [System.Serializable]
    public struct Dialogue
    {
        public string name;

        [TextArea(3, 8)]
        public string[] sentences;
    }
}
