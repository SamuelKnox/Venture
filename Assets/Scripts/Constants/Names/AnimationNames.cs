﻿public static class AnimationNames
{
    public static class Enemy
    {
        public static class Bools
        {
            /// <summary>
            /// Animation name for when enemy dies
            /// </summary>
            public const string Death = "Dead";
        }

        public static class Floats
        {
            /// <summary>
            /// Animation name for an enemy's horizontal speed
            /// </summary>
            public const string HorizontalSpeed = "Horizontal Speed";

            /// <summary>
            /// Animation name for an enemy's vertical speed
            /// </summary>
            public const string VerticalSpeed = "Vertical Speed";
        }
        public static class Triggers
        {

            /// <summary>
            /// Animation name for enemy's primary attack
            /// </summary>
            public const string Attack = "Attack";

            /// <summary>
            /// Animation name for when enemy is hurt
            /// </summary>
            public const string Hurt = "Hurt";
        }
    }

    public static class Player
    {
        public static class Bools
        {
            /// <summary>
            /// Animation name for whether or not the player is climbing
            /// </summary>
            public const string Climbing = "Climbing";

            /// <summary>
            /// Animation name for when the player is dead
            /// </summary>
            public const string Dead = "Dead";
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
            /// Animation name for player collecting an item
            /// </summary>
            public const string CollectItem = "Collect Item";

            public const string CollectSpecialItem = "Collect Special Item";
        }
    }
}