using System;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    //public enum PlayerState
    //{
    //    /* These can be expanded on pretty easily, WallClimbing, Running, etc */
    //    None,
    //    Walking,
    //    Jumping
    //}

    public class StateComponent : Component
    {
        private readonly List<string> states_ = new List<string>();

        public string State { get; set; }

        public StateComponent(GameObject parent = null)
            : base(parent)
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