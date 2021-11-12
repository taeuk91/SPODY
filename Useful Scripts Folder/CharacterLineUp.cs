using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Util.Usefull
{
    public class CharacterLineUp : MonoBehaviour
    {
        enum GameMode
        {
            SingleMode,
            ChallengeMode
        }
        [SerializeField] private GameMode gameMode = GameMode.SingleMode;
        // Start is called before the first frame update

        public GameObject kidsG;
        public GameObject kidsG_Challenge;
        public GameObject challengeBg;

        [Title("Kids Prefabs")] [SerializeField] private List<GameObject> leftKids;
        [Title("Kids Prefabs")] [SerializeField] private List<GameObject> rightKids;

        public float K_Speed = 1f;

        [Title("Number of Kids")]
        public int kidCountL;
        public int kidCountR;
        public float spacing;

        bool isLeft = true;
        public IEnumerator LetsWalk()
        {
            switch (gameMode)
            {
                case GameMode.SingleMode:
                    challengeBg.SetActive(false);
                    // 3명의 아이를 생성해준다.
                    for (int i = 0; i < kidCountL; i++)
                    {
                        StartCoroutine(CoMoveCharacter(leftKids[i], i, isLeft));

                        yield return new WaitForSeconds(0.034f/kidCountL);
                    }
                    
                    break;
                case GameMode.ChallengeMode:
                    challengeBg.SetActive(true);
                    for (int i = 0; i < kidCountL; i++)
                    {
                        StartCoroutine(CoMoveCharacter(leftKids[i], i, isLeft));
                        yield return new WaitForSeconds(0.1256f / kidCountL);
                    }

                    for (int i = 0; i < kidCountR; i++)
                    {
                        StartCoroutine(CoMoveCharacter(rightKids[i], i, !isLeft));
                        yield return new WaitForSeconds(0.1256f / kidCountL);
                    }
                    
                    break;
            }
        }

        IEnumerator CoMoveCharacter(GameObject kid, int index, bool isLeft)
        {
            Vector3 originPos = kid.transform.localPosition;
            float currenttime = 0;

            float distance = 0;

            switch (gameMode)
            {
                case GameMode.SingleMode:
                    distance = 13;
                    break;
                case GameMode.ChallengeMode:
                    distance = 6.5f;
                    break;
            }

            while (true)
            {
                currenttime += Time.deltaTime;
                if (isLeft)
                {
                    kid.transform.localPosition = Vector3.Lerp(originPos, originPos + new Vector3(distance + (index / rightKids.Count) - (1.6f * index), 0, 0), currenttime / K_Speed);
                    
                }
                else
                {
                    kid.transform.localPosition = Vector3.Lerp(originPos, originPos + new Vector3(-distance - (index / rightKids.Count) + (1.6f * index), 0, 0), currenttime / K_Speed);
                }
                yield return null;
            }
        }
    }

}
