using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Utils
{
    public interface IPair<T, U>
    {
        T Key { get; }
        U Value { get; }
    }
}
