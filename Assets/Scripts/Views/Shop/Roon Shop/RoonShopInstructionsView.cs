using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoonShopInstructionsView : MonoBehaviour
{
    [Tooltip("Instructions for when A Button is pressed")]
    [SerializeField]
    private RectTransform aButton;

    [Tooltip("Instructions for when B Button is pressed")]
    [SerializeField]
    private RectTransform bButton;

    [Tooltip("Instructions for when X Button is pressed")]
    [SerializeField]
    private RectTransform xButton;

    [Tooltip("Instructions for when Y Button is pressed")]
    [SerializeField]
    private RectTransform yButton;
}