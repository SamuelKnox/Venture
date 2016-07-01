using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Devdog.QuestSystemPro.Dialogue
{
    public interface IPlayerInputNode
    {

        void SetPlayerInputStringAndMoveToNextNode(string enteredString);
        bool IsInputCorrect(string enteredString);

    }
}