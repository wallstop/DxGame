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
        
        private Button e_4;
        
        private Button e_5;
        
        private Button e_6;
        
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
            // e_1 element
            this.e_1 = new Image();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Opacity = 0.9F;
            Grid.SetColumnSpan(this.e_1, 6);
            Grid.SetRowSpan(this.e_1, 3);
            this.e_1.SetResourceReference(Image.SourceProperty, "BackgroundMainMenu");
            // e_2 element
            this.e_2 = new TextBlock();
            this.e_0.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_2.VerticalAlignment = VerticalAlignment.Center;
            this.e_2.Text = "Babel";
            this.e_2.FontSize = 1000F;
            Grid.SetColumn(this.e_2, 1);
            Grid.SetRow(this.e_2, 0);
            Grid.SetColumnSpan(this.e_2, 4);
            // e_3 element
            this.e_3 = new Grid();
            this.e_0.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            RowDefinition row_e_3_0 = new RowDefinition();
            row_e_3_0.Height = new GridLength(4F, GridUnitType.Star);
            this.e_3.RowDefinitions.Add(row_e_3_0);
            RowDefinition row_e_3_1 = new RowDefinition();
            row_e_3_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_3.RowDefinitions.Add(row_e_3_1);
            RowDefinition row_e_3_2 = new RowDefinition();
            row_e_3_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_3.RowDefinitions.Add(row_e_3_2);
            RowDefinition row_e_3_3 = new RowDefinition();
            row_e_3_3.Height = new GridLength(1F, GridUnitType.Star);
            this.e_3.RowDefinitions.Add(row_e_3_3);
            RowDefinition row_e_3_4 = new RowDefinition();
            row_e_3_4.Height = new GridLength(4F, GridUnitType.Star);
            this.e_3.RowDefinitions.Add(row_e_3_4);
            Grid.SetColumn(this.e_3, 2);
            Grid.SetRow(this.e_3, 0);
            Grid.SetColumnSpan(this.e_3, 2);
            Grid.SetRowSpan(this.e_3, 3);
            // e_4 element
            this.e_4 = new Button();
            this.e_3.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            this.e_4.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_4.VerticalAlignment = VerticalAlignment.Center;
            this.e_4.Content = "Play";
            Grid.SetRow(this.e_4, 1);
            // e_5 element
            this.e_5 = new Button();
            this.e_3.Children.Add(this.e_5);
            this.e_5.Name = "e_5";
            this.e_5.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_5.VerticalAlignment = VerticalAlignment.Center;
            this.e_5.Content = "Settings";
            Grid.SetRow(this.e_5, 2);
            // e_6 element
            this.e_6 = new Button();
            this.e_3.Children.Add(this.e_6);
            this.e_6.Name = "e_6";
            this.e_6.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_6.VerticalAlignment = VerticalAlignment.Center;
            this.e_6.Content = "Quit";
            Grid.SetRow(this.e_6, 3);
            FontManager.Instance.AddFont("Segoe UI", 1000F, FontStyle.Regular, "Segoe_UI_750_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(ResourceBindings.Instance);
        }
    }
}
