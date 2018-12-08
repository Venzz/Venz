using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Venz.UI.Xaml
{
    public partial class Page
    {
        private UInt32 Id;
        private Boolean StateSet;

        protected FrameNavigation Navigation { get; private set; }



        private void Page_Navigation() { }

        private void Navigation_OnNavigatedTo(NavigationEventArgs args)
        {
            Window.Current.VisibilityChanged += OnWindowVisibilityChanged;
            if (!StateSet)
            {
                StateSet = true;
                if (args.Parameter is UInt32)
                {
                    Navigation = ((Frame)Frame).Navigation;
                    Id = Convert.ToUInt32(args.Parameter);
                    var navigationEntry = Navigation.GetEntry(Id);
                    SetState((FrameNavigation.Parameter)navigationEntry.Parameter.Get("navigation"), (FrameNavigation.Parameter)navigationEntry.Parameter.TryGet("state"));
                }
                else
                {
                    SetState(null, null);
                }
            }
        }

        private void Navigation_OnNavigatedFrom(NavigationEventArgs args)
        {
            Window.Current.VisibilityChanged -= OnWindowVisibilityChanged;
            if (Id > 0)
            {
                if (args.NavigationMode == NavigationMode.Back)
                {
                    Navigation.RemoveEntry(Id);
                }
                else
                {
                    var navigationEntry = Navigation.GetEntry(Id);
                    var stateParameter = GetState();
                    navigationEntry.Parameter = new FrameNavigation.Parameter("navigation", (FrameNavigation.Parameter)navigationEntry.Parameter.Get("navigation")).Add("state", stateParameter);
                }
            }

            switch (args.NavigationMode)
            {
                case NavigationMode.New:
                case NavigationMode.Forward:
                    if ((this is IBackStackResettablePage) || Navigation.IsBackStackResetInitiated)
                    {
                        Navigation.IsBackStackResetInitiated = false;
                        Frame.BackStack.Clear();
                        Navigation.ResetBasedOn(Id);
                    }
                    else if ((this is IBackStackRemovablePage) && (Frame.BackStack.Count > 0))
                    {
                        Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
                        Navigation.RemoveEntry(Id);
                    }
                    break;
            }
        }

        protected virtual void SetState(FrameNavigation.Parameter navigationParameter, FrameNavigation.Parameter stateParameter) { }

        protected virtual FrameNavigation.Parameter GetState() => new FrameNavigation.Parameter();

        protected virtual void OnActivating() { }

        protected virtual void OnDiactivating() { }

        private void OnWindowVisibilityChanged(Object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (!args.Visible)
                OnDiactivating();
            else
                OnActivating();
        }
    }
}
