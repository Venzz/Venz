using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Venz.UI.Xaml
{
    public partial class Frame: Windows.UI.Xaml.Controls.Frame
    {
        public FrameNavigation Navigation { get; }



        public Frame()
        {
            Navigation = new FrameNavigation(this);
        }

        public async Task StoreNavigationStateAsync()
        {
            var navigationState = GetNavigationState();
            var stateFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("navigation.state", CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
            try
            {
                await FileIO.WriteLinesAsync(stateFile, new String[] { navigationState, Navigation.ToString() }).AsTask().ConfigureAwait(false);
            }
            catch (Exception)
            {
                #if !DEBUG
                await stateFile.DeleteAsync().AsTask().ConfigureAwait(false);
                #endif
            }
        }

        public async Task RestoreNavigationStateAsync()
        {
            var stateFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("navigation.state", CreationCollisionOption.OpenIfExists);
            try
            {
                var lines = await FileIO.ReadLinesAsync(stateFile);
                if (lines.Count != 2)
                    return;

                Navigation.Restore(lines[1]);
                SetNavigationState(lines[0]);
                await stateFile.DeleteAsync();
            }
            catch (Exception)
            {
                #if !DEBUG
                await stateFile.DeleteAsync();
                #endif
            }
        }
    }
}
