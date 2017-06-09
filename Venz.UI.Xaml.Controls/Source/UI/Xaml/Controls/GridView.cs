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
        private TaskCompletionSource<Boolean> LoadedStateAwaiter = new TaskCompletionSource<Boolean>();
        private Grid ControlTemplateGrid;

        public GridView()
        {
            ItemsSourceModificationListener.Changed += (sender, count) => OnItemsSourceChanged(count);
            RegisterPropertyChangedCallback(ItemsSourceProperty, (sender, property) => ItemsSourceModificationListener.ChangeCollection(ItemsSource));
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;

            GridView_EmptyListMessage();
        }

        protected virtual void OnLoaded(Object sender, RoutedEventArgs args)
        {
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
        }

        protected virtual void OnItemsSourceChanged(UInt32 newItemsCount)
        {
            GridView_OnItemsSourceChanged(newItemsCount);
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
