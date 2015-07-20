using DXGame.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Behavior
{
    public class Transition
    {
        public Trigger Trigger { get; }

        public Transition(Trigger trigger)
        {
            Validate.IsNotNull(trigger, "Cannot create a transition with a null trigger");
            Trigger = trigger;
        }
    }
}
