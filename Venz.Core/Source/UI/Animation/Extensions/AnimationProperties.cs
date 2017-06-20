using Windows.UI.Xaml;

namespace Venz.UI.Animation
{
    public class Opacity: DoubleBasedAnimation
    {
        public Opacity(FrameworkElement target): base(target, "Opacity") { }
    }

    public class TranslateX: DoubleBasedAnimation
    {
        public TranslateX(FrameworkElement target): base(target, "(FrameworkElement.RenderTransform).(CompositeTransform.TranslateX)") { }
    }

    public class TranslateY: DoubleBasedAnimation
    {
        public TranslateY(FrameworkElement target): base(target, "(FrameworkElement.RenderTransform).(CompositeTransform.TranslateY)") { }
    }

    public class ScaleX: DoubleBasedAnimation
    {
        public ScaleX(FrameworkElement target): base(target, "(FrameworkElement.RenderTransform).(CompositeTransform.ScaleX)") { }
    }

    public class ScaleY: DoubleBasedAnimation
    {
        public ScaleY(FrameworkElement target): base(target, "(FrameworkElement.RenderTransform).(CompositeTransform.ScaleY)") { }
    }

    public class TranslateClipY: DoubleBasedAnimation
    {
        public TranslateClipY(FrameworkElement target): base(target.Clip, "(Geometry.Transform).(CompositeTransform.TranslateY)") { }
    }

    public class Rotation: DoubleBasedAnimation
    {
        public Rotation(FrameworkElement target): base(target, "(FrameworkElement.RenderTransform).(CompositeTransform.Rotation)") { }
    }
}
