using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Xaml
{
    public interface IOverlaidContentTransition
    {
        Storyboard CreateShowing(OverlaidContent target);
        Storyboard CreateHiding(OverlaidContent target);
    }
}
