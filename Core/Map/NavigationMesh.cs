using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class NavigationMesh
    {
        private static ThreadLocal<Dictionary<UniqueId, NavigationMesh>> CACHE =
            new ThreadLocal<Dictionary<UniqueId, NavigationMesh>>(() => new Dictionary<UniqueId, NavigationMesh>());

        public struct Node
        {
            private DxVector2 Position { get; set; }


        }



        public NavigationMesh(MapModel mapModel)
        {
            Validate.IsNotNullOrDefault(mapModel, StringUtils.GetFormattedNullOrDefaultMessage(this, mapModel));


        }

        private static NavigationMesh PopulateOrRetrieveMesh(MapModel mapModel)
        {
            var cachedNavigationMesh = CACHE.Value;
            var mapId = mapModel.Map.Id;
            if (cachedNavigationMesh.ContainsKey(mapId))
            {
                return cachedNavigationMesh[mapId];
            }

            // TODO
            return null;

        }

    }
}
