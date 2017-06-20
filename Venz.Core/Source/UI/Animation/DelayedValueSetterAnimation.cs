using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Animation
{
    public class DelayedValueSetterAnimation
    {
        private ObjectAnimationUsingKeyFrames ObjectAnimation;

        public Object InitialValue { get; set; }
        public Object Value { get; set; }
        public Int32 Delay { get; set; }



        public DelayedValueSetterAnimation(FrameworkElement target, String property)
        {
            ObjectAnimation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(ObjectAnimation, target);
            Storyboard.SetTargetProperty(ObjectAnimation, property);
        }

        public Timeline GetTimeline()
        {
            ObjectAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = InitialValue });
            ObjectAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(Delay)), Value = Value });
            return ObjectAnimation;
        }
    }
}
