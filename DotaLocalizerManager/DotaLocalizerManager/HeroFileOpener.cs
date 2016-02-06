using System.Collections.Generic;
using KV3Lib;

namespace DotaLocalizerManager
{
    public class HeroFileOpener
    {
        public List<HeroClass> Heroes = new List<HeroClass>();

        public void LoadFile(string path)
        {
            Heroes.Clear();
            string fileText = System.IO.File.ReadAllText(path);

            List<KeyValue> keys = KVParser.Parse(fileText);

            parse(fileText, keys[0]);
        }

        private void parse(string code, KeyValue heroeKeyValues)
        {
            foreach (var kv in heroeKeyValues.Children)
            {
                if (kv.Type == KvType.ParentKey)
                {
                    Heroes.Add(new HeroClass()
                    {
                        Name = kv.Key,
                        HeroValues = kv,
                    });
                }
            }
        }

        public void LocalizateHeroes(ref LocalizationFileClass locFile)
        {
            foreach (var hero in Heroes)
            {
                if (!locFile.LocalizationKeys.HasKeyInChildren(hero.Name))
                {
                    locFile.LocalizationKeys.Children.Add(new KeyValue()
                    {
                        Key = hero.Name,
                        Value = "",
                        Parent = locFile.LocalizationKeys,
                        Type = KvType.KeyValue,
                    });
                }
            }
        }
    }
}