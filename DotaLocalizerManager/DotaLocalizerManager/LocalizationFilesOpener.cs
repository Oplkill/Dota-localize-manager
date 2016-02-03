using System.Collections.Generic;
using System.Linq;
using KV3Lib;

namespace DotaLocalizerManager
{
    public class LocalizationFilesOpener
    {
        public List<LocalizationFileClass> Files = new List<LocalizationFileClass>();

        public void LoadFile(string path, bool locked)
        {
            string fileText = System.IO.File.ReadAllText(path);

            LocalizationFileClass fileClass = new LocalizationFileClass { FilePathName = path, Locked = locked };

            List<KeyValue> keys = KVParser.Parse(fileText);

            parseTokens(ref fileClass, keys[0]);

            Files.Add(fileClass);
        }

        public void CompareAllFiles()
        {
            List<string> keys = new List<string>();

            foreach (var file in Files)
            {
                foreach (var kv in file.LocalizationKeys.Children)
                {
                    if(keys.FirstOrDefault(key => key == kv.Key) == null)
                        keys.Add(kv.Key);
                }
            }

            foreach (var file in Files)
            {
                foreach (var key in keys.Where(key => !file.LocalizationKeys.HasKeyInChildren(key)))
                {
                    file.LocalizationKeys.Children.Add(new KeyValue()
                    {
                        Key = key,
                        Value = "",
                        Parent = file.LocalizationKeys,
                    });
                }
            }
        }

        public void SaveAllFiles()
        {
            foreach (var file in Files)
            {
                if(!file.Locked)
                    System.IO.File.WriteAllText(file.FilePathName, file.Keys.ToString());
            }
        }

        private void parseTokens(ref LocalizationFileClass fileClass, KeyValue key)
        {
            fileClass.Language = key.Children[0].Value;
            fileClass.Keys = key;
            fileClass.LocalizationKeys = key.Children[1];
        }
    }
}