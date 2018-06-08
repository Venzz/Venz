namespace System.Text
{
    public static class EncodingExtensions
    {
        public static String GetString(this Encoding encoding, Byte[] data)
        {
            return encoding.GetString(data, 0, data.Length);
        }
    }
}
