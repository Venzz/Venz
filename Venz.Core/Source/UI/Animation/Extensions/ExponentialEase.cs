using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Animation
{
    public static class EaseOutExponentialEase
    {
        public static ExponentialEase Create() => new ExponentialEase() { Exponent = 5, EasingMode = EasingMode.EaseOut };
    }

    public static class EaseInExponentialEase
    {
        public static ExponentialEase Create() => new ExponentialEase() { Exponent = 5, EasingMode = EasingMode.EaseIn };
    }
}
