using UnityEngine;
using System.Collections;
using System.Globalization;

namespace Devdog.QuestSystemPro.Dialogue
{
    public interface IVariablesStringFormatter
    {
        string Format(string msg, VariablesContainer variables);
    }
}