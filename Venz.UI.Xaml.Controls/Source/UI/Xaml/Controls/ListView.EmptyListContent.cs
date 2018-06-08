using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public partial class ListView
    {
        private Lazy<ContentPresenter> EmptyListContentPresenter;
        private UIElement HeaderTemplateRoot;

        public static readonly DependencyProperty EmptyListContentTemplateProperty =
            DependencyProperty.Register("EmptyListContentTemplate", typeof(DataTemplate), typeof(ListView), new PropertyMetadata(null));

        public DataTemplate EmptyListContentTemplate
        {
            get { return (DataTemplate)GetValue(EmptyListContentTemplateProperty); }
            set { SetValue(EmptyListContentTemplateProperty, value); }
        }



        private void ListView_EmptyListMessage()
        {
            EmptyListContentPresenter = new Lazy<ContentPresenter>(() => new ContentPresenter() { ContentTemplate = EmptyListContentTemplate });
        }

        private void ListView_OnSizeChanged(SizeChangedEventArgs args)
        {
            if (HeaderTemplateRoot != null)
                HeaderTemplateRoot = HeaderContentControl?.ContentTemplateRoot;

            if (HeaderTemplateRoot != null)
                UpdateEmptyListMessageTextBlock(ControlTemplateGrid, Header, ItemsSourceModificationListener.Count);
        }

        private void ListView_OnControlTemplateControlsAvailable(Grid controlTemplateGrid)
        {
            UpdateEmptyListMessageTextBlock(controlTemplateGrid, Header, ItemsSourceModificationListener.Count);
        }

        private void ListView_OnItemsSourceChanged(UInt32 newItemsCount)
        {
            UpdateEmptyListMessageTextBlock(ControlTemplateGrid, Header, newItemsCount);
        }

        private void ListView_OnHeaderChanged(Object newHeader)
        {
            UpdateEmptyListMessageTextBlock(ControlTemplateGrid, newHeader, ItemsSourceModificationListener.Count);
        }

        protected virtual void OnHeaderVisibilityChanged(Visibility visibility)
        {
            // By mysterious reason, when there is something in the header, setting Visibility to Collapsed doesn't have any effect, it automatically gets returned back to Visible.
            if (HeaderContentControl != null)
                HeaderContentControl.Visibility = Visibility.Collapsed;
        }

        private void UpdateEmptyListMessageTextBlock(Grid controlTemplateGrid, Object header, UInt32 itemsCount)
        {
            if (EmptyListContentTemplate == null)
                return;

            if ((itemsCount == 0) && ((header == null) || (header == DataContext)))
            {
                OnHeaderVisibilityChanged(Visibility.Collapsed);
                if (controlTemplateGrid == null)
                    return;
                if ((EmptyListContentTemplate != null) && (!EmptyListContentPresenter.IsValueCreated || !controlTemplateGrid.Children.Contains(EmptyListContentPresenter.Value)))
                    controlTemplateGrid.Children.Add(EmptyListContentPresenter.Value);
            }
            else if (((itemsCount > 0) || (header != null) && (header != DataContext)) && EmptyListContentPresenter.IsValueCreated)
            {
                OnHeaderVisibilityChanged(Visibility.Visible);
                controlTemplateGrid?.Children.Remove(EmptyListContentPresenter.Value);
            }
        }
    }
}
