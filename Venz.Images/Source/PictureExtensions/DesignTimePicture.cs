using System;

namespace Venz.Images
{
    public sealed class DesignTimePicture: UriPicture
    {
        private String FullFilePath;

        public DesignTimePicture(String fullFilePath) { FullFilePath = fullFilePath; }

        public override Uri GetUri() => new Uri(FullFilePath, UriKind.Absolute);
    }
}
