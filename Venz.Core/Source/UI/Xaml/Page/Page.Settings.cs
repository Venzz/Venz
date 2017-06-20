using System;
using Windows.Graphics.Display;

namespace Venz.UI.Xaml
{
    public partial class Page
    {
        private Settings SavedPageSettings;

        public void OverrideSettings(Settings settings)
        {
            if (SavedPageSettings != null)
                throw new InvalidOperationException("Unable to override page settings. Page settings is overridden already, use RestoreSettings to be able to override the settings again.");

            SavedPageSettings = new Settings() { BottomAppBar = BottomAppBar, Orientation = DisplayInformation.AutoRotationPreferences };
            if (settings.BottomAppBar != null)
                BottomAppBar = settings.BottomAppBar;
            if (settings.Orientation.HasValue)
                DisplayInformation.AutoRotationPreferences = settings.Orientation.Value;
        }

        public void RestoreSettings()
        {
            if (SavedPageSettings != null)
            {
                BottomAppBar = SavedPageSettings.BottomAppBar;
                DisplayInformation.AutoRotationPreferences = SavedPageSettings.Orientation.Value;
                SavedPageSettings = null;
            }
        }

        public class Settings
        {
            public Windows.UI.Xaml.Controls.AppBar BottomAppBar { get; set; }
            public DisplayOrientations? Orientation { get; set; }
        }
    }
}
