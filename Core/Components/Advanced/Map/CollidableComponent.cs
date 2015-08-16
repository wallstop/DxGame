using System.Collections.Generic;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Map
{
    public enum CollidableDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    public class CollidableComponent : SpatialComponent
    {
        public List<CollidableDirection> CollidableDirections { get; } = new List<CollidableDirection>();

        public CollidableComponent(DxGame game)
            : base(game)
        {
        }

        public CollidableComponent WithCollidableDirections(List<CollidableDirection> directions)
        {
            Validate.IsNotEmpty(directions, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(directions)));
            CollidableDirections.Clear();
            CollidableDirections.AddRange(directions);
            return this;
        }
    }
}