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
        
        private Grid e_21;
        
        private Button e_22;
        
        private Button e_23;
        
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
            row_e_0_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            row_e_0_1.Height = new GridLength(2F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_1);
            RowDefinition row_e_0_2 = new RowDefinition();
            row_e_0_2.Height = new GridLength(1F, GridUnitType.Star);
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
            // e_21 element
            this.e_21 = new Grid();
            this.e_3.Children.Add(this.e_21);
            this.e_21.Name = "e_21";
            this.e_21.Margin = new Thickness(2F, 2F, 2F, 2F);
            RowDefinition row_e_21_0 = new RowDefinition();
            row_e_21_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_21.RowDefinitions.Add(row_e_21_0);
            RowDefinition row_e_21_1 = new RowDefinition();
            row_e_21_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_21.RowDefinitions.Add(row_e_21_1);
            RowDefinition row_e_21_2 = new RowDefinition();
            row_e_21_2.Height = new GridLength(2F, GridUnitType.Star);
            this.e_21.RowDefinitions.Add(row_e_21_2);
            RowDefinition row_e_21_3 = new RowDefinition();
            row_e_21_3.Height = new GridLength(2F, GridUnitType.Star);
            this.e_21.RowDefinitions.Add(row_e_21_3);
            // e_22 element
            this.e_22 = new Button();
            this.e_21.Children.Add(this.e_22);
            this.e_22.Name = "e_22";
            this.e_22.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_22.Content = "Load Map";
            Grid.SetRow(this.e_22, 0);
            Binding binding_e_22_Command = new Binding("LoadMapCommand");
            this.e_22.SetBinding(Button.CommandProperty, binding_e_22_Command);
            // e_23 element
            this.e_23 = new Button();
            this.e_21.Children.Add(this.e_23);
            this.e_23.Name = "e_23";
            this.e_23.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_23.Content = "Save Map";
            Grid.SetRow(this.e_23, 1);
            Binding binding_e_23_Command = new Binding("SaveMapCommand");
            this.e_23.SetBinding(Button.CommandProperty, binding_e_23_Command);
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_e_4_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_5 element
            TabItem e_5 = new TabItem();
            e_5.Name = "e_5";
            e_5.Margin = new Thickness(2F, 2F, 2F, 2F);
            e_5.Header = "Block";
            // e_6 element
            Grid e_6 = new Grid();
            e_5.Content = e_6;
            e_6.Name = "e_6";
            RowDefinition row_e_6_0 = new RowDefinition();
            row_e_6_0.Height = new GridLength(1F, GridUnitType.Star);
            e_6.RowDefinitions.Add(row_e_6_0);
            RowDefinition row_e_6_1 = new RowDefinition();
            row_e_6_1.Height = new GridLength(5F, GridUnitType.Star);
            e_6.RowDefinitions.Add(row_e_6_1);
            // e_7 element
            Grid e_7 = new Grid();
            e_6.Children.Add(e_7);
            e_7.Name = "e_7";
            ColumnDefinition col_e_7_0 = new ColumnDefinition();
            e_7.ColumnDefinitions.Add(col_e_7_0);
            ColumnDefinition col_e_7_1 = new ColumnDefinition();
            e_7.ColumnDefinitions.Add(col_e_7_1);
            // e_8 element
            Button e_8 = new Button();
            e_7.Children.Add(e_8);
            e_8.Name = "e_8";
            e_8.Margin = new Thickness(5F, 5F, 5F, 5F);
            e_8.Content = "Load";
            Grid.SetRow(e_8, 0);
            Binding binding_e_8_Command = new Binding("LoadBlockCommand");
            e_8.SetBinding(Button.CommandProperty, binding_e_8_Command);
            // e_9 element
            ScrollViewer e_9 = new ScrollViewer();
            e_6.Children.Add(e_9);
            e_9.Name = "e_9";
            Grid.SetRow(e_9, 1);
            // e_10 element
            ListBox e_10 = new ListBox();
            e_9.Content = e_10;
            e_10.Name = "e_10";
            Func<UIElement, UIElement> e_10_iptFunc = e_10_iptMethod;
            ControlTemplate e_10_ipt = new ControlTemplate(e_10_iptFunc);
            e_10.ItemsPanel = e_10_ipt;
            Func<UIElement, UIElement> e_10_dtFunc = e_10_dtMethod;
            e_10.ItemTemplate = new DataTemplate(e_10_dtFunc);
            Binding binding_e_10_ItemsSource = new Binding("Blocks");
            e_10.SetBinding(ListBox.ItemsSourceProperty, binding_e_10_ItemsSource);
            Binding binding_e_10_SelectedItem = new Binding("SelectedBlock");
            binding_e_10_SelectedItem.Mode = BindingMode.OneWayToSource;
            e_10.SetBinding(ListBox.SelectedItemProperty, binding_e_10_SelectedItem);
            items.Add(e_5);
            // e_13 element
            TabItem e_13 = new TabItem();
            e_13.Name = "e_13";
            e_13.Margin = new Thickness(2F, 2F, 2F, 2F);
            e_13.Header = "Platform";
            // e_14 element
            Grid e_14 = new Grid();
            e_13.Content = e_14;
            e_14.Name = "e_14";
            RowDefinition row_e_14_0 = new RowDefinition();
            row_e_14_0.Height = new GridLength(1F, GridUnitType.Star);
            e_14.RowDefinitions.Add(row_e_14_0);
            RowDefinition row_e_14_1 = new RowDefinition();
            row_e_14_1.Height = new GridLength(5F, GridUnitType.Star);
            e_14.RowDefinitions.Add(row_e_14_1);
            // e_15 element
            Grid e_15 = new Grid();
            e_14.Children.Add(e_15);
            e_15.Name = "e_15";
            ColumnDefinition col_e_15_0 = new ColumnDefinition();
            e_15.ColumnDefinitions.Add(col_e_15_0);
            ColumnDefinition col_e_15_1 = new ColumnDefinition();
            e_15.ColumnDefinitions.Add(col_e_15_1);
            // e_16 element
            Button e_16 = new Button();
            e_15.Children.Add(e_16);
            e_16.Name = "e_16";
            e_16.Margin = new Thickness(5F, 5F, 5F, 5F);
            e_16.Content = "Load";
            Grid.SetRow(e_16, 0);
            Binding binding_e_16_Command = new Binding("LoadPlatformCommand");
            e_16.SetBinding(Button.CommandProperty, binding_e_16_Command);
            // e_17 element
            ScrollViewer e_17 = new ScrollViewer();
            e_14.Children.Add(e_17);
            e_17.Name = "e_17";
            Grid.SetRow(e_17, 1);
            // e_18 element
            ListBox e_18 = new ListBox();
            e_17.Content = e_18;
            e_18.Name = "e_18";
            Func<UIElement, UIElement> e_18_iptFunc = e_18_iptMethod;
            ControlTemplate e_18_ipt = new ControlTemplate(e_18_iptFunc);
            e_18.ItemsPanel = e_18_ipt;
            Func<UIElement, UIElement> e_18_dtFunc = e_18_dtMethod;
            e_18.ItemTemplate = new DataTemplate(e_18_dtFunc);
            Binding binding_e_18_ItemsSource = new Binding("Platforms");
            e_18.SetBinding(ListBox.ItemsSourceProperty, binding_e_18_ItemsSource);
            Binding binding_e_18_SelectedItem = new Binding("SelectedPlatform");
            binding_e_18_SelectedItem.Mode = BindingMode.OneWayToSource;
            e_18.SetBinding(ListBox.SelectedItemProperty, binding_e_18_SelectedItem);
            items.Add(e_13);
            return items;
        }
        
        private static UIElement e_10_iptMethod(UIElement parent) {
            // e_11 element
            WrapPanel e_11 = new WrapPanel();
            e_11.Parent = parent;
            e_11.Name = "e_11";
            e_11.Height = float.NaN;
            e_11.MaxWidth = 180F;
            e_11.IsItemsHost = true;
            return e_11;
        }
        
        private static UIElement e_10_dtMethod(UIElement parent) {
            // e_12 element
            Image e_12 = new Image();
            e_12.Parent = parent;
            e_12.Name = "e_12";
            e_12.MaxHeight = 50F;
            e_12.MinHeight = 50F;
            e_12.MaxWidth = 50F;
            e_12.MinWidth = 50F;
            e_12.Margin = new Thickness(5F, 5F, 5F, 5F);
            Binding binding_e_12_Source = new Binding("Tile");
            e_12.SetBinding(Image.SourceProperty, binding_e_12_Source);
            return e_12;
        }
        
        private static UIElement e_18_iptMethod(UIElement parent) {
            // e_19 element
            WrapPanel e_19 = new WrapPanel();
            e_19.Parent = parent;
            e_19.Name = "e_19";
            e_19.Height = float.NaN;
            e_19.MaxWidth = 180F;
            e_19.IsItemsHost = true;
            return e_19;
        }
        
        private static UIElement e_18_dtMethod(UIElement parent) {
            // e_20 element
            Image e_20 = new Image();
            e_20.Parent = parent;
            e_20.Name = "e_20";
            e_20.MaxHeight = 50F;
            e_20.MinHeight = 50F;
            e_20.MaxWidth = 50F;
            e_20.MinWidth = 50F;
            e_20.Margin = new Thickness(5F, 5F, 5F, 5F);
            Binding binding_e_20_Source = new Binding("Tile");
            e_20.SetBinding(Image.SourceProperty, binding_e_20_Source);
            return e_20;
        }
    }
}
