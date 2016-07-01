using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Other/Sync collection")]
    public class SyncCollection : MonoBehaviour
    {
        [InventoryRequired]
        public ItemCollectionBase toSyncFrom;

        [InventoryRequired]
        public UIWindow toSyncFromWindow;

        [InventoryRequired]
        public UIWindow toSyncToWindow;

        [InventoryRequired]
        public RectTransform toSyncToContainer;


//        private Transform _fromParent;
//        private int _fromChildIndex;

        protected virtual void Awake()
        {
            StartSyncing();

        }

        protected virtual void Start()
        {
        }


        protected void OnDestroy()
        {
            StopSyncing();
        }
        
        public void StartSyncing()
        {
            toSyncFromWindow.OnShow += CopyToOriginal;
            toSyncToWindow.OnShow += CopyToSynced;
            toSyncToWindow.OnHide += CopyToOriginal;
            toSyncFrom.OnSetItem += RebuildToLayout;
        }

        private void CopyToOriginal()
        {
            foreach (var item in toSyncFrom.items)
            {
                item.transform.SetParent(toSyncFrom.container);
                InventoryUtility.ResetTransform(item.transform);
            }
        }

        private void CopyToSynced()
        {
            foreach (var item in toSyncFrom.items)
            {
                item.transform.SetParent(toSyncToContainer);
                InventoryUtility.ResetTransform(item.transform);
            }
        }

        public void StopSyncing()
        {
            toSyncFromWindow.OnShow -= CopyToOriginal;
            toSyncToWindow.OnShow -= CopyToSynced;
            toSyncToWindow.OnHide -= CopyToOriginal;
            toSyncFrom.OnSetItem -= RebuildToLayout;
        }

        private void RebuildToLayout(uint slot, InventoryItemBase item)
        {
            LayoutRebuilder.MarkLayoutForRebuild(toSyncToContainer);
        }
    }
}
