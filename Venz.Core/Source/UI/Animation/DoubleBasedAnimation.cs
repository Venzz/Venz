using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Animation
{
    public class DoubleBasedAnimation
    {
        private DoubleAnimationUsingKeyFrames DoubleAnimation;

        public Int32 Delay { get; set; }
        public Int32 Duration { get; set; }
        public Double From { get; set; }
        public Double To { get; set; }
        public EasingFunctionBase EasingFunction { get; set; }



        public DoubleBasedAnimation(FrameworkElement target, String property)
        {
            DoubleAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(DoubleAnimation, target);
            Storyboard.SetTargetProperty(DoubleAnimation, property);
        }

        public DoubleBasedAnimation(Geometry target, String property)
        {
            DoubleAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(DoubleAnimation, target);
            Storyboard.SetTargetProperty(DoubleAnimation, property);
        }

        public Timeline GetTimeline()
        {
            DoubleAnimation.BeginTime = TimeSpan.FromMilliseconds(Delay);
            DoubleAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = From });
            if (EasingFunction != null)
                DoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(Duration)), Value = To, EasingFunction = EasingFunction });
            else
                DoubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(Duration)), Value = To });
            return DoubleAnimation;
        }
    }
}
