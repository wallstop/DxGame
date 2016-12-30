using System.Collections.Generic;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using WallNetCore.Validate;

namespace DxCore.Core.Services.Components
{
    public sealed class NetworkProcessor : Component
    {
        private List<NetworkComponent> NetworkComponents { get; }

        public NetworkProcessor(List<NetworkComponent> networkComponents)
        {
            Validate.Hard.IsNotNull(networkComponents);
            NetworkComponents = networkComponents;
        }

        protected override void Update(DxGameTime gameTime)
        {
            foreach(NetworkComponent networkComponent in NetworkComponents)
            {
                networkComponent.Process(gameTime);
            }
        }
    }
}