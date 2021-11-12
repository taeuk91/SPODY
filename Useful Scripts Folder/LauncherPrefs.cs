using UnityEngine;

namespace Util
{
    public class LauncherPrefs : MonoBehaviour
    {
        public Launcher_Animation_Tweening[] LauncherSort;
        public GameObject _back;

        

        private void OnEnable()
        {
            var v = PlayerPrefs.GetInt("Launcher");
            
            switch (v)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    _back.SetActive(false);
                    LauncherSort[v].gameObject.SetActive(true);
                    LauncherSort[v].OnClickIn();
                    return;
                default:
                    return;
            }
        }


        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("Launcher", -1);
        }


    }
}