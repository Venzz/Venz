using System;
using Venz.UI.Animation;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Xaml.Controls
{
    public enum SplitViewDisplayMode { Overlay, Inline, CompactOverlay, CompactInline }

    [ContentProperty(Name = "Content")]
    public class SplitView: Control
    {
        private Grid PaneRoot;
        private Action BackKeyAction;
        private FrameworkElement LightDismissLayer;

        public static readonly DependencyProperty PaneProperty =
            DependencyProperty.Register("Pane", typeof(Object), typeof(SplitView), new PropertyMetadata(null));

        public Object Pane
        {
            get { return (Object)GetValue(PaneProperty); }
            set { SetValue(PaneProperty, value); }
        }

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(SplitViewDisplayMode), typeof(SplitView), new PropertyMetadata(SplitViewDisplayMode.Inline));

        public SplitViewDisplayMode DisplayMode
        {
            get { return (SplitViewDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(SplitView), new PropertyMetadata(null));

        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty IsPaneOpenProperty =
            DependencyProperty.Register("IsPaneOpen", typeof(Boolean), typeof(SplitView),
            new PropertyMetadata(false, (obj, args) => ((SplitView)obj).OnIsPaneOpenChanged((Boolean)args.NewValue)));

        public Boolean IsPaneOpen
        {
            get { return (Boolean)GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }



        public SplitView()
        {
            DefaultStyleKey = typeof(SplitView);
            BackKeyAction = () => IsPaneOpen = false;
            SizeChanged += (sender, args) => PaneRoot.Width = (args.NewSize.Width > 400) ? 400 : args.NewSize.Width;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PaneRoot = (Grid)GetTemplateChild(nameof(PaneRoot));
            LightDismissLayer = (FrameworkElement)GetTemplateChild(nameof(LightDismissLayer));
            LightDismissLayer.Tapped += (sender, args) => IsPaneOpen = false;

            PaneRoot.Background = Resources.ContainsKey("AppLowChromeBrush") ? (Brush)Resources["AppLowChromeBrush"] : (Brush)Resources["PhoneChromeBrush"];
            OnIsPaneOpenChanged(IsPaneOpen);
        }

        private void OnIsPaneOpenChanged(Boolean isPaneOpen)
        {
            if (PaneRoot == null)
                return;

            if (isPaneOpen)
            {
                LightDismissLayer.Visibility = Visibility.Visible;
                PaneRoot.Opacity = 1;
                PaneRoot.IsHitTestVisible = true;
                if (!DesignMode.DesignModeEnabled)
                    Page.Active.AddBackKeyAction(BackKeyAction);

                var storyboard = CreatePaneShowingTransition(PaneRoot);
                storyboard.Begin();
            }
            else
            {
                LightDismissLayer.Visibility = Visibility.Collapsed;
                if (!DesignMode.DesignModeEnabled)
                    Page.Active.RemoveBackKeyAction(BackKeyAction);

                var storyboard = CreatePaneHidingTransition(PaneRoot);
                storyboard.Completed += (sender, args) =>
                {
                    PaneRoot.Opacity = 0;
                    PaneRoot.IsHitTestVisible = false;
                };
                storyboard.Begin();
            }
        }

        private static Storyboard CreatePaneShowingTransition(FrameworkElement target)
        {
            var translateX = new TranslateX(target) { From = -target.ActualWidth, Duration = 500, EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 8 } };
            var storyboard = new Storyboard();
            storyboard.Children.Add(translateX.GetTimeline());
            return storyboard;
        }

        private static Storyboard CreatePaneHidingTransition(FrameworkElement target)
        {
            var translateX = new TranslateX(target) { To = -target.ActualWidth, Duration = 250, EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 8 } };
            var storyboard = new Storyboard();
            storyboard.Children.Add(translateX.GetTimeline());
            return storyboard;
        }
    }
}
