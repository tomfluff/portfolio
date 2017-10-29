using System;

namespace SunnyDay.Client.Core.Models
{
    public class StorageTokenElement
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public string MainSasToken { get; set; }
        public string ReadSasToken { get; set; }
    }
}
