using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SPODY.Team;

namespace Util.Usefull
{
    public class Effects : MonoBehaviour
    {
        public static IEnumerator CoFireworks(GameObject effect, int numOfEffs, float minVal = 0.3f, float maxVal = 0.5f)
        {
            for (int i = 0; i < numOfEffs; i++)
            {
                SpawnFirework(effect);

                float randSec = Random.Range(minVal, maxVal);
                yield return new WaitForSeconds(randSec);
            }
        }

        public static IEnumerator CoFireworks(Team team, GameObject effect, int numOfEffs, float minVal = 0.3f, float maxVal = 0.5f)
        {
            int minX = team.Equals(Team.RED) ? 0 : Screen.width / 2;
            int minY = team.Equals(Team.RED) ? 0 : Screen.height / 2;

            int maxX = team.Equals(Team.RED) ? Screen.width / 2 : Screen.width;
            int maxY = team.Equals(Team.RED) ? Screen.height / 2 : Screen.height;

            for (int i = 0; i < numOfEffs; i++)
            {
                SpawnFirework(effect, minX, minY, maxX, maxY);

                float randSec = Random.Range(minVal, maxVal);
                yield return new WaitForSeconds(randSec);
            }
        }

        private static void SpawnFirework(GameObject effect)
        {
            SpawnFirework(effect, 0, 0, Screen.width, Screen.height);
        }
        private static void SpawnFirework(GameObject effect, int minX, int minY, int maxX, int maxY)
        {
            int randX = Random.Range(minX, maxX);
            int randY = Random.Range(minY, maxY);

            Vector3 randPosInResolution = new Vector3(randX, randY, 0);
            Vector3 explosionPos = Camera.main.ScreenToWorldPoint(randPosInResolution);

            GameObject fireworks = Instantiate(effect);
            fireworks.transform.position = new Vector3(explosionPos.x, explosionPos.y, 0);
        }

        public static IEnumerator FadeInOut(DOTweenAnimation fadeImg)
        {
            fadeImg.DORestartById("IN");
            yield return new WaitForSeconds(2f);;
            fadeImg.DORestartById("OUT");
        }
    }
}