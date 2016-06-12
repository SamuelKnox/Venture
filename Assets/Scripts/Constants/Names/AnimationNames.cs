﻿public static class AnimationNames {
    public static class Player
    {
        public static class Triggers
        {
            /// <summary>
            /// Animation name for player's melee attack
            /// </summary>
            public const string MeleeWeaponAttack = "Melee Weapon Attack";

            /// <summary>
            /// Animation name for player's ranged attack
            /// </summary>
            public const string BowAttack = "Bow Attack";

            /// <summary>
            /// Animation name for player when casting spell
            /// </summary>
            public const string WandAttack = "Wand Attack";

            /// <summary>
            /// Animation name for intiating the player's jump
            /// </summary>
            public const string Jump = "Jump";

            /// <summary>
            /// Animation name for player collecting an item
            /// </summary>
            public const string CollectItem = "Collect Item";
        }

        public static class Floats
        {
            /// <summary>
            /// Animation name for player's horizontal speed
            /// </summary>
            public const string HorizontalSpeed = "Horizontal Speed";

            /// <summary>
            /// Animation name for player's vertical speed
            /// </summary>
            public const string VerticalSpeed = "Vertical Speed";
        }
    }
}