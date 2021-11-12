using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SPODY.Team
{
    #region ENUM : Team

    public enum Team
    {
        RED, BLUE
    }

    public class TeamComparer : IEqualityComparer<Team>
    {
        public bool Equals(Team x, Team y)
        {
            return ((int)x).Equals((int)y);
        }

        public int GetHashCode(Team obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    [Serializable]
    public class TeamDictionarySetter<T>
    {
        public T redValue;
        public T blueValue;

        public T GetTeamValue(Team team)
        {
            return teamDictionary[team];
        }

        public void SetTeamValue(Team team, T value)
        {
            teamDictionary[team] = value;
        }

        private Dictionary<Team, T> teamDictionary = new Dictionary<Team, T>(new TeamComparer());

        public void Init()
        {
            teamDictionary.Add(Team.RED, redValue);
            teamDictionary.Add(Team.BLUE, blueValue);
        }

        public void SetInit(Team team, T initValue)
        {
            teamDictionary.Add(team, initValue);
        }

        /// <summary>
        /// RED Value == Blue Value
        /// </summary>
        /// <param name="initValue"></param>
        public void SetInit(T initValue)
        {
            SetInit(Team.RED, initValue);
            SetInit(Team.BLUE, initValue);
        }
    }

    #endregion
}
