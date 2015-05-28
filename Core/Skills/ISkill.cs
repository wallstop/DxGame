using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Skills
{
    // TODO: Keep playing around with this
    public interface ISkill : IIdentifiable
    {
        string Name();

        TimeSpan Cooldown();

        void Activate();
    }
}
