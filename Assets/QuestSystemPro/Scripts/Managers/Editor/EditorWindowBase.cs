using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors
{
    public class EditorWindowBase : UnityEditor.EditorWindow
    {
//        /// <summary>
//        /// Called 10x a second -> Repaints the OnGUI Call
//        /// </summary>
//        protected virtual void OnInspectorUpdate()
//        {
//            Repaint();
//        }

        protected virtual void Update()
        {
            Repaint();
        }

        protected virtual void OnGUI()
        {
            HandleEvents();
        }

        private void HandleEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    OnMouseDown(Event.current.button);
//                    Event.current.Use();
                    break;
                case EventType.MouseUp:
                    OnMouseUp(Event.current.button);
//                    Event.current.Use();
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag(Event.current.delta);
                    Event.current.Use();
                    break;
                case EventType.KeyDown:
                    OnKeyDown(Event.current.keyCode);
//                    Event.current.Use();
                    break;
                case EventType.KeyUp:
                    OnKeyUp(Event.current.keyCode);
//                    Event.current.Use();
                    break;
                case EventType.ScrollWheel:
                    OnScroll(Event.current.delta);
                    Event.current.Use();
                    break;
                case EventType.Repaint:
                case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                case EventType.Used:
                case EventType.ExecuteCommand:
                case EventType.ValidateCommand:
                case EventType.Ignore:
                    break;
                case EventType.ContextClick:
                    OnContextClick();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

//            Event.current.Use();
        }

        protected virtual void OnMouseDown(int button)
        {

        }

        protected virtual void OnMouseUp(int button)
        {

        }

        protected virtual void OnMouseDrag(Vector2 delta)
        {
//            Debug.Log("Mouse drag " + delta);
        }

        protected virtual void OnKeyDown(KeyCode keyCode)
        {

        }

        protected virtual void OnKeyUp(KeyCode keyCode)
        {

        }

        protected virtual void OnScroll(Vector2 delta)
        {

        }

        protected virtual void OnContextClick()
        {

        }
    }
}
