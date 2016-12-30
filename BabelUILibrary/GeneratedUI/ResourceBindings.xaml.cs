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
    using EmptyKeys.UserInterface.Media.Effects;
    using EmptyKeys.UserInterface.Media.Animation;
    using EmptyKeys.UserInterface.Media.Imaging;
    using EmptyKeys.UserInterface.Shapes;
    using EmptyKeys.UserInterface.Renderers;
    using EmptyKeys.UserInterface.Themes;
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "2.6.0.0")]
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
            // Resource - [BaseStyle] Style
            Style r_1_s = new Style(typeof(Control));
            Setter r_1_s_S_0 = new Setter(Control.FontFamilyProperty, new FontFamily("Fonts/visitor_tt1_brk"));
            r_1_s.Setters.Add(r_1_s_S_0);
            this.Add("BaseStyle", r_1_s);
            // Resource - [ButtonStyle] Style
            var r_2_s_bo = this["BaseStyle"];
            Style r_2_s = new Style(typeof(Button), r_2_s_bo as Style);
            Setter r_2_s_S_0 = new Setter(Button.FontSizeProperty, 34.66667F);
            r_2_s.Setters.Add(r_2_s_S_0);
            Setter r_2_s_S_1 = new Setter(Button.OpacityProperty, 0.85F);
            r_2_s.Setters.Add(r_2_s_S_1);
            Setter r_2_s_S_2 = new Setter(Button.BackgroundProperty, new SolidColorBrush(new ColorW(255, 255, 255, 0)));
            r_2_s.Setters.Add(r_2_s_S_2);
            Setter r_2_s_S_3 = new Setter(Button.BorderBrushProperty, new SolidColorBrush(new ColorW(255, 255, 255, 0)));
            r_2_s.Setters.Add(r_2_s_S_3);
            Setter r_2_s_S_4 = new Setter(Button.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            r_2_s.Setters.Add(r_2_s_S_4);
            Setter r_2_s_S_5 = new Setter(Button.VerticalAlignmentProperty, VerticalAlignment.Center);
            r_2_s.Setters.Add(r_2_s_S_5);
            this.Add("ButtonStyle", r_2_s);
            // Resource - [VisitorFont] FontFamily
            this.Add("VisitorFont", new FontFamily("Fonts/visitor_tt1_brk"));
            ImageManager.Instance.AddImage("Menu/MainMenuBackground");
        }
    }
}
