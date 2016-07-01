using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public partial class CraftingDelegates
    {
        public delegate void CraftStart(CraftingProgressContainer.CraftInfo craftInfo);
        public delegate void CraftSuccess(CraftingProgressContainer.CraftInfo craftInfo);
        public delegate void CraftFailed(CraftingProgressContainer.CraftInfo craftInfo);
        public delegate void CraftProgress(CraftingProgressContainer.CraftInfo craftInfo, float progress);
        public delegate void CraftCanceled(CraftingProgressContainer.CraftInfo craftInfo, float progress);
    }
}