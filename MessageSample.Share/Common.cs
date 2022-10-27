namespace MessageSample.Share
{
    public static class Common
    {
        public static string GetString(this byte[] bytes)
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);

            string prettyMessage = string.Empty;
            foreach (var nullChar in message)
            {
                if (nullChar != '\0')
                {
                    prettyMessage += nullChar;
                }
            }
            return prettyMessage;
        }

        public static ClientTypeEnum GetClientType(this string message)
        {
            var clientType = message.Split('|')[0];
            return Enum.Parse<ClientTypeEnum>(clientType);
        }

        public static StorageTypeEnum GetStorageType(this string message)
        {
            var storageType = message.Split('|')[2];
            return Enum.Parse<StorageTypeEnum>(storageType);
        }

        public static string GetStorage(this string message)
        {
            return message.Split('|')[3];
        }
    }
}