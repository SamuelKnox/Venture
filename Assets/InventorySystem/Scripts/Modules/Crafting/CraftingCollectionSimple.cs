using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEngine;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public partial class CraftingCollectionSimple : ItemCollectionBase
    {
        [SerializeField]
        private uint _initialCollectionSize;
        public override uint initialCollectionSize
        {
            get { return _initialCollectionSize; }
        }


        public override void Awake()
        {
            base.Awake();
        }

        public override bool OverrideUseMethod(InventoryItemBase item)
        {
//            InventoryManager.AddItemAndRemove(item);
            return true;
        }
    }
}
