using System;
using System.Threading.Tasks;
using Venz.Extensions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Venz.UI.Xaml
{
    public class Application: Windows.UI.Xaml.Application
    {
        private Task InitializationTask;
        private PrelaunchStage Prelaunch;

        public static ApplicationDispatcher Dispatcher { get; private set; }



        public Application()
        {
            Suspending += OnSuspending;
            Resuming += OnResuming;
            UnhandledException += OnUnhandledException;
            InitializationTask = InitializeAsync();
        }

        //
        // Activation.
        //

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PrelaunchActivated)
                Prelaunch = PrelaunchStage.ApplicationPrelaunched;
            else if (Prelaunch == PrelaunchStage.ApplicationPrelaunched)
                Prelaunch = PrelaunchStage.ApplicationActivated;
            else
                Prelaunch = PrelaunchStage.None;

            if (args.IsNewInstance())
            {
                var frame = new Frame();
                Window.Current.Content = frame;
                Window.Current.Activate();
                Window.Current.CoreWindow.KeyUp += OnCoreWindowKeyUp;
                Dispatcher = new ApplicationDispatcher();
                await StartAsync(frame, args.Kind, args.PreviousExecutionState, Prelaunch);
                await InitializationTask;
                await OnManuallyActivatedAsync(frame, true, Prelaunch, args.Arguments);
                await OnStartedAsync();
            }
            else
            {
                Window.Current.Activate();
                await OnManuallyActivatedAsync((Frame)Window.Current.Content, false, Prelaunch, args.Arguments);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            Prelaunch = (Prelaunch == PrelaunchStage.ApplicationPrelaunched) ? PrelaunchStage.ApplicationActivated : PrelaunchStage.None;
            if (args.IsNewInstance())
            {
                var frame = new Frame();
                Window.Current.Content = frame;
                Window.Current.Activate();
                Window.Current.CoreWindow.KeyUp += OnCoreWindowKeyUp;
                Dispatcher = new ApplicationDispatcher();
                await StartAsync(frame, args.Kind, args.PreviousExecutionState, Prelaunch);
                await InitializationTask;
                if (args is FileActivatedEventArgs)
                    await OnFileActivatedAsync(frame, true, Prelaunch, (FileActivatedEventArgs)args);
                else if (args is ProtocolActivatedEventArgs)
                    await OnUriActivatedAsync(frame, true, Prelaunch, (ProtocolActivatedEventArgs)args);
                else if (args is VoiceCommandActivatedEventArgs)
                    await OnVoiceActivatedAsync(frame, true, Prelaunch, (VoiceCommandActivatedEventArgs)args);
                await OnStartedAsync();
            }
            else
            {
                Window.Current.Activate();
                if (args is FileActivatedEventArgs)
                    await OnFileActivatedAsync((Frame)Window.Current.Content, false, Prelaunch, (FileActivatedEventArgs)args);
                else if (args is ProtocolActivatedEventArgs)
                    await OnUriActivatedAsync((Frame)Window.Current.Content, false, Prelaunch, (ProtocolActivatedEventArgs)args);
                else if (args is VoiceCommandActivatedEventArgs)
                    await OnVoiceActivatedAsync((Frame)Window.Current.Content, false, Prelaunch, (VoiceCommandActivatedEventArgs)args);
            }
        }
        
        /// Gets called when application is activated with file. OnActivated isn't get called.
        protected override void OnFileActivated(FileActivatedEventArgs args) => OnActivated(args);

        //
        // Suspending / Resuming
        //

        private async void OnResuming(Object sender, Object args)
        {
            await OnResumedAsync().ConfigureAwait(false);
        }

        private async void OnSuspending(Object sender, SuspendingEventArgs args)
        {
            var deferral = args.SuspendingOperation.GetDeferral();
            await OnSuspendingAsync(args.SuspendingOperation.Deadline).ConfigureAwait(false);
            deferral.Complete();
        }

        //
        // Other Events
        //

        private async void OnUnhandledException(Object sender, UnhandledExceptionEventArgs args)
        {
            await OnUnhandledExceptionAsync(args).ConfigureAwait(false);
        }

        private void OnCoreWindowKeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.F12:
                    OnDiagnosticsRequested((Frame)Window.Current.Content);
                    break;
                default:
                    ((Window.Current.Content as Frame)?.Content as Page).HandleKeyPressed(args.VirtualKey);
                    break;
            }
        }

        //
        // Interface
        //

        protected virtual Task InitializeAsync() => Task.FromResult<Object>(null);

        protected virtual Task StartAsync(Frame frame, ActivationKind activationKind, ApplicationExecutionState previousExecutionState, PrelaunchStage prelaunchStage) => Task.FromResult<Object>(null);

        protected virtual Task OnManuallyActivatedAsync(Frame frame, Boolean newInstance, PrelaunchStage prelaunchStage, String args) => Task.FromResult<Object>(null);

        protected virtual Task OnFileActivatedAsync(Frame frame, Boolean newInstance, PrelaunchStage prelaunchStage, FileActivatedEventArgs args) => Task.FromResult<Object>(null);

        protected virtual Task OnUriActivatedAsync(Frame frame, Boolean newInstance, PrelaunchStage prelaunchStage, ProtocolActivatedEventArgs args) => Task.FromResult<Object>(null);
 
        protected virtual Task OnVoiceActivatedAsync(Frame frame, Boolean newInstance, PrelaunchStage prelaunchStage, VoiceCommandActivatedEventArgs args) => Task.FromResult<Object>(null);

        protected virtual Task OnStartedAsync() => Task.FromResult<Object>(null);

        protected virtual Task OnResumedAsync() => Task.FromResult<Object>(null);

        protected virtual Task OnSuspendingAsync(DateTimeOffset deadline) => Task.FromResult<Object>(null);

        protected virtual Task OnUnhandledExceptionAsync(UnhandledExceptionEventArgs args) => Task.FromResult<Object>(null);

        protected virtual void OnDiagnosticsRequested(Frame frame) { }



        protected enum PrelaunchStage { None, ApplicationPrelaunched, ApplicationActivated }
    }
}