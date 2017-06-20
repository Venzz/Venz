using System;
using Windows.UI.Xaml;

namespace Venz.UI.Xaml
{
    public partial class Page
    {
        private PageContent PageContent;

        public void Embed(FrameworkElement content)
        {
            var customContentContainer = GetCustomContentContainer();
            if (customContentContainer != null)
                customContentContainer.Children.Add(content);
        }

        public void Remove(FrameworkElement content)
        {
            var customContentContainer = GetCustomContentContainer();
            if (customContentContainer != null)
                customContentContainer.Children.Remove(content);
        }

        private Windows.UI.Xaml.Controls.Grid GetCustomContentContainer()
        {
            if (PageContent == null)
                PageContent = Content as PageContent;

            if (PageContent == null)
                throw new InvalidOperationException("Page content isn't represented by an instance of PageContent class.");

            var customContentContainer = PageContent.GetCustomContentLayer();
            if (customContentContainer == null)
                throw new InvalidOperationException("PageContent doesn't have custom content container control.");

            return customContentContainer;
        }
    }
}
