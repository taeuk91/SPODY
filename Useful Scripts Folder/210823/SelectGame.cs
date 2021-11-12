using Sirenix.OdinInspector;
using SPODY.Team;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util.Usefull.Game
{
    public class SelectGame : MonoBehaviour
    {
        private bool onStart = false;
        public bool OnStart
        {
            get
            {
                return onStart;
            }

            set
            {
                onStart = value;
            }
        }

        [SerializeField] private Team team;

        [Title("Spawn Settings")]

        [SerializeField] private List<SelectCharacter> characterPrefab;

        [Space]

        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private int maxAnswerSize;

        [Space]

        [SerializeField] private Transform spawnStorage;

        [Space]

        [SerializeField] private float resetDelay;

        private List<SelectCharacter> spawnPool= new List<SelectCharacter>();

        public void OnEnable()
        {
            StartCoroutine(CoCheckAnswers());
        }

        private IEnumerator CoCheckAnswers()
        {
            while (true)
            {
                while (!onStart)
                {
                    yield return null;
                }

                if (DisableAnswer())
                {
                    ResetCharacter();

                    yield return new WaitForSeconds(resetDelay);

                    Spawn();
                }

                yield return null;
            }
        }

        private bool DisableAnswer()
        {
            bool returnValue = true;

            foreach(SelectCharacter character in spawnPool)
            {
                if(character.isAnswer && character.gameObject.activeSelf)
                {
                    returnValue = false;
                }
            }

            return returnValue;
        }

        [Button]
        public void StartGame()
        {
            onStart = true;
        }

        public void Spawn()
        {
            ResetCharacter();

            int answerSpawn = 0;

            Function.ShuffleList(ref spawnPoints);

            foreach(Transform point in spawnPoints)
            {
                bool answer = answerSpawn++ < maxAnswerSize;

                SpawnCharacter(answer, point);
            }
        }

        private void ResetCharacter()
        {
            foreach(SelectCharacter character in spawnPool)
            {
                character.Active(false);
            }
        }

        private void SpawnCharacter(bool isAnswer, Transform spawnPoint)
        {
            SelectCharacter spawnCharacter = null;

            foreach (SelectCharacter character in spawnPool)
            {
                if (!character.gameObject.activeSelf)
                {
                    spawnCharacter = character;
                    break;
                }
            }

            if(spawnCharacter == null)
            {
                spawnCharacter = Instantiate(GetCharacter(), spawnStorage);
                spawnPool.Add(spawnCharacter);
            }

            spawnCharacter.gameObject.SetActive(true);
            spawnCharacter.Active(true);

            spawnCharacter.isAnswer = isAnswer;
            spawnCharacter.Controller = this;

            spawnCharacter.transform.localPosition = spawnPoint.localPosition;
        }

        private int characterIndex = 0;

        private SelectCharacter GetCharacter()
        {
            SelectCharacter returnCharacter = characterPrefab[characterIndex++];

            if(characterIndex >= characterPrefab.Count)
            {
                characterIndex = 0;
                Function.ShuffleList(ref characterPrefab);
            }

            return returnCharacter;
        }

        public void UpScore()
        {
            ScoreManager.Instance.UpScore(team, 1);
        }
    }
}


