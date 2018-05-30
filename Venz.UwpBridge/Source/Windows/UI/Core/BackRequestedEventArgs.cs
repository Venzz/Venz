using System;

namespace Windows.UI.Core
{
    public class BackRequestedEventArgs: EventArgs
    {
        public Boolean Handled { get; set; }
    }
}
