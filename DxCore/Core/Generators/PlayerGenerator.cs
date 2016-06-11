using System;
using System.Collections.Generic;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Primitives;

namespace DxCore.Core.Generators
{
    [Obsolete("Kill this ASAP and determine a better way to inject behavior into a game instance")]
    public interface IPlayerGenerator
    {
        IPlayerGenerator From(DxVector2 playerPosition);
        GameObject GeneratePlayer(AbstractCommandComponent playerCommander, bool isActivePlayer);
        GameObject GeneratePlayer();
        List<GameObject> Generate();
    }
}