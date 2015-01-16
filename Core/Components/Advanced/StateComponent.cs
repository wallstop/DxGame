using System;
using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class StateComponent : Component
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(StateComponent));

        private readonly HashSet<string> states_ = new HashSet<string>();

        public string State { get; set; }

        public StateComponent(DxGame game)
            : base(game)
        {
            State = "None";
        }

        public StateComponent WithState(string state)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(state), "StateComponent cannot have its state to an empty state");
            Debug.Assert(states_.Contains(state),
                String.Format("StateComponent cannot have its state set to one it doesn't know about: {0}, {1}", state,
                    states_));
            if (!states_.Contains(state))
            {
                State = state;
            }

            return this;
        }

        public void AddState(String state)
        {
            states_.Add(state);
        }

        public void AddStates(params String[] states)
        {
            foreach (var state in states)
            {
                AddState(state);
            }
        }

        public IEnumerable<string> States
        {
            get { return states_; }
        }
    }
}