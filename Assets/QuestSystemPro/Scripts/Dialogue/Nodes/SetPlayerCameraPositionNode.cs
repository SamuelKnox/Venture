using System;
using UnityEngine;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    [Summary("Set the player's camera position. Useful when you want to create specific shots for your character.")]
    public class SetPlayerCameraPositionNode : SetCameraPositionNodeBase
    {
        private DialogueCamera _camera;
        public override DialogueCamera camera
        {
            get
            {
                if (_camera == null)
                {
                    if (QuestManager.instance != null && QuestManager.instance.currentPlayer != null)
                    {
                       _camera = QuestManager.instance.currentPlayer.dialogueCamera;
                    }

#if UNITY_EDITOR
                    if (_camera == null)
                    {
                        var p = UnityEngine.Object.FindObjectOfType<QuestSystemPlayer>();
                        if (p != null)
                        {
                            _camera = p.dialogueCamera;
                        }
                    }
#endif
                }

                return _camera;
            }
        }

        public override void OnExecute()
        {
            if (camera == null)
            {
                QuestLogger.LogWarning("The player's camera is not defined. Can't set position");
                Finish(true);
                return;
            }

            camera.SetCameraPosition(position);
            Finish(true);
        }
    }
}