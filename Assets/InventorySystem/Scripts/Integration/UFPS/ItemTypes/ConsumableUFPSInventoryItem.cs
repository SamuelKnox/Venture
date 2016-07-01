#if UFPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [RequireComponent(typeof(ObjectTriggererItemUFPS))]
    public partial class ConsumableUFPSInventoryItem : UFPSInventoryItemBase
    {
        public InventoryAudioClip pickupSound;

        public float restoreHealthAmount = 10f;


        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
#if UFPS_MULTIPLAYER

            if (vp_MPPickupManager.Instance != null)
            {
                //var dropObj = base.Drop(location, rotation);
                var dropPos = GetDropPosition(location, rotation);
                NotifyItemDropped(null);

                //gameObject.SetActive(false);
                vp_MPPickupManager.Instance.photonView.RPC("InventoryDroppedObject", PhotonTargets.AllBuffered, (int)ID, objectTriggererItemUfps.ID, (int)currentStackSize, dropPos, rotation);

                return null;
            }

            return base.Drop(location, rotation);
#else
            return base.Drop(location, rotation);
#endif
        }

        public override bool PickupItem()
        {
            bool pickedUp = base.PickupItem();
            if (pickedUp)
            {
                transform.position = Vector3.zero; // Reset position to avoid the user from looting it twice when reloading (reloading temp. enables the item)
                InventoryAudioManager.AudioPlayOneShot(pickupSound);
            }
            return pickedUp;
        }

        public override int Use()
        {
            var used = base.Use();
            if (used < 0)
            {
                return used;
            }

            var dmgHandler = InventoryPlayerManager.instance.currentPlayer.gameObject.GetComponent<vp_FPPlayerDamageHandler>();
            if (dmgHandler != null)
            {
                dmgHandler.CurrentHealth += restoreHealthAmount;
            }

            currentStackSize--;
            if (itemCollection != null)
            {
                if (currentStackSize <= 0)
                {
                    itemCollection.NotifyItemRemoved(this, ID, index, 1);
                    itemCollection.SetItem(index, null, true);
                }

                itemCollection[index].Repaint();
            }

//            NotifyItemUsed(1, true);
            return 1;
        }
    }
}

#endif