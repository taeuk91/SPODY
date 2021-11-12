using DG.Tweening;
using SPODY;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Util;

namespace SPODY.Team
{
    public enum WinTeam
    {
        RED, BLUE, DRAW
    }

    [Serializable]
    public class Score
    {
        public TextMeshPro text;
        private int score;

        public void Reset()
        {
            score = 0;
            text.text = score + "";
        }

        public void UpScore(int value)
        {
            score += value;
            text.text = score + "";
        }

        public int GetScore()
        {
            return score;
        }
    }

    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        public delegate void UpScoreEvent(Team team);
        public static event UpScoreEvent UpScoreEventHandler;

        [SerializeField] private TeamDictionarySetter<Score> teamScore;

        [SerializeField] int maxScore;

        [SerializeField] bool upScoreFx;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            UpScoreEventHandler = delegate (Team team) { };
        }

        private void OnEnable()
        {
            UpScoreEventHandler = delegate (Team team) { };
            teamScore.Init();
            ResetScore();
        }

        private void ResetScore()
        {
            teamScore.GetTeamValue(Team.RED).Reset();
            teamScore.GetTeamValue(Team.BLUE).Reset();
        }

        public void UpScore(Team team, int value)
        {
            teamScore.GetTeamValue(team).UpScore(value);
            UpScoreFx(team);

            if (team.Equals(Team.RED))
            {
                Winner.red_team_score += value;
            }
            else
            {
                Winner.blue_team_score += value;
            }
            
            UpScoreEventHandler(team);
        }

        private void UpScoreFx(Team team)
        {
            if (upScoreFx)
            {
                DOTween.Kill(GetInstanceID());
                teamScore.GetTeamValue(team).text.transform.DOScale(Vector3.one * 1.5f, .3f)
                    .SetEase(Ease.OutBack)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetId(GetInstanceID() + team.ToString());
            }
        }

        public bool Win(ref WinTeam winTeam)
        {
            if(teamScore.GetTeamValue(Team.RED).GetScore() >= maxScore)
            {
                winTeam = WinTeam.RED;
                return true;
            }

            if (teamScore.GetTeamValue(Team.BLUE).GetScore() >= maxScore)
            {
                winTeam = WinTeam.BLUE;
                return true;
            }

            if (!Timer.Instance.TimeOver()) return false;

            bool redWin = teamScore.GetTeamValue(Team.RED).GetScore() > teamScore.GetTeamValue(Team.BLUE).GetScore();
            bool blueWin = teamScore.GetTeamValue(Team.RED).GetScore() < teamScore.GetTeamValue(Team.BLUE).GetScore();
            bool draw = teamScore.GetTeamValue(Team.RED).GetScore() == teamScore.GetTeamValue(Team.BLUE).GetScore();

            if (redWin) winTeam = WinTeam.RED;
            else if(blueWin) winTeam = WinTeam.BLUE;
            else if(draw) winTeam = WinTeam.DRAW;

            return true;
        }
    }
}


