using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Venz.Images
{
    public static class PictureLoader
    {
        private static RequestProcessor PictureProcessor;
        
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(Picture), typeof(PictureLoader), new PropertyMetadata(null, OnSourceChanged));        
        public static Picture GetSource(DependencyObject obj) => (Picture)obj.GetValue(SourceProperty);
        public static void SetSource(DependencyObject obj, Picture value) => obj.SetValue(SourceProperty, value);

        public static readonly DependencyProperty PriorityProperty = DependencyProperty.RegisterAttached("Priority", typeof(Int32), typeof(PictureLoader), new PropertyMetadata(0));
        public static Int32 GetPriority(DependencyObject obj) => (Int32)obj.GetValue(PriorityProperty);
        public static void SetPriority(DependencyObject obj, Int32 value) => obj.SetValue(PriorityProperty, value);
        


        static PictureLoader()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                var uiThreadContext = TaskScheduler.FromCurrentSynchronizationContext();
                PictureProcessor = new RequestProcessor(uiThreadContext);
            }
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var priority = GetPriority(obj);
            if (obj is IImageUriRenderer uriRenderer)
            {
                uriRenderer.UriContent = null;
                uriRenderer.PictureRequestId = null;
            }
            if (obj is IImageStreamRenderer streamRenderer)
            {
                streamRenderer.StreamContent = null;
                streamRenderer.PictureRequestId = null;
            }

            if ((obj is IImageUriRenderer) && ((args.NewValue == null) || (args.NewValue is UriPicture)))
            {
                if (DesignMode.DesignModeEnabled)
                {
                    if (args.NewValue is DesignTimePicture)
                        ((IImageUriRenderer)obj).UriContent = new ImageUriContent(null, ((DesignTimePicture)args.NewValue).GetUri());
                }
                else
                {
                    Enqueue((IImageUriRenderer)obj, (UriPicture)args.NewValue, priority);
                }
            }
            else if ((obj is IImageStreamRenderer) && ((args.NewValue == null) || (args.NewValue is StreamPicture)))
            {
                if (!DesignMode.DesignModeEnabled)
                    Enqueue((IImageStreamRenderer)obj, (StreamPicture)args.NewValue, priority);
            }
            else
            {
                throw new ArgumentException();
            }
            Dequeue((Picture)args.OldValue);
        }

        private static void Enqueue(IImageStreamRenderer renderer, StreamPicture picture, Int32 priority)
        {
            if ((picture == null) || !picture.IsAvailable)
                return;

            if (!picture.UseAsyncPattern)
                renderer.StreamContent = new ImageStreamContent(picture.Size, picture.GetStream());
            else
                PictureProcessor.Enqueue(new StreamPictureRequest(renderer, picture), priority);
        }

        private static void Enqueue(IImageUriRenderer renderer, UriPicture picture, Int32 priority)
        {
            if ((picture == null) || !picture.IsAvailable)
                return;

            if (!picture.UseAsyncPattern)
                renderer.UriContent = new ImageUriContent(picture.Size, picture.GetUri());
            else
                PictureProcessor.Enqueue(new UriPictureRequest(renderer, picture), priority);
        }

        private static void Dequeue(Picture picture)
        {
            if (picture == null)
                return;

            PictureProcessor.Dequeue(picture);
        }

        public static void Remove(Object tag)
        {
            if (tag == null)
                return;

            PictureProcessor.Dequeue(tag);
        }
    }
}
