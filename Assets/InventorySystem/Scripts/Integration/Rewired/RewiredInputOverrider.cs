#if REWIRED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rewired;

namespace Devdog.InventorySystem.Integration.RewiredIntegration
{
    public class RewiredInputOverrider : IObjectTriggerInputOverrider
    {
        private Rewired.Player _player;
        private string _actionName;

        public RewiredInputOverrider(Rewired.Player player, string actionName)
        {
            this._player = player;
            this._actionName = actionName;
        }

        public bool AreKeysDown(ObjectTriggererBase triggerer)
        {
            return _player.GetButtonDown(_actionName);
        }
    }
}

#endif