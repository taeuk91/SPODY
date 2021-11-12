using DG.Tweening;
using SPODY.Team;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.Usefull;

namespace SPODY.Team
{

    public abstract class TeamManager<T> : MonoBehaviour where T : TeamManager<T>
    {
        #region Singleton Codes..

        protected static T instance = null;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }
        }

        #endregion

        [SerializeField] protected GameObject endingFx;
        [SerializeField] protected TeamDictionarySetter<Animator> endResultSetter;

        protected TeamDictionarySetter<Coroutine> teamCoroutineSetter = new TeamDictionarySetter<Coroutine>();

        public abstract void AnswerEvent(Team team, bool value);

        public abstract void InitSetting();

        public abstract void StartGame();

        protected abstract IEnumerator INGAME(Team team);

        protected abstract bool IsGameEnd();


        public abstract void EndGame();

        public abstract void Ending();
    }
}

