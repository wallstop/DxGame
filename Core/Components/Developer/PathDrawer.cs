using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProtoBuf;

namespace DXGame.Core.Components.Developer
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class PathDrawer : DrawableComponent
    {
        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            PathfindingInputComponent pathfindingInput =
                Parent?.Components.OfType<PathfindingInputComponent>().FirstOrDefault();
            if (ReferenceEquals(pathfindingInput, null))
            {
                return;
            }
            SpatialComponent position = Parent?.Components.OfType<SpatialComponent>().FirstOrDefault();
            if (ReferenceEquals(position, null))
            {
                return;
            }

            var waypoints = pathfindingInput.WayPoints.ToList();
            if (!waypoints.Any())
            {
                return;
            }

            var previous = new DxVector2(position.Position.X, position.Position.Y + position.Space.Height);
            foreach (var waypoint in waypoints)
            {
                spriteBatch.DrawLine(previous, waypoint, Color.Orange, 1);
                previous = waypoint;
            }
        }
    }
}
