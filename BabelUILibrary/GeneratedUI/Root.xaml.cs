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
    public partial class Root : UIRoot {
        
        private Grid e_0;
        
        private Image e_1;
        
        private TextBlock e_2;
        
        private Grid e_3;
        
        private MainMenu e_4;
        
        private Settings e_5;
        
        public Root() : 
                base() {
            this.Initialize();
        }
        
        public Root(int width, int height) : 
                base(width, height) {
            this.Initialize();
        }
        
        private void Initialize() {
            Style style = RootStyle.CreateRootStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            this.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.Background.Opacity = 0F;
            InitializeElementResources(this);
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            RowDefinition row_e_0_0 = new RowDefinition();
            row_e_0_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            row_e_0_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_1);
            RowDefinition row_e_0_2 = new RowDefinition();
            row_e_0_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_2);
            RowDefinition row_e_0_3 = new RowDefinition();
            row_e_0_3.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_3);
            RowDefinition row_e_0_4 = new RowDefinition();
            row_e_0_4.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_4);
            RowDefinition row_e_0_5 = new RowDefinition();
            row_e_0_5.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_5);
            ColumnDefinition col_e_0_0 = new ColumnDefinition();
            col_e_0_0.Width = new GridLength(3F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            col_e_0_1.Width = new GridLength(3F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            ColumnDefinition col_e_0_2 = new ColumnDefinition();
            col_e_0_2.Width = new GridLength(4F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_2);
            ColumnDefinition col_e_0_3 = new ColumnDefinition();
            col_e_0_3.Width = new GridLength(4F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_3);
            ColumnDefinition col_e_0_4 = new ColumnDefinition();
            col_e_0_4.Width = new GridLength(3F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_4);
            ColumnDefinition col_e_0_5 = new ColumnDefinition();
            col_e_0_5.Width = new GridLength(3F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_5);
            Binding binding_e_0_Visibility = new Binding("RootVisible");
            this.e_0.SetBinding(Grid.VisibilityProperty, binding_e_0_Visibility);
            Binding binding_e_0_IsEnabled = new Binding("RootVisible");
            this.e_0.SetBinding(Grid.IsEnabledProperty, binding_e_0_IsEnabled);
            // e_1 element
            this.e_1 = new Image();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Opacity = 0.7F;
            this.e_1.Stretch = Stretch.Uniform;
            Grid.SetColumnSpan(this.e_1, 6);
            Grid.SetRowSpan(this.e_1, 6);
            this.e_1.SetResourceReference(Image.SourceProperty, "BackgroundMainMenu");
            // e_2 element
            this.e_2 = new TextBlock();
            this.e_0.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_2.VerticalAlignment = VerticalAlignment.Center;
            this.e_2.Opacity = 0.9F;
            this.e_2.Foreground = new SolidColorBrush(new ColorW(75, 0, 130, 255));
            this.e_2.Text = "Babel";
            this.e_2.FontSize = 100F;
            Grid.SetColumn(this.e_2, 1);
            Grid.SetRow(this.e_2, 0);
            Grid.SetColumnSpan(this.e_2, 4);
            Grid.SetRowSpan(this.e_2, 2);
            this.e_2.SetResourceReference(TextBlock.FontFamilyProperty, "VisitorFont");
            // e_3 element
            this.e_3 = new Grid();
            this.e_0.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            Grid.SetColumn(this.e_3, 1);
            Grid.SetRow(this.e_3, 1);
            Grid.SetColumnSpan(this.e_3, 4);
            Grid.SetRowSpan(this.e_3, 4);
            // e_4 element
            this.e_4 = new MainMenu();
            this.e_3.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            Binding binding_e_4_Visibility = new Binding("MainMenuVisible");
            this.e_4.SetBinding(MainMenu.VisibilityProperty, binding_e_4_Visibility);
            // e_5 element
            this.e_5 = new Settings();
            this.e_3.Children.Add(this.e_5);
            this.e_5.Name = "e_5";
            Binding binding_e_5_DataContext = new Binding("SettingsController");
            this.e_5.SetBinding(Settings.DataContextProperty, binding_e_5_DataContext);
            Binding binding_e_5_Visibility = new Binding("SettingsMenuVisible");
            this.e_5.SetBinding(Settings.VisibilityProperty, binding_e_5_Visibility);
            FontManager.Instance.AddFont("Fonts/visitor_tt1_brk", 100F, FontStyle.Regular, "Fonts/visitor_tt1_brk_75_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(ResourceBindings.Instance);
        }
    }
}
