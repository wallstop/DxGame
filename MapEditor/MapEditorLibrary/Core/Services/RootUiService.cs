using System;
using System.Collections.Generic;
using DxCore.Core.Map;
using DxCore.Core.Services;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using MapEditorLibrary.Controls;
using MapEditorLibrary.Core.Services.Components;
using WallNetCore.Validate;

namespace MapEditorLibrary.Core.Services
{
    public class RootUiService : DxService
    {
        public Tile SelectedTile => AssetManagerView.SelectedTile;
        public TileModel SelectedTileModel => AssetManagerView.SelectedTileModel;
        public Root UI { get; }

        private AssetManagerView AssetManagerView { get; }

        private UiDrawer UiDrawer { get; set; }

        public RootUiService(Root rootUi)
        {
            Validate.Hard.IsNotNullOrDefault(rootUi);
            UI = rootUi;
            AssetManagerView = new AssetManagerView();
            UI.DataContext = AssetManagerView;
            foreach(KeyValuePair<RoutedEvent, Delegate> eventAndHandler in AssetManagerView.Handlers)
            {
                UI.AddHandler(eventAndHandler.Key, eventAndHandler.Value);
            }
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(UiDrawer))
            {
                UiDrawer = new UiDrawer(UI);
                Self.AttachComponent(UiDrawer);
            }
        }
    }
}