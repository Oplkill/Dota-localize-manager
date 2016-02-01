using System.Collections.Generic;

namespace DotaLocalizerManager
{
    public class FileClass
    {
        public bool Locked;
        public string FilePathName;
        public string Language;
        public Dictionary<string, string> LocalizeString = new Dictionary<string, string>();
    }
}