/// <summary>
/// Tag options for game objects
/// </summary>
public enum Tag
{
    Untagged,
    Respawn,
    Finish,
    EditorOnly,
    MainCamera,
    Player,
    GameController,
    Enemy,
    Neutral,
    Level,
    UI,
    Container
}

/// <summary>
/// Types of items available
/// </summary>
public enum ItemType
{
    Roon,
    MeleeWeapon,
    RangedWeapon
}

/// <summary>
/// Types of roons available
/// </summary>
public enum RoonType
{
    Red,
    Blue,
    Yellow,
    MeleeWeapon,
    Bow,
    Wand
}

/// <summary>
/// Modes for inventory UI
/// </summary>
public enum InventoryMode
{
    WeaponBrowser,
    RoonEditor
}

/// <summary>
/// Input options for gamepad
/// </summary>
public enum GamePadInputs
{
    A,
    B,
    X,
    Y
}

/// <summary>
/// Cardinal Directions
/// </summary>
public enum Direction
{
    North,
    South,
    East,
    West
}