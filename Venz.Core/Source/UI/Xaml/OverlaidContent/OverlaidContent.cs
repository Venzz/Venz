using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Xaml
{
    public class OverlaidContent: ContentControl
    {
        private Page Page;
        private Action BackKeyAction;
        private TaskCompletionSource<Boolean> ClosingAwaiter;
        private Storyboard ShowingTransition;

        public IOverlaidContentTransition Transition { get; set; }
        public AppBar BottomAppBar { get; set; }



        public OverlaidContent()
        {
            DefaultStyleKey = typeof(OverlaidContent);
            BackKeyAction = new Action(() => Close(skipAnimation: false));
            ClosingAwaiter = new TaskCompletionSource<Boolean>();
            Loaded += OnLoaded;
        }

        public async Task ShowAsync()
        {
            Page = Page.Active;
            #if DEBUG
            if (Page == null)
                throw new InvalidOperationException("Unable to show OverlaidContent. There is no active page.");
            #else
            if (Page == null)
                return;
            #endif
            
            Window.Current.Activated += OnWindowActivationStateChanged;
            Page.Embed(this);
            Page.AddBackKeyAction(BackKeyAction);
            Page.OverrideSettings(new Page.Settings() { BottomAppBar = BottomAppBar });

            await ClosingAwaiter.Task;
        }

        protected virtual void OnDisplayed()
        {
        }

        protected virtual void OnClosing()
        {
        }

        protected virtual void OnClosed()
        {
        }

        protected void Close(Boolean skipAnimation)
        {
            if (ShowingTransition != null)
                ShowingTransition.Stop();
            Window.Current.Activated -= OnWindowActivationStateChanged;
            Page.RemoveBackKeyAction(BackKeyAction);

            Action finishClosing = () =>
            {
                OnClosed();
                Page.Remove(this);
                ClosingAwaiter.SetResult(true);
            };

            OnClosing();
            Page.RestoreSettings();
            if (skipAnimation || (Transition == null))
            {
                finishClosing();
            }
            else
            {
                var storyboard = Transition.CreateHiding(this);
                if (storyboard != null)
                {
                    storyboard.Completed += (s, e) => finishClosing();
                    storyboard.Begin();
                }
                else
                {
                    finishClosing();
                }
            }
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            ShowingTransition = (Transition != null) ? Transition.CreateShowing(this) : null;
            if (ShowingTransition != null)
            {
                ShowingTransition.Completed += (s, args) => OnDisplayed();
                ShowingTransition.Begin();
            }
            else
            {
                OnDisplayed();
            }
        }

        private void OnWindowActivationStateChanged(Object sender, Windows.UI.Core.WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                Close(skipAnimation: true);
        }
    }
}
