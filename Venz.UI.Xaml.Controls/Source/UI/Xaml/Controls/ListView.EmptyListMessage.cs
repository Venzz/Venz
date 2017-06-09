using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Venz.UI.Xaml.Controls
{
    public partial class ListView
    {
        private Lazy<TextBlock> EmptyListMessageTextBlock;

        public static readonly DependencyProperty EmptyListMessageProperty =
            DependencyProperty.Register("EmptyListMessage", typeof(String), typeof(ListView), new PropertyMetadata(""));

        public String EmptyListMessage
        {
            get => (String)GetValue(EmptyListMessageProperty);
            set => SetValue(EmptyListMessageProperty, value);
        }



        private void ListView_EmptyListMessage()
        {
            EmptyListMessageTextBlock = new Lazy<TextBlock>(() =>
            {
                var textBlock = new TextBlock() { MinHeight = 200, Foreground = (Brush)Resources["SystemControlForegroundBaseMediumBrush"] };
                textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = this, Path = new PropertyPath(nameof(EmptyListMessage)), Mode = BindingMode.OneWay });
                return textBlock;
            });
        }

        private void ListView_OnControlTemplateControlsAvailable(Grid controlTemplateGrid)
        {
            UpdateEmptyListMessageTextBlock(controlTemplateGrid, ItemsSourceModificationListener.Count);
        }

        private void ListView_OnItemsSourceChanged(UInt32 newItemsCount)
        {
            UpdateEmptyListMessageTextBlock(ControlTemplateGrid, newItemsCount);
        }

        private void UpdateEmptyListMessageTextBlock(Grid controlTemplateGrid, UInt32 itemsCount)
        {
            if (itemsCount == 0)
            {
                ControlTemplateGrid?.Children.Add(EmptyListMessageTextBlock.Value);
            }
            else if ((itemsCount > 0) && EmptyListMessageTextBlock.IsValueCreated)
            {
                ControlTemplateGrid?.Children.Remove(EmptyListMessageTextBlock.Value);
            }
        }
    }
}
