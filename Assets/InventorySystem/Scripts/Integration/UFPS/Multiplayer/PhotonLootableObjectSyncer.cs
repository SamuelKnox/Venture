#if UFPS_MULTIPLAYER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [UnityEngine.RequireComponent(typeof(LootableObject))]
    [UnityEngine.RequireComponent(typeof(PhotonView))]
    [UnityEngine.AddComponentMenu("InventorySystem/Integration/UFPS/PhotonLootableObjectSyncer")]
    public partial class PhotonLootableObjectSyncer : Photon.MonoBehaviour
    {
        private LootableObject _lootable { get; set; }
        private ObjectTriggerer _triggerer { get; set; }

        private List<PhotonPlayer> _activeUsers = new List<PhotonPlayer>();

        protected virtual void Awake()
        {
            _lootable = GetComponent<LootableObject>();
            _lootable.OnLootedItem += LootableOnOnLootedItem;

            _triggerer = GetComponent<ObjectTriggerer>();
            _triggerer.OnTriggerUsed += (player) =>
            {
                if (PhotonNetwork.isMasterClient == false)
                {
                    _lootable.items = new InventoryItemBase[0]; // Clear the items, wait for the server to respond with the item list.

                    // Request the data from the server
                    photonView.RPC("RequestLootableItems", PhotonTargets.MasterClient);
                }

                photonView.RPC("OnTriggerUsedByOtherClient", PhotonTargets.Others);
            };

            _triggerer.OnTriggerUnUsed += (player) => { photonView.RPC("OnTriggerUnUsedByOtherClient", PhotonTargets.Others); };
        }

        [PunRPC]
        protected void OnTriggerUsedByOtherClient(PhotonMessageInfo info)
        {
            _triggerer.DoVisuals();

            if (PhotonNetwork.isMasterClient)
            {
                _activeUsers.Add(info.sender);
            }
        }

        [PunRPC]
        protected void OnTriggerUnUsedByOtherClient(PhotonMessageInfo info)
        {
            _triggerer.UndoVisuals();

            if (PhotonNetwork.isMasterClient)
            {
                _activeUsers.Remove(info.sender);
            }
        }

        [PunRPC]
        protected void SetLootableItems(string itemIDsString, PhotonMessageInfo info)
        {
            Assert.IsTrue(info.sender.ID == PhotonNetwork.masterClient.ID, "SetLootableItems didn't come from masterClient!");
            UnityEngine.Debug.Log("Server gave a list of items: " + itemIDsString);

            foreach (var item in _lootable.items)
            {
                Destroy(item.gameObject); // Destroy old items that were still here...
            }

            if (string.IsNullOrEmpty(itemIDsString))
            {
                _lootable.items = new InventoryItemBase[0];
                return;
            }

            var items = GetItemsFromNetworkingString(itemIDsString);

            // Set items
            _lootable.items = items; // Set the lootable items for this object.

            // Update loot window
            _lootable.lootUI.SetItems(_lootable.items, false);
            foreach (var cur in _lootable.currencies)
            {
                _lootable.lootUI.AddCurrency(cur.amount, cur._currencyID);
            }

            _lootable.lootUI.window.Show();
        }

        private void LootableOnOnLootedItem(InventoryItemBase item, uint itemId, uint slot, uint amount)
        {
            photonView.RPC("LootableObjectItemRemoved", PhotonTargets.OthersBuffered, (int)slot);
        }

        [PunRPC]
        private void LootableObjectItemRemoved(int slot)
        {
            UnityEngine.Debug.Log("Other client looted slot: " + slot, transform);
            if (slot < 0 || slot >= _lootable.items.Length)
            {
                UnityEngine.Debug.LogWarning("Item is out of range " + slot + " can't set item");
                return;
            }

            _lootable.items[slot] = null;
            if (_lootable.lootUI.items.Length == _lootable.items.Length)
            {
                _lootable.lootUI.SetItem((uint)slot, null, true);
            }
        }

        [PunRPC]
        private void RequestLootableItems(PhotonMessageInfo info)
        {
            string result = GetItemsAsNetworkString();
            photonView.RPC("SetLootableItems", info.sender, result);
        }

        private InventoryItemBase[] GetItemsFromNetworkingString(string itemIDsString)
        {
            string[] itemIDs = itemIDsString.Split(',');
            var items = new InventoryItemBase[itemIDs.Length];

            for (int i = 0; i < itemIDs.Length; i++)
            {
                if (itemIDs[i] == "-1")
                {
                    items[i] = null;
                    continue;
                }

                var x = itemIDs[i].Split(':');
                UnityEngine.Debug.Log(itemIDs[i]);
                
                var item = Instantiate<InventoryItemBase>(ItemManager.database.items[int.Parse(x[0])]);
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform);
                item.currentStackSize = uint.Parse(x[1]);
                items[i] = item;
            }

            return items;
        }

        private string GetItemsAsNetworkString()
        {
            // Master defines the items to be looted, and sends it to the clients.
            return string.Join(",", _lootable.items.Select(x => x == null ? ("-1:0") : (x.ID.ToString() + ":" + x.currentStackSize.ToString())).ToArray()); // Concat as a string, because photon is being bitchy about int arrays
        }
    }
}

#endif