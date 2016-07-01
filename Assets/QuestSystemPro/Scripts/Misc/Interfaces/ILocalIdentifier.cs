using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Devdog.QuestSystemPro
{
    public interface ILocalIdentifier : IEquatable<ILocalIdentifier>
    {
        /// <summary>
        /// The identity of this 'player'. 
        /// </summary>
        NetworkIdentity identity { get; }

//        QuestsContainer quests { get; }
    }
}
