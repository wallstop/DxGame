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
        
        private Grid e_1;
        
        private TextBlock e_2;
        
        private Slider mapTileWidth;
        
        private StackPanel e_3;
        
        private TabControl e_4;
        
        private Grid e_9;
        
        private Button e_10;
        
        private Button e_11;
        
        private Button e_12;
        
        private Button e_13;
        
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
            this.e_1 = new Grid();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_1.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_1.Opacity = 0.95F;
            RowDefinition row_e_1_0 = new RowDefinition();
            row_e_1_0.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_1.RowDefinitions.Add(row_e_1_0);
            RowDefinition row_e_1_1 = new RowDefinition();
            row_e_1_1.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_1.RowDefinitions.Add(row_e_1_1);
            RowDefinition row_e_1_2 = new RowDefinition();
            row_e_1_2.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_1.RowDefinitions.Add(row_e_1_2);
            RowDefinition row_e_1_3 = new RowDefinition();
            row_e_1_3.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_1.RowDefinitions.Add(row_e_1_3);
            RowDefinition row_e_1_4 = new RowDefinition();
            row_e_1_4.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_1.RowDefinitions.Add(row_e_1_4);
            ColumnDefinition col_e_1_0 = new ColumnDefinition();
            this.e_1.ColumnDefinitions.Add(col_e_1_0);
            ColumnDefinition col_e_1_1 = new ColumnDefinition();
            this.e_1.ColumnDefinitions.Add(col_e_1_1);
            ColumnDefinition col_e_1_2 = new ColumnDefinition();
            this.e_1.ColumnDefinitions.Add(col_e_1_2);
            ColumnDefinition col_e_1_3 = new ColumnDefinition();
            this.e_1.ColumnDefinitions.Add(col_e_1_3);
            Grid.SetColumn(this.e_1, 1);
            Grid.SetRow(this.e_1, 0);
            // e_2 element
            this.e_2 = new TextBlock();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.VerticalAlignment = VerticalAlignment.Center;
            this.e_2.Text = "Map Tile Width";
            // mapTileWidth element
            this.mapTileWidth = new Slider();
            this.e_1.Children.Add(this.mapTileWidth);
            this.mapTileWidth.Name = "mapTileWidth";
            this.mapTileWidth.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.mapTileWidth.VerticalAlignment = VerticalAlignment.Center;
            this.mapTileWidth.Minimum = 1F;
            this.mapTileWidth.Maximum = 150F;
            Grid.SetColumn(this.mapTileWidth, 1);
            // e_3 element
            this.e_3 = new StackPanel();
            this.e_0.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            this.e_3.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_3.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_3.Opacity = 0.95F;
            this.e_3.Background = new SolidColorBrush(new ColorW(0, 0, 0, 255));
            this.e_3.Orientation = Orientation.Horizontal;
            Grid.SetColumn(this.e_3, 0);
            Grid.SetRow(this.e_3, 1);
            // e_4 element
            this.e_4 = new TabControl();
            this.e_3.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            this.e_4.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_4.ItemsSource = Get_e_4_Items();
            // e_9 element
            this.e_9 = new Grid();
            this.e_3.Children.Add(this.e_9);
            this.e_9.Name = "e_9";
            this.e_9.Margin = new Thickness(2F, 2F, 2F, 2F);
            RowDefinition row_e_9_0 = new RowDefinition();
            this.e_9.RowDefinitions.Add(row_e_9_0);
            RowDefinition row_e_9_1 = new RowDefinition();
            this.e_9.RowDefinitions.Add(row_e_9_1);
            RowDefinition row_e_9_2 = new RowDefinition();
            this.e_9.RowDefinitions.Add(row_e_9_2);
            RowDefinition row_e_9_3 = new RowDefinition();
            this.e_9.RowDefinitions.Add(row_e_9_3);
            // e_10 element
            this.e_10 = new Button();
            this.e_9.Children.Add(this.e_10);
            this.e_10.Name = "e_10";
            this.e_10.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_10.Content = "Load Tile";
            Grid.SetRow(this.e_10, 0);
            Binding binding_e_10_Command = new Binding("LoadTileCommand");
            this.e_10.SetBinding(Button.CommandProperty, binding_e_10_Command);
            // e_11 element
            this.e_11 = new Button();
            this.e_9.Children.Add(this.e_11);
            this.e_11.Name = "e_11";
            this.e_11.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_11.Content = "Delete Tile";
            Grid.SetRow(this.e_11, 1);
            Binding binding_e_11_Command = new Binding("DeleteTileCommand");
            this.e_11.SetBinding(Button.CommandProperty, binding_e_11_Command);
            // e_12 element
            this.e_12 = new Button();
            this.e_9.Children.Add(this.e_12);
            this.e_12.Name = "e_12";
            this.e_12.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_12.Content = "Load Map";
            Grid.SetRow(this.e_12, 2);
            Binding binding_e_12_Command = new Binding("LoadMapCommand");
            this.e_12.SetBinding(Button.CommandProperty, binding_e_12_Command);
            // e_13 element
            this.e_13 = new Button();
            this.e_9.Children.Add(this.e_13);
            this.e_13.Name = "e_13";
            this.e_13.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_13.Content = "Save Map";
            Grid.SetRow(this.e_13, 3);
            Binding binding_e_13_Command = new Binding("SaveMapCommand");
            this.e_13.SetBinding(Button.CommandProperty, binding_e_13_Command);
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_e_4_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_5 element
            TabItem e_5 = new TabItem();
            e_5.Name = "e_5";
            e_5.Margin = new Thickness(2F, 2F, 2F, 2F);
            e_5.Header = "Block";
            // blockData element
            ListBox blockData = new ListBox();
            e_5.Content = blockData;
            blockData.Name = "blockData";
            Func<UIElement, UIElement> blockData_dtFunc = blockData_dtMethod;
            blockData.ItemTemplate = new DataTemplate(blockData_dtFunc);
            Binding binding_blockData_ItemsSource = new Binding("Blocks");
            blockData.SetBinding(ListBox.ItemsSourceProperty, binding_blockData_ItemsSource);
            Binding binding_blockData_SelectedItem = new Binding("SelectedBlock");
            binding_blockData_SelectedItem.Mode = BindingMode.OneWayToSource;
            blockData.SetBinding(ListBox.SelectedItemProperty, binding_blockData_SelectedItem);
            items.Add(e_5);
            // e_7 element
            TabItem e_7 = new TabItem();
            e_7.Name = "e_7";
            e_7.Margin = new Thickness(2F, 2F, 2F, 2F);
            e_7.Header = "Platform";
            // platformData element
            ListBox platformData = new ListBox();
            e_7.Content = platformData;
            platformData.Name = "platformData";
            Func<UIElement, UIElement> platformData_dtFunc = platformData_dtMethod;
            platformData.ItemTemplate = new DataTemplate(platformData_dtFunc);
            Binding binding_platformData_ItemsSource = new Binding("Platforms");
            platformData.SetBinding(ListBox.ItemsSourceProperty, binding_platformData_ItemsSource);
            Binding binding_platformData_SelectedItem = new Binding("SelectedPlatform");
            binding_platformData_SelectedItem.Mode = BindingMode.OneWayToSource;
            platformData.SetBinding(ListBox.SelectedItemProperty, binding_platformData_SelectedItem);
            items.Add(e_7);
            return items;
        }
        
        private static UIElement blockData_dtMethod(UIElement parent) {
            // e_6 element
            Image e_6 = new Image();
            e_6.Parent = parent;
            e_6.Name = "e_6";
            e_6.MaxHeight = 50F;
            e_6.MinHeight = 50F;
            e_6.MaxWidth = 50F;
            e_6.MinWidth = 50F;
            Binding binding_e_6_Source = new Binding("Tile");
            e_6.SetBinding(Image.SourceProperty, binding_e_6_Source);
            return e_6;
        }
        
        private static UIElement platformData_dtMethod(UIElement parent) {
            // e_8 element
            Image e_8 = new Image();
            e_8.Parent = parent;
            e_8.Name = "e_8";
            Binding binding_e_8_Source = new Binding("Tile");
            e_8.SetBinding(Image.SourceProperty, binding_e_8_Source);
            return e_8;
        }
    }
}
