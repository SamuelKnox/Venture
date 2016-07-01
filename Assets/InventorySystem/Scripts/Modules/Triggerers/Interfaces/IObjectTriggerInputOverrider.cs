using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public interface IObjectTriggerInputOverrider
    {
        bool AreKeysDown(ObjectTriggererBase triggerer);
    }
}
