using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Venz.UI.Xaml.Controls
{
    public sealed partial class MasterDetailsContent: UserControl
    {
        private Boolean IsStateDetermined;
        private Action DetailsClosureAction;
        private ContentState State;
        private IMasterContent Master;
        private IDetailsContent Details;

        public static readonly DependencyProperty MasterContentProperty =
            DependencyProperty.Register("MasterContent", typeof(IMasterContent), typeof(MasterDetailsContent),
            new PropertyMetadata(null, (obj, args) => ((MasterDetailsContent)obj).OnMasterChanged((IMasterContent)args.OldValue, (IMasterContent)args.NewValue)));

        public IMasterContent MasterContent
        {
            get => (IMasterContent)GetValue(MasterContentProperty);
            set => SetValue(MasterContentProperty, value);
        }

        public static readonly DependencyProperty MasterBackgroundProperty =
            DependencyProperty.Register("MasterBackground", typeof(Brush), typeof(MasterDetailsContent), new PropertyMetadata(null));

        public Brush MasterBackground
        {
            get => (Brush)GetValue(MasterBackgroundProperty);
            set => SetValue(MasterBackgroundProperty, value);
        }

        public event TypedEventHandler<MasterDetailsContent, Action> BackKeyAvailable = delegate { };
        public event TypedEventHandler<MasterDetailsContent, Action> BackKeyUnavailable = delegate { };
        public event EventHandler StateDetermined = delegate { };



        public MasterDetailsContent()
        {
            InitializeComponent();
            State = ContentState.Stacked;
            DetailsClosureAction = () =>
            {
                Details.Dispose();
                Details.ClosureRequested -= OnDetailsClosureRequested;
                Details = null;
                DetailsPresenter.Content = null;
                MasterPresenter.Visibility = Visibility.Visible;
                Master.OnDetailsClosed();
            };
        }

        public Task InitializeAsync(FrameNavigation.Parameter navigationParameter, FrameNavigation.Parameter stateParameter)
        {
            Master = (IMasterContent)MasterPresenter.Content;

            var tasks = new List<Task>() { MasterContent.InitializeAsync(navigationParameter, (FrameNavigation.Parameter)stateParameter?.TryGet("master"), State) };
            if (stateParameter?.Contains("details") == true)
            {
                Details = MasterContent.GetDetails();
                Details.ClosureRequested += OnDetailsClosureRequested;
                DetailsPresenter.Content = Details;
                if (State == ContentState.Stacked)
                {
                    MasterPresenter.Visibility = Visibility.Collapsed;
                    BackKeyAvailable(this, DetailsClosureAction);
                }
                tasks.Add(Details.InitializeAsync((FrameNavigation.Parameter)stateParameter.TryGet("details"), State));
            }
            return Task.WhenAll(tasks);
        }

        public FrameNavigation.Parameter GetNavigationParameter()
        {
            var parameter = new FrameNavigation.Parameter();
            var masterParameter = Master.GetParameter();
            if (masterParameter is FrameNavigation.Parameter)
                parameter.Add("master", (FrameNavigation.Parameter)masterParameter);
            var detailsParameter = Details?.GetParameter();
            if (detailsParameter is FrameNavigation.Parameter)
                parameter.Add("details", (FrameNavigation.Parameter)detailsParameter);
            return parameter;
        }

        private void OnSizeChanged(Object sender, SizeChangedEventArgs args)
        {
            if ((args.NewSize.Width < 720) && (State == ContentState.SideBySide))
            {
                State = ContentState.Stacked;
                Master?.OnContentStateChanged(State);
                Details?.OnContentStateChanged(State);

                Grid.SetColumn(DetailsPresenter, 0);
                if (MasterBackgroundControl != null)
                    MasterBackgroundControl.Visibility = Visibility.Collapsed;

                if (Details == null)
                {
                    if (MasterPresenter.Content == null)
                        MasterPresenter.Content = MasterContent;
                }
                else if (MasterPresenter.Visibility == Visibility.Visible)
                {
                    MasterPresenter.Visibility = Visibility.Collapsed;
                    BackKeyAvailable(this, DetailsClosureAction);
                }
            }
            else if ((args.NewSize.Width >= 720) && (State == ContentState.Stacked))
            {
                State = ContentState.SideBySide;
                Master?.OnContentStateChanged(State);
                Details?.OnContentStateChanged(State);

                Grid.SetColumn(DetailsPresenter, 1);
                if (MasterBackgroundControl == null)
                {
                    var masterBackgroundControl = (FrameworkElement)FindName(nameof(MasterBackgroundControl));
                    masterBackgroundControl.SetBinding(Shape.FillProperty, new Binding() { Source = this, Path = new PropertyPath(nameof(MasterBackground)) });
                }
                MasterBackgroundControl.Visibility = Visibility.Visible;

                MasterPresenter.Visibility = Visibility.Visible;
                BackKeyUnavailable(this, DetailsClosureAction);
            }

            if (State == ContentState.Stacked)
            {
                MasterPresenter.Width = args.NewSize.Width;
                DetailsPresenter.Width = args.NewSize.Width;
            }
            else
            {
                if (args.NewSize.Width / 3 < 360)
                {
                    MasterPresenter.Width = 360;
                    DetailsPresenter.Width = args.NewSize.Width - 360;
                }
                else
                {
                    MasterPresenter.Width = args.NewSize.Width / 3;
                    DetailsPresenter.Width = args.NewSize.Width * 2 / 3;
                }
            }

            if (!IsStateDetermined)
            {
                IsStateDetermined = true;
                StateDetermined(this, EventArgs.Empty);
            }
        }

        private void OnMasterChanged(IMasterContent oldValue, IMasterContent newValue)
        {
            if (oldValue != null)
                oldValue.DetailsRequested -= OnDetailsRequested;
            MasterPresenter.Content = newValue;
            if (newValue != null)
                newValue.DetailsRequested += OnDetailsRequested;
        }

        private async void OnDetailsRequested(IMasterContent sender, FrameNavigation.Parameter parameter)
        {
            if (Details != null)
            {
                Details.Dispose();
                Details.ClosureRequested -= OnDetailsClosureRequested;
            }

            Details = sender.GetDetails();
            Details.ClosureRequested += OnDetailsClosureRequested;
            DetailsPresenter.Content = Details;

            if (RenderSize.Width < 720)
            {
                MasterPresenter.Visibility = Visibility.Collapsed;
                BackKeyAvailable?.Invoke(this, DetailsClosureAction);
            }
            await Details.InitializeAsync(parameter, State);
        }

        private void OnDetailsClosureRequested(Object sender, EventArgs args)
        {
            BackKeyUnavailable?.Invoke(this, DetailsClosureAction);
            DetailsClosureAction.Invoke();
        }

        public void Dispose()
        {
            Master.Dispose();
            Details?.Dispose();
        }

        public interface IMasterContent
        {
            event TypedEventHandler<IMasterContent, FrameNavigation.Parameter> DetailsRequested;

            IDetailsContent GetDetails();
            Object GetParameter();
            Task InitializeAsync(FrameNavigation.Parameter navigationParameter, FrameNavigation.Parameter stateParameter, ContentState state);
            void OnContentStateChanged(ContentState state);
            void Dispose();
            void OnDetailsClosed();
        }

        public interface IDetailsContent
        {
            event EventHandler ClosureRequested;

            Object GetParameter();
            Task InitializeAsync(FrameNavigation.Parameter stateParameter, ContentState state);
            void OnContentStateChanged(ContentState state);
            void Dispose();
        }

        public enum ContentState { Stacked, SideBySide }
    }
}
