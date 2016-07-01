#if REWIRED

using UnityEngine;
using System.Collections;
using System.Linq;
using Devdog.InventorySystem.UI;
using Rewired;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Integration.RewiredIntegration
{
    [AddComponentMenu("InventorySystem/Integration/Rewired/Rewired Input Manager")]
    public class RewiredInputManager : MonoBehaviour
    {
        [Header("Rewired for item pickups")]
        public bool useSystemPlayer = false;
        public int rewiredPlayerID = 0;
        public string rewiredActionName;

        protected Rewired.Player player;
        protected virtual void Awake()
        {
            if (useSystemPlayer)
            {
                player = Rewired.ReInput.players.GetSystemPlayer();
            }
            else
            {
                player = Rewired.ReInput.players.GetPlayer(rewiredPlayerID);
            }

            Assert.IsNotNull(player, "Rewired player with ID " + rewiredPlayerID + " could not be found!");
            Assert.IsTrue(Rewired.ReInput.mapping.Actions.Any(o => o.name == rewiredActionName), "No rewired action found with name: " + rewiredActionName);

            Debug.Log("Overwriting default Inventory Pro input module -> Replacing with Rewired input.");
            InventoryInputManager.instance.objectTriggererItemInputOverrider = new RewiredInputOverrider(player, rewiredActionName);
        }
    }
}

#endif