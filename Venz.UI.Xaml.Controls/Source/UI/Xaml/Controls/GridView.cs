using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venz.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public partial class GridView: global::Windows.UI.Xaml.Controls.GridView
    {
        private ItemsSourceModificationListener ItemsSourceModificationListener = new ItemsSourceModificationListener();
        private Grid ControlTemplateGrid;
        private ContentControl HeaderContentControl;

        protected TaskCompletionSource<Boolean> LoadedStateAwaiter = new TaskCompletionSource<Boolean>();

        public GridView()
        {
            ItemsSourceModificationListener.Changed += (sender, args) => OnItemsSourceChanged(args);
            RegisterPropertyChangedCallback(ItemsSourceProperty, (sender, property) => ItemsSourceModificationListener.ChangeCollection(ItemsSource));
            RegisterPropertyChangedCallback(HeaderProperty, (sender, property) => OnHeaderChanged(Header));
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;

            GridView_EmptyListMessage();
        }

        protected virtual void OnLoaded(Object sender, RoutedEventArgs args)
        {
            if (!LoadedStateAwaiter.Task.IsCompleted)
                LoadedStateAwaiter.SetResult(true);
        }

        protected virtual void OnSizeChanged(Object sender, SizeChangedEventArgs args)
        {
            if (args.NewSize.Width == 0)
                return;

            if (ControlTemplateGrid == null)
                ControlTemplateGrid = this.TryGetVisualTreeChildAt<Grid>(4);
            if (HeaderContentControl == null)
                HeaderContentControl = this.TryGetVisualTreeChildAt<ContentControl>(7);

            if (ControlTemplateGrid != null)
                OnTemplateControlsAvailable(ControlTemplateGrid);

            GridView_OnSizeChanged(args);
        }

        protected virtual void OnItemsSourceChanged(ItemsSourceModificationListener.Args args)
        {
            if (args.CountChanged)
                GridView_OnItemsSourceChanged(args.NewCount);
        }

        private void OnHeaderChanged(Object newHeader)
        {
            GridView_OnHeaderChanged(newHeader);
        }

        protected virtual void OnTemplateControlsAvailable(Grid controlTemplateGrid)
        {
            GridView_OnControlTemplateControlsAvailable(controlTemplateGrid);
        }



        public IReadOnlyCollection<GridViewItem> TryGetItemContainers()
        {
            if (ItemsPanelRoot == null)
                return null;
            return ItemsPanelRoot.Children.Cast<GridViewItem>().ToList();
        }
    }
}
