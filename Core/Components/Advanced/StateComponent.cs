using System;
using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    public class StateComponent : Component
    {
        private readonly List<string> states_ = new List<string>();

        public string State { get; set; }

        public StateComponent(DxGame game)
            : base(game)
        {
            State = "None";
        }

        public StateComponent WithState(string state)
        {
            State = state;
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
                states_.Add(state);
            }
        }

        public List<string> GetStateList()
        {
            return states_;
        }
    }
}