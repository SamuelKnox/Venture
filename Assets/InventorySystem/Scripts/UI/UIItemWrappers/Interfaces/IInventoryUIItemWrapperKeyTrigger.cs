using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public interface IInventoryUIItemWrapperKeyTrigger
    {

        string keyCombination { get; set; }
        void Repaint();

    }
}
