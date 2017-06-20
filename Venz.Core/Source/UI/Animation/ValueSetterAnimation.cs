using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Animation
{
    public class ValueSetterAnimation
    {
        private ObjectAnimationUsingKeyFrames ObjectAnimation;

        public Object Value { get; set; }



        public ValueSetterAnimation(FrameworkElement target, String property)
        {
            ObjectAnimation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(ObjectAnimation, target);
            Storyboard.SetTargetProperty(ObjectAnimation, property);
        }

        public Timeline GetTimeline()
        {
            ObjectAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = Value });
            return ObjectAnimation;
        }
    }
}
