// -----------------------------------------------------------
//  
//  This file was generated, please do not modify.
//  
// -----------------------------------------------------------
namespace EmptyKeys.UserInterface.Generated {
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Charts;
    using EmptyKeys.UserInterface.Data;
    using EmptyKeys.UserInterface.Controls;
    using EmptyKeys.UserInterface.Controls.Primitives;
    using EmptyKeys.UserInterface.Input;
    using EmptyKeys.UserInterface.Interactions.Core;
    using EmptyKeys.UserInterface.Interactivity;
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Media.Animation;
    using EmptyKeys.UserInterface.Media.Imaging;
    using EmptyKeys.UserInterface.Shapes;
    using EmptyKeys.UserInterface.Renderers;
    using EmptyKeys.UserInterface.Themes;
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "2.3.0.0")]
    public sealed class ResourceBindings : ResourceDictionary {
        
        private static ResourceBindings singleton = new ResourceBindings();
        
        public ResourceBindings() {
            this.InitializeResources();
        }
        
        public static ResourceBindings Instance {
            get {
                return singleton;
            }
        }
        
        private void InitializeResources() {
            // Resource - [BackgroundMainMenu] BitmapImage
            BitmapImage r_0_bm = new BitmapImage();
            r_0_bm.TextureAsset = "Menu/MainMenuBackground";
            this.Add("BackgroundMainMenu", r_0_bm);
            // Resource - [Visitor] FontFamily
            this.Add("Visitor", new FontFamily("Fonts/visitor_tt1_brk"));
            ImageManager.Instance.AddImage("Menu/MainMenuBackground");
        }
    }
}
