﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    public class VariablesContainer
    {
        [OnlyDerivedTypes(typeof(Variable<>))]
        public Variable[] variables = new Variable[0];


        public Variable<T> Add<T>(T type)
        {
            var v = new Variable<T>();
            AddVariable(v);
            return v;
        }

        [Obsolete("Use Add<T> instead.", true)]
        public Variable Add(Type type)
        {
            var v = (Variable)Activator.CreateInstance(typeof(Variable<>).MakeGenericType(type));
            AddVariable(v);
            return v;
        }

        public void AddVariable(Variable variable)
        {
            var l = variables.ToList();
            l.Add(variable);
            variables = l.ToArray();
        }

        public Variable Get(Guid g)
        {
            foreach (var variable in variables)
            {
                if (variable.guid == g)
                {
                    return variable;
                }
            }

            QuestLogger.Log("Trying to get variable with GUID " + g + " - Not found.");
            return null;
        }

        public Variable<T> Get<T>(Guid g)
        {
            return (Variable<T>)Get(g);
        }

        public Variable Get(string name)
        {
            foreach (var variable in variables)
            {
                if (variable.name == name)
                {
                    return variable;
                }
            }

            QuestLogger.Log("Trying to get variable with name " + name + " - Not found.");
            return null;
        }

        public Variable<T> Get<T>(string name)
        {
            return (Variable<T>)Get(name);
        }
    }
}