using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.QuestSystemPro.ThirdParty.FullSerializer;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    [System.Serializable]
    public class Asset<T> : IAsset where T : UnityEngine.Object
    {
        public T val;
        public UnityEngine.Object objectVal
        {
            get { return val; }
            set { val = (T) value; }
        }

        public Asset()
        {
            
        }

        public Asset(T val)
        {
            this.val = val;
        }
    }

    public interface IAsset
    {
        UnityEngine.Object objectVal { get; set; }
    }
}
