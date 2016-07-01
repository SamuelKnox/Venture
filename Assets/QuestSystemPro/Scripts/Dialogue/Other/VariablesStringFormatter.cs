using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Devdog.QuestSystemPro.Dialogue
{
    public class VariablesStringFormatter : IVariablesStringFormatter
    {
        private readonly Regex _regex;

        public VariablesStringFormatter()
            : this(@"(?:{\s?(.+?)\s?})")
        { }

        public VariablesStringFormatter(string regex)
        {
            _regex = new Regex(regex, RegexOptions.Multiline);
        }

        public string Format(string msg, VariablesContainer variables)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }

            var matches = _regex.Matches(msg);
            foreach (Match match in matches)
            {
                var variableWithBraces = match.Groups[0].Value;
                var variableName = match.Groups[1].Value;
                var variable = variables.Get(variableName);

                msg = msg.Replace(variableWithBraces, variable.ToString());
            }

            return msg;
        }
    }
}
