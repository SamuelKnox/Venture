using UnityEngine;
using Devdog.InventorySystem;
using Devdog.InventorySystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomInfoBox : InfoBoxUI
{
    private static readonly Vector2 DefaultPivot = Vector2.zero;

    private RectTransform rectTransform;
    private Vector2 position;

    void OnEnable()
    {
        MenuController.OnSelectedWrapperChanged += OnSelectedWrapperChanged;
    }

    void OnDisable()
    {
        MenuController.OnSelectedWrapperChanged -= OnSelectedWrapperChanged;
    }

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Repositions the info box
    /// </summary>
    protected override void PositionInfoBox()
    {
        base.PositionInfoBox();
        InventoryUIUtility.PositionRectTransformAtPosition(rectTransform, position);
        HandleBorders();
    }

    /// <summary>
    /// Handles the need for an updated info box
    /// </summary>
    /// <param name="wrapper">Currently selected wrapper</param>
    private void OnSelectedWrapperChanged(InventoryUIItemWrapperBase wrapper)
    {
        if (wrapper)
        {
            position = wrapper.transform.position;
            HandleInfoBox(wrapper.item);
        }
        else
        {
            HandleInfoBox(null);
        }
    }

    protected override void HandleBorders()
    {
        if (InventorySettingsManager.instance.guiRoot.renderMode == RenderMode.WorldSpace)
        {
            return;
        }
        if (moveWhenHitBorderHorizontal)
        {
            Debug.Log("need to realign");
        //    if (position.x + rectTransform.sizeDelta.x > Screen.width - borderMargins.x)
        //    {
        //        rectTransform.pivot = new Vector2(DefaultPivot.y, rectTransform.pivot.x);
        //    }
        //    else
        //    {
        //        rectTransform.pivot = new Vector2(DefaultPivot.x, rectTransform.pivot.y);
        //    }
        }
        if (moveWhenHitBorderVertical)
        {
            Debug.Log("need to realign");
            //if (position.y - rectTransform.sizeDelta.y < 0.0f - borderMargins.y)
            //{
            //    rectTransform.pivot = new Vector2(rectTransform.pivot.x, DefaultPivot.x);
            //}
            //else
            //{
            //    rectTransform.pivot = new Vector2(rectTransform.pivot.x, DefaultPivot.y);
            //}
        }
    }
}
