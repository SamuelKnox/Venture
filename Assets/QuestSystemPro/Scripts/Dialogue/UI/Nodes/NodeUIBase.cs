using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Devdog.QuestSystemPro.UI;

namespace Devdog.QuestSystemPro.Dialogue.UI
{
    public abstract class NodeUIBase : MonoBehaviour
    {
        public Text message;
        public RectTransform playerDecisionsContainer;

        public string moveToNextNodeText = "Next";

        [Header("Prefabs")]
        public PlayerDecisionUI defaultPlayerDecisionUIPrefab;

        [NonSerialized]
        protected List<PlayerDecisionUI> decisions = new List<PlayerDecisionUI>();

        [NonSerialized]
        protected NodeBase currentNode;

        [NonSerialized]
        protected IVariablesStringFormatter variablesStringFormatter;

        protected virtual void Awake()
        {
            variablesStringFormatter = new VariablesStringFormatter();
        }

        public virtual void Repaint(NodeBase node)
        {
            decisions.Clear();
            currentNode = node;
            RemoveOldDecisions();

            SetText(variablesStringFormatter.Format(currentNode.message, currentNode.owner.variables));
            SetDecisions();
        }

        protected void RemoveOldDecisions()
        {
            foreach (Transform t in playerDecisionsContainer)
            {
                Destroy(t.gameObject);
            }
        }

        protected virtual void SetText(string msg)
        {
            var textAnimator = message.GetComponent<ITextAnimator>();
            if (textAnimator != null)
            {
                textAnimator.AnimateText(msg);
            }
            else
            {
                message.text = msg;
            }
        }

        protected virtual void SetDecisions()
        {
            SetDefaultPlayerDecision();
        }

        protected virtual void SetDefaultPlayerDecision()
        {
            var playerDecisionInst = UnityEngine.Object.Instantiate<PlayerDecisionUI>(defaultPlayerDecisionUIPrefab);
            playerDecisionInst.transform.SetParent(playerDecisionsContainer);
            UIUtility.ResetTransform(playerDecisionInst.transform);

            playerDecisionInst.Repaint(new PlayerDecision() {option = moveToNextNodeText}, null, currentNode.edges.Length == 0 || currentNode.edges.Any(o => o.CanUse(currentNode.owner)));
            playerDecisionInst.button.onClick.AddListener(OnDefaultPlayerDecisionClicked);

            decisions.Add(playerDecisionInst);
        }

        protected virtual void OnDefaultPlayerDecisionClicked()
        {
            currentNode.Finish(true);
        }
    }
}