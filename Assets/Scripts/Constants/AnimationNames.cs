public static class AnimationNames
{
    public static class Enemy
    {
        public static class Bools
        {
            /// <summary>
            /// Animation name for when enemy dies
            /// </summary>
            public const string Death = "Dead";

            /// <summary>
            /// Animation name for when enemy is stunned
            /// </summary>
            public const string Stunned = "Stunned";
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

            /// <summary>
            /// Animation name for when the player is stunned
            /// </summary>
            public const string Stunned = "Stunned";
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

            /// <summary>
            /// Animation name for horizontal input
            /// </summary>
            public const string HorizontalInput = "Horizontal Input";

            /// <summary>
            /// Animation name for vertical input
            /// </summary>
            public const string VerticalInput = "Vertical Input";
        }

        public static class Triggers
        {
            /// <summary>
            /// Animation name for player's melee attack
            /// </summary>
            public const string MeleeWeaponAttack = "Melee Weapon Attack";

            /// <summary>
            /// Animation name for player's bow draw
            /// </summary>
            public const string BowDraw = "Bow Draw";

            /// <summary>
            /// Animation name for player's bow attack
            /// </summary>
            public const string BowAttack = "Bow Attack";

            /// <summary>
            /// Animation name for player's wand prepartion
            /// </summary>
            public const string WandPreparation = "Wand Preparation";

            /// <summary>
            /// Animation name for player when casting spell
            /// </summary>
            public const string WandAttack = "Wand Attack";

            /// <summary>
            /// Animation name for player collecting an item
            /// </summary>
            public const string CollectItem = "Collect Item";

            /// <summary>
            /// Animation name for collecting a special item
            /// </summary>
            public const string CollectSpecialItem = "Collect Special Item";
        }
    }
}