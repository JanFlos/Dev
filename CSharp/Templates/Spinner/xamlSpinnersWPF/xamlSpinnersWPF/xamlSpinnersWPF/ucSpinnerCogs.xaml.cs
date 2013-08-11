using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace xamlSpinnersWPF
{
    /// <summary>
    /// Interaction logic for ucSpinnerCogs.xaml
    /// </summary>
    public partial class ucSpinnerCogs : UserControl
    {
        public ucSpinnerCogs()
        {
            this.InitializeComponent();
        }



        public bool IsIndetimate
        {
            get { return (bool)GetValue(IsIndetimateProperty); }
            set
            {
                SetValue(IsIndetimateProperty, value);
                this.Visibility = value ? Visibility.Visible : Visibility.Hidden;

            }
        }

        // Using a DependencyProperty as the backing store for IsIndetimate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndetimateProperty =
            DependencyProperty.Register("IsIndetimate", typeof(bool), typeof(ucSpinnerCogs), 
             new FrameworkPropertyMetadata( true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));



    }
}