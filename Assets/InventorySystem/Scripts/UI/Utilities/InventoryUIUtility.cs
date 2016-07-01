using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    public partial class InventoryUIUtility
    {
        #region Variables 

        public static bool isFocusedOnInput
        {
            get
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                    if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
                        return true;

                return false;
            }
        }

        public static bool isHoveringUIElement
        {
            get
            {
                return InventoryInputManager.cursorRaycastUIResults.Count > 0;
            }
        }

        /// <summary>
        /// Get the wrapper we're currently hovering over with our mouse (or touch).
        /// Convenience property.
        /// </summary>
        public static InventoryUIItemWrapperBase currentlyHoveringWrapper
        {
            get { return InventoryInputManager.instance.currentlyHoveringWrapper; }
        }

        
        /// <summary>
        /// Get the currently selected wrapper. Is null if none selected.
        /// Note that this is not the same as the hovering item.
        /// </summary>
        public static InventoryUIItemWrapperBase currentlySelectedWrapper
        {
            get
            {
                var o = currentlySelectedGameObject;
                if (o != null)
                    return o.GetComponent<InventoryUIItemWrapperBase>();

                return null;
            }
        }


        /// <summary>
        /// Get the currently selected wrapper. Is null if none selected.
        /// Note that this is not the same as the hovering item.
        /// </summary>
        public static GameObject currentlySelectedGameObject
        {
            get
            {
                if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
                    return null;

                return EventSystem.current.currentSelectedGameObject;
            }
        }

        /// <summary>
        /// Get the current mouse position, or the current touch position if this is a mobile device.
        /// </summary>
        public static Vector3 mouseOrTouchPosition
        {
            get
            {
                if (Application.isMobilePlatform)
                {
                    if (Input.touchCount > 0)
                        return Input.GetTouch(0).position;

                }

                return Input.mousePosition;
            }
        }


        /// <summary>
        /// Check if an object is allowed to get input based on the CanvasGroup.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool CanReceiveInput(GameObject obj)
        {
            if (InventoryInputManager.instance == null)
                return true;

            return InventoryInputManager.instance.CanReceiveInput(obj);
        }

        public static bool CanReceiveInput(InventoryUIItemWrapperBase wrapper)
        {
            if (wrapper == null)
                return false;

            return CanReceiveInput(wrapper.gameObject);
        }

        #endregion

        public static void PositionRectTransformAtPosition(RectTransform rectTransform, Vector3 screenPos)
        {
            var canvas = InventorySettingsManager.instance.guiRoot;

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos, canvas.worldCamera, out pos);
                rectTransform.position = canvas.transform.TransformPoint(pos);
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                rectTransform.position = screenPos;
            }


            //            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            //            {
            //                var p = screenPos;
            //#if UNITY_EDITOR
            //                // ??
            //#else
            //                p.y -= Screen.height; // TODO: Remove -- Why unity?? Why??
            //#endif
            //                rectTransform.anchoredPosition = p;
            //            }
            //            else
            //            {
            //                
            //            }
        }
    }
}