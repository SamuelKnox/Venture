using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.QuestSystemPro
{
    public class LocalIdentifier : ILocalIdentifier
    {
        public NetworkIdentity identity { get; private set; }
//        public QuestsContainer quests { get; private set; }

        public LocalIdentifier(NetworkIdentity identity)
        {
            this.identity = identity;
        }


        #region Equality comparers

        public static bool operator ==(LocalIdentifier a, LocalIdentifier b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LocalIdentifier a, LocalIdentifier b)
        {
            return !(a == b);
        }

        public bool Equals(ILocalIdentifier other)
        {
            return identity == other.identity;
        }

        protected bool Equals(LocalIdentifier other)
        {
            return Equals(identity, other.identity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LocalIdentifier)obj);
        }

        public override int GetHashCode()
        {
            return (identity != null ? identity.GetHashCode() : 0);
        }

        #endregion
    }
}
