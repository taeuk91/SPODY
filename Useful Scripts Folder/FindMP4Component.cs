using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Util
{
    public class FindMP4Component : MonoBehaviour
    {
        private VideoController videoController;

        private void OnEnable()
        {
            videoController = GameObject.FindObjectOfType<VideoController>();
        }

        public void OnExpercisingSongBtnClkEvent()
        {
            videoController.PlayExerciseSong();
        }

        public void OnExpercisingSongInLauncherBtnClkEvent()
        {
            videoController.PlayExerciseSongInLauncher();
        }
    }
}
