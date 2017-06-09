using System;
using Venz.Extensions;
using Venz.Images;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Venz.UI.Xaml.Controls
{
    public sealed partial class Image: UserControl, IImageUriRenderer, IImageStreamRenderer
    {
        private ImageStreamContent _StreamContent;
        private ImageUriContent _UriContent;
        private Boolean IsImageSet;
        private Size LastAppliedSize;

        public static readonly DependencyProperty DecodePixelWidthOverrideProperty =
            DependencyProperty.Register("DecodePixelWidthOverride", typeof(Int32), typeof(Image), new PropertyMetadata(0));

        public Int32 DecodePixelWidthOverride
        {
            get => (Int32)GetValue(DecodePixelWidthOverrideProperty);
            set => SetValue(DecodePixelWidthOverrideProperty, value);
        }

        public static readonly DependencyProperty DecodePixelHeightOverrideProperty =
            DependencyProperty.Register("DecodePixelHeightOverride", typeof(Int32), typeof(Image), new PropertyMetadata(0));

        public Int32 DecodePixelHeightOverride
        {
            get => (Int32)GetValue(DecodePixelHeightOverrideProperty);
            set => SetValue(DecodePixelHeightOverrideProperty, value);
        }

        public static readonly DependencyProperty DisablePixelDecodingProperty =
            DependencyProperty.Register("DisablePixelDecoding", typeof(Boolean), typeof(Image), new PropertyMetadata(false));
            
        public Boolean DisablePixelDecoding
        {
            get => (Boolean)GetValue(DisablePixelDecodingProperty);
            set => SetValue(DisablePixelDecodingProperty, value);
        }

        public Object PictureRequestId { get; set; }
        public ImageStreamContent StreamContent { private get { return _StreamContent; } set { _StreamContent = value; OnImageContentChanged(value); } }
        public ImageUriContent UriContent { private get { return _UriContent; } set { _UriContent = value; OnImageContentChanged(value); } }



        public Image()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(BackgroundProperty, (sender, property) => OnBackgroundChanged(Background));
            if (!DesignMode.DesignModeEnabled)
                SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(Object sender, SizeChangedEventArgs args)
        {
            if (!LastAppliedSize.IsVisible() || (Math.Abs(LastAppliedSize.Width - args.NewSize.Width) > 50) || (Math.Abs(LastAppliedSize.Height - args.NewSize.Height) > 50))
            {
                TryApplyImageContent();
                LastAppliedSize = args.NewSize;
            }
        }

        private void OnBackgroundChanged(Brush value) => LayoutControl.Background = IsImageSet ? null : value;

        private void OnImageContentChanged(ImageStreamContent streamContent)
        {
            _UriContent = null;
            IsImageSet = streamContent != null;
            if (streamContent == null)
                ImageControl.Source = null;
            TryApplyImageContent();
            OnBackgroundChanged(Background);
        }

        private void OnImageContentChanged(ImageUriContent uriContent)
        {
            _StreamContent = null;
            IsImageSet = uriContent != null;
            if (uriContent == null)
                ImageControl.Source = null;
            TryApplyImageContent();
            OnBackgroundChanged(Background);
        }

        private void TryApplyImageContent()
        {
            if ((ActualWidth == 0) || (ActualHeight == 0))
                return;

            if (UriContent != null)
            {
                var bitmapImage = new BitmapImage() { DecodePixelType = DecodePixelType.Logical };
                if (!DisablePixelDecoding && ((DecodePixelWidthOverride > 0) || (DecodePixelHeightOverride > 0)))
                {
                    if (DecodePixelWidthOverride > 0)
                        bitmapImage.DecodePixelType = DecodePixelType.Logical;
                        bitmapImage.DecodePixelWidth = DecodePixelWidthOverride;
                    if (DecodePixelHeightOverride > 0)
                        bitmapImage.DecodePixelHeight = DecodePixelHeightOverride;
                }
                else if (!DisablePixelDecoding && UriContent.Size.HasValue)
                {
                    if (UriContent.Size.Value.IsPortrait())
                        bitmapImage.DecodePixelHeight = (Int32)ActualHeight;
                    else
                        bitmapImage.DecodePixelWidth = (Int32)ActualWidth;
                }
                else if (!DisablePixelDecoding)
                {
                    if (ActualWidth > ActualHeight)
                        bitmapImage.DecodePixelWidth = (Int32)ActualWidth;
                    else
                        bitmapImage.DecodePixelHeight = (Int32)ActualHeight;
                }
                bitmapImage.UriSource = UriContent.Uri;
                ImageControl.Source = bitmapImage;
            }
            else if (StreamContent != null)
            {
                var bitmapImage = new BitmapImage() { DecodePixelType = DecodePixelType.Logical };
                if (!DisablePixelDecoding && ((DecodePixelWidthOverride > 0) || (DecodePixelHeightOverride > 0)))
                {
                    if (DecodePixelWidthOverride > 0)
                        bitmapImage.DecodePixelWidth = DecodePixelWidthOverride;
                    if (DecodePixelHeightOverride > 0)
                        bitmapImage.DecodePixelHeight = DecodePixelHeightOverride;
                }
                else if (!DisablePixelDecoding && StreamContent.Size.HasValue)
                {
                    if (StreamContent.Size.Value.IsPortrait())
                        bitmapImage.DecodePixelHeight = (Int32)ActualHeight;
                    else
                        bitmapImage.DecodePixelWidth = (Int32)ActualWidth;
                }
                else if (!DisablePixelDecoding)
                {
                    if (ActualWidth > ActualHeight)
                        bitmapImage.DecodePixelWidth = (Int32)ActualWidth;
                    else
                        bitmapImage.DecodePixelHeight = (Int32)ActualHeight;
                }
                StreamContent.Stream.Seek(0);
                bitmapImage.SetSource(StreamContent.Stream);
                ImageControl.Source = bitmapImage;
            }
        }
    }
}
