using System;
using UnityEngine;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    [Summary("Set a camera's position. Useful when you want to create specific shots from your character.")]
    public class SetCameraPositionNode : SetCameraPositionNodeBase
    {
        [ShowInNode]
        public string cameraName = "";

        public override DialogueCamera camera
        {
            get
            {
                return DialogueCamera.GetCamera(cameraName);
            }
        }

        public override void OnExecute()
        {
            if (camera == null)
            {
                QuestLogger.LogWarning("The dialogue owner's camera is not defined. Can't set position");
                Finish(true);
                return;
            }

            camera.SetCameraPosition(position);
            Finish(true);
        }
    }
}