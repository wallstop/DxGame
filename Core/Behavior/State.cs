﻿using System.Collections.Generic;
using DXGame.Core.Utils;

namespace DXGame.Core.Behavior
{
    public class State
    {
        public ICollection<Transition> Transitions { get; }
        public string Name { get; }

        public State(string name)
        {
            Validate.IsNotNullOrDefault(name, $"Cannot create a {nameof(State)} with a null name");
            Name = name;
            Transitions = new HashSet<Transition>();
        }

        public State WithTransition(Transition transition)
        {
            Validate.IsNotNull(transition, $"Cannot add a null {nameof(transition)} to a {nameof(State)}");
            Transitions.Add(transition);
            return this;
        }
    }
}