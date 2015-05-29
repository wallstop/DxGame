﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class StateComponent : Component
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (StateComponent));
        [DataMember] private readonly HashSet<string> states_ = new HashSet<string>();

        [DataMember]
        public string State { get; set; }

        public IEnumerable<string> States => states_;

        public StateComponent(DxGame game)
            : base(game)
        {
            State = "None";
        }

        public StateComponent WithState(string state)
        {
            Validate.IsNotNull(state, $"Cannot initialize a {GetType()} with a null {nameof(state)}");
            Validate.IsTrue(states_.Contains(state),
                $"Cannot initialize a {GetType()} with an invalid state ({state}). Valid states: {states_}");

            State = state;

            return this;
        }

        public void AddState(string state)
        {
            states_.Add(state);
        }

        public void AddStates(params string[] states)
        {
            foreach (var state in states)
            {
                AddState(state);
            }
        }
    }
}