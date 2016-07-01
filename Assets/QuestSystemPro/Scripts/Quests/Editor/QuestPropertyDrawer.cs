using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Devdog.QuestSystemPro.Editors.ReflectionDrawers;

namespace Devdog.QuestSystemPro.Editors
{
    [CustomPropertyDrawer(typeof(Quest))]
    public class QuestPropertyDrawer : QuestPropertyDrawerBase<Quest>
    {
        
    }
}
