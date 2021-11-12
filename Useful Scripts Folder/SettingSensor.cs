using System.IO;
using UnityEngine;
using System.Linq;


namespace Util.Usefull
{
    public class SettingSensor : MonoBehaviour
    {
        // 현재 카메라가 무엇인지 확인
        // Setting Model에서 읽어 그냥.
        public SceneChange m_SceneChange;


        private void Start()
        {
            GetRead();
        }

        private void GetRead()
        {
            string path = "Diary.json";
            var origin = File.ReadLines(path);
            string SensorName = string.Join(System.Environment.NewLine, origin);

            switch(SensorName.First())
            {
                case 'K':
                    PlayerPrefs.SetString("SENSOR", "KINECT");
                    // print(SensorName);
                    break;
                case 'A':
                    PlayerPrefs.SetString("SENSOR", "ASTRA");
                    print(SensorName);
                    break;
                default:
                    //print("ERROR");
                    break;
            }
        }

    }
}