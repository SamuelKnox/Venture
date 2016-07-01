﻿/// <summary>
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
    Rune,
    MeleeWeapon,
    RangedWeapon,
    Boots,
    Gloves,
    Helmet,
    Leggings,
}

/// <summary>
/// Types of runes available
/// </summary>
public enum RuneType
{
    Health,
    Poison,
    Speed
}