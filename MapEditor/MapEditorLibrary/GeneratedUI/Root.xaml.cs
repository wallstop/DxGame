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
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "2.2.0.0")]
    public partial class Root : UIRoot {
        
        private Grid e_0;
        
        private StackPanel e_1;
        
        private TabControl e_2;
        
        private StackPanel e_9;
        
        private Button e_10;
        
        private Button e_11;
        
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
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            RowDefinition row_e_0_0 = new RowDefinition();
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            this.e_0.RowDefinitions.Add(row_e_0_1);
            RowDefinition row_e_0_2 = new RowDefinition();
            this.e_0.RowDefinitions.Add(row_e_0_2);
            ColumnDefinition col_e_0_0 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            ColumnDefinition col_e_0_2 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_2);
            // e_1 element
            this.e_1 = new StackPanel();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_1.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_1.Opacity = 0.95F;
            this.e_1.Orientation = Orientation.Horizontal;
            Grid.SetColumn(this.e_1, 0);
            Grid.SetRow(this.e_1, 1);
            // e_2 element
            this.e_2 = new TabControl();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.ItemsSource = Get_e_2_Items();
            // e_9 element
            this.e_9 = new StackPanel();
            this.e_1.Children.Add(this.e_9);
            this.e_9.Name = "e_9";
            this.e_9.Orientation = Orientation.Vertical;
            // e_10 element
            this.e_10 = new Button();
            this.e_9.Children.Add(this.e_10);
            this.e_10.Name = "e_10";
            this.e_10.Content = "Load";
            Binding binding_e_10_Command = new Binding("LoadCommand");
            this.e_10.SetBinding(Button.CommandProperty, binding_e_10_Command);
            // e_11 element
            this.e_11 = new Button();
            this.e_9.Children.Add(this.e_11);
            this.e_11.Name = "e_11";
            this.e_11.Content = "Delete";
            Binding binding_e_11_Command = new Binding("DeleteCommand");
            this.e_11.SetBinding(Button.CommandProperty, binding_e_11_Command);
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_e_2_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_3 element
            TabItem e_3 = new TabItem();
            e_3.Name = "e_3";
            e_3.Header = "Block";
            // blockData element
            ListBox blockData = new ListBox();
            e_3.Content = blockData;
            blockData.Name = "blockData";
            Func<UIElement, UIElement> blockData_dtFunc = blockData_dtMethod;
            blockData.ItemTemplate = new DataTemplate(blockData_dtFunc);
            Binding binding_blockData_ItemsSource = new Binding("Blocks");
            blockData.SetBinding(ListBox.ItemsSourceProperty, binding_blockData_ItemsSource);
            items.Add(e_3);
            // e_6 element
            TabItem e_6 = new TabItem();
            e_6.Name = "e_6";
            e_6.Header = "Platform";
            // platformData element
            ListBox platformData = new ListBox();
            e_6.Content = platformData;
            platformData.Name = "platformData";
            Func<UIElement, UIElement> platformData_dtFunc = platformData_dtMethod;
            platformData.ItemTemplate = new DataTemplate(platformData_dtFunc);
            Binding binding_platformData_ItemsSource = new Binding("Platforms");
            platformData.SetBinding(ListBox.ItemsSourceProperty, binding_platformData_ItemsSource);
            items.Add(e_6);
            return items;
        }
        
        private static UIElement blockData_dtMethod(UIElement parent) {
            // e_4 element
            StackPanel e_4 = new StackPanel();
            e_4.Parent = parent;
            e_4.Name = "e_4";
            // e_5 element
            Image e_5 = new Image();
            e_4.Children.Add(e_5);
            e_5.Name = "e_5";
            e_5.Height = 50F;
            e_5.Width = 50F;
            Binding binding_e_5_Source = new Binding("Tile");
            e_5.SetBinding(Image.SourceProperty, binding_e_5_Source);
            return e_4;
        }
        
        private static UIElement platformData_dtMethod(UIElement parent) {
            // e_7 element
            StackPanel e_7 = new StackPanel();
            e_7.Parent = parent;
            e_7.Name = "e_7";
            // e_8 element
            Image e_8 = new Image();
            e_7.Children.Add(e_8);
            e_8.Name = "e_8";
            Binding binding_e_8_Source = new Binding("Tile");
            e_8.SetBinding(Image.SourceProperty, binding_e_8_Source);
            return e_7;
        }
    }
}
