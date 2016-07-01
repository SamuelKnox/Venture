#if UFPS

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Devdog.InventorySystem.Integration.UFPS
{
    public class InventoryVPFPWeaponHandler : vp_FPWeaponHandler
    {

        /// <summary>
        /// 
        /// </summary>
        protected override bool OnAttempt_AutoReload()
        {
            if (CurrentWeapon == null)
                return false;

            return base.OnAttempt_AutoReload();
        }
    }
}

#endif