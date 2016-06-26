using System.Collections.Generic;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Validate;
using MapEditorLibrary.Controls;

namespace MapEditorLibrary.Core.Messaging
{
    public class AddTileToMap : Message
    {
        public TileModel Tile { get; }

        public AddTileToMap(TileModel tile)
        {
            Validate.Hard.IsNotNullOrDefault(tile);
            Tile = tile;
        }
    }
}
