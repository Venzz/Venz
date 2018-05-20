using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venz.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public partial class ListView: global::Windows.UI.Xaml.Controls.ListView
    {
        private ItemsSourceModificationListener ItemsSourceModificationListener = new ItemsSourceModificationListener();
        private TaskCompletionSource<Boolean> LoadedStateAwaiter = new TaskCompletionSource<Boolean>();
        private Grid ControlTemplateGrid;
        private ContentControl HeaderContentControl;
        private ScrollViewer ScrollViewerControl;

        public ListView()
        {
            ItemsSourceModificationListener.Changed += (sender, args) => OnItemsSourceChanged(args);
            RegisterPropertyChangedCallback(ItemsSourceProperty, (sender, property) => ItemsSourceModificationListener.ChangeCollection(ItemsSource));
            RegisterPropertyChangedCallback(HeaderProperty, (sender, property) => OnHeaderChanged(Header));
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;

            ListView_EmptyListMessage();
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
            {
                ControlTemplateGrid = this.TryGetVisualTreeChildAt<Grid>(4);
                if (ControlTemplateGrid != null)
                    OnTemplateControlsAvailable(ControlTemplateGrid);
            }
            if (HeaderContentControl == null)
            {
                HeaderContentControl = this.TryGetVisualTreeChildAt<ContentControl>(7);
            }
            if (ScrollViewerControl == null)
            {
                ScrollViewerControl = this.TryGetVisualTreeChildAt<ScrollViewer>(2);
                ScrollViewerControl.ViewChanged += (s, a) => OnViewChanged(ScrollViewerControl, a);
            }
            
            ListView_OnSizeChanged(args);
        }

        protected virtual void OnViewChanged(ScrollViewer sender, ScrollViewerViewChangedEventArgs args)
        {
        }

        protected virtual void OnItemsSourceChanged(ItemsSourceModificationListener.Args args)
        {
            if (args.CountChanged)
                ListView_OnItemsSourceChanged(args.NewCount);
        }

        private void OnHeaderChanged(Object newHeader)
        {
            ListView_OnHeaderChanged(newHeader);
        }

        protected virtual void OnTemplateControlsAvailable(Grid controlTemplateGrid)
        {
            ListView_OnControlTemplateControlsAvailable(controlTemplateGrid);
        }



        public IReadOnlyCollection<ListViewItem> TryGetItemContainers()
        {
            if (ItemsPanelRoot == null)
                return null;
            return ItemsPanelRoot.Children.Cast<ListViewItem>().ToList();
        }
    }
}
