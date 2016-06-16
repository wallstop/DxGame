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
        
        private DockPanel e_1;
        
        private ListBox e_2;
        
        private TextBlock e_4;
        
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
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            this.e_0.Margin = new Thickness(0F, 0F, 0F, 280F);
            RowDefinition row_e_0_0 = new RowDefinition();
            row_e_0_0.Height = new GridLength(20F, GridUnitType.Pixel);
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            this.e_0.RowDefinitions.Add(row_e_0_1);
            // e_1 element
            this.e_1 = new DockPanel();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            // e_2 element
            this.e_2 = new ListBox();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.ItemsSource = Get_e_2_Items();
            // e_4 element
            this.e_4 = new TextBlock();
            this.e_0.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            this.e_4.Margin = new Thickness(108F, 0F, 143F, 274F);
            this.e_4.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_4.VerticalAlignment = VerticalAlignment.Center;
            this.e_4.Text = "Wow";
            this.e_4.FontSize = 20F;
            this.e_4.FontStyle = FontStyle.Bold;
            Grid.SetRowSpan(this.e_4, 2);
            FontManager.Instance.AddFont("Segoe UI", 20F, FontStyle.Bold, "Segoe_UI_15_Bold");
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_e_2_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_3 element
            ListBoxItem e_3 = new ListBoxItem();
            e_3.Name = "e_3";
            items.Add(e_3);
            return items;
        }
    }
}
