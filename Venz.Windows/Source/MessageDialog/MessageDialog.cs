using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Venz.Windows
{
    public static class MessageDialog
    {
        public static Task ShowAsync(String header, String message)
        {
            var dialog = new global::Windows.UI.Popups.MessageDialog(message, header);
            return dialog.ShowAsync().AsTask();
        }

        public static async Task<String> ConfirmAsync(String header, String message, String buttonLabel1, String buttonLabel2)
        {
            var dialog = new global::Windows.UI.Popups.MessageDialog(message, header);
            dialog.Commands.Add(new UICommand(buttonLabel1, null, 1));
            dialog.Commands.Add(new UICommand(buttonLabel2, null, 2));
            var command = await dialog.ShowAsync();
            return command.Label;
        }
    }
}
