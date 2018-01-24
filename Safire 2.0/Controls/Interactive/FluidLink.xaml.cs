using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Safire.Animation;
namespace Safire.Controls.Interactive
{
    /// <summary>
    ///     Interaction logic for FluidLink.xaml
    /// </summary>
    public partial class FluidLink : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor, basic
        /// </summary>
        #region Ctor

        public FluidLink()
        {
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Methods for property change notifications
        /// </summary>
        #region NotiyProp

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// Sets the caption of the link
        /// </summary>
        #region Caption

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(FluidLink),
                                        new PropertyMetadata(string.Empty, OnCaptionPropertyChanged));
        [Category("Common Properties")]
        public string Caption
        {
            get { return GetValue(CaptionProperty).ToString(); }
            set { SetValue(CaptionProperty, value); }
        }

        private static void OnCaptionPropertyChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs e)
        {
            var myUserControl = dependencyObject as FluidLink;
            myUserControl.OnPropertyChanged("Caption");
            myUserControl.OnCaptionPropertyChanged(e);
        }

        private void OnCaptionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlock.Text = Caption;
        }

        #endregion

        /// <summary>
        /// Sets the ToggleBackground of the link
        /// </summary>
        #region ToggledBackground

        public static readonly DependencyProperty ToggledBackgroundProperty =
            DependencyProperty.Register("ToggledBackground", typeof(Brush), typeof(FluidLink),
                                        new PropertyMetadata(null, OnToggledBackgroundPropertyChanged));
        [Category("Brushes")]
        public Brush ToggledBackground
        {
            get { return GetValue(ToggledBackgroundProperty) as Brush; }
            set { SetValue(ToggledBackgroundProperty, value); }
        }

        private static void OnToggledBackgroundPropertyChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs e)
        {
            var myUserControl = dependencyObject as FluidLink;
            myUserControl.OnPropertyChanged("ToggledBackground");
            
        }

   

        #endregion


        /// <summary>
        /// IsChecked for font/other focusing props
        /// </summary>
        #region IsChecked

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(FluidLink),
                                        new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        [Category("Common Properties")]
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject dependencyObject,
                                                       DependencyPropertyChangedEventArgs e)
        {
            var myUserControl = dependencyObject as FluidLink;
            myUserControl.OnPropertyChanged("IsChecked");
            myUserControl.OnIsCheckedPropertyChanged(e);
        }

        /// <summary>
        /// Makes the font bigger if checked.
        /// During checking, if toggle is ON, it will uncheck all other FluidLinks in the same parent.
        /// </summary>
        /// <param name="e"></param>
        private void OnIsCheckedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {

            if (IsChecked)
            {
                //Animate font, bigger.
                DoubleAnimation animation = new DoubleAnimation(FontSize + (FontSize / FontFactor), TimeSpan.FromSeconds(0.5));
                animation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
                textBlock.BeginAnimation(TextBlock.FontSizeProperty, animation);

                //Animate Background
                if (BackgoundCanToggle)
                {
                    var solidColorBrush = ToggledBackground as SolidColorBrush;
                    if (solidColorBrush != null) grid.ColorAnimation(solidColorBrush.Color);
                }

                //Uncheck all other links
                var x = VisualTreeHelper.GetParent(this);
                foreach (FluidLink fluidLink in ((Panel)x).Children)
                {
                    if (fluidLink != this) fluidLink.IsChecked = false;
                }
            }
            else
            {
                //if toggle is ON, links cannot self uncheck, it should be performed during 
                //the check of another link under the same parent
                if (!CanToggle) return; 
                //Animate font, smaller.
                DoubleAnimation animation = new DoubleAnimation(FontSize - (FontSize /  FontFactor), TimeSpan.FromSeconds(0.5));
                animation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn };
                textBlock.BeginAnimation(TextBlock.FontSizeProperty, animation);

                //Animate background
                if (BackgoundCanToggle)
                {
                    var solidColorBrush = Background as SolidColorBrush;
                    if (solidColorBrush != null) grid.ColorAnimation(solidColorBrush.Color);
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks if toggling is available
        /// </summary>
        #region CanToggle
        public static readonly DependencyProperty CanToggleProperty =
            DependencyProperty.Register("CanToggle", typeof(bool), typeof(FluidLink),
                                        new PropertyMetadata(false, OnCanTogglePropertyChanged));

        [Category("Common Properties")]
        public bool CanToggle
        {
            get { return (bool)GetValue(CanToggleProperty); }
            set { SetValue(CanToggleProperty, value); }
        }

        private static void OnCanTogglePropertyChanged(DependencyObject dependencyObject,
                                                       DependencyPropertyChangedEventArgs e)
        {
            var myUserControl = dependencyObject as FluidLink;
            myUserControl.OnPropertyChanged("CanToggle");
            myUserControl.OnIsCheckedPropertyChanged(e);
        }
 

        #endregion

        /// <summary>
        /// Checks if Background toggling is available
        /// </summary>
        #region BackgoundCanToggle
        public static readonly DependencyProperty BackgoundCanToggleProperty =
            DependencyProperty.Register("BackgoundCanToggle", typeof(bool), typeof(FluidLink),
                                        new PropertyMetadata(false, OnBackgoundCanTogglePropertyChanged));

        [Category("Common Properties")]
        public bool BackgoundCanToggle
        {
            get { return (bool)GetValue(BackgoundCanToggleProperty); }
            set { SetValue(BackgoundCanToggleProperty, value); }
        }

        private static void OnBackgoundCanTogglePropertyChanged(DependencyObject dependencyObject,
                                                       DependencyPropertyChangedEventArgs e)
        {
            var myUserControl = dependencyObject as FluidLink;
            myUserControl.OnPropertyChanged("BackgoundCanToggle");
            myUserControl.OnIsCheckedPropertyChanged(e);
        }


        #endregion

        /// <summary>
        /// Checks if Background toggling is available
        /// </summary>
        #region FontFactor
        public static readonly DependencyProperty FontFactorProperty =
            DependencyProperty.Register("FontFactor", typeof(float), typeof(FluidLink),
                                        new PropertyMetadata(3.0f, OnFontFactorPropertyChanged));

        [Category("Common Properties")]
        public float FontFactor
        {
            get { return (float)GetValue(FontFactorProperty); }
            set { SetValue(FontFactorProperty, value); }
        }

        private static void OnFontFactorPropertyChanged(DependencyObject dependencyObject,
                                                       DependencyPropertyChangedEventArgs e)
        {
            var myUserControl = dependencyObject as FluidLink;
            myUserControl.OnPropertyChanged("FontFactor");
            myUserControl.OnIsCheckedPropertyChanged(e);
        }


        #endregion

        /// <summary>
        /// Click event for the link
        /// </summary>
        #region ClickRoutedEvent

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (FluidLink));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// Raise Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Listener_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
            if (CanToggle) IsChecked = true;

        }

        #endregion

    }
}