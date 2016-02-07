using System.Collections.Generic;
using System.Linq;
using KV3Lib;

namespace DotaLocalizerManager
{
    public class AbilityFileOpener
    {
        public List<AbilityClass> Abilities = new List<AbilityClass>();

        public void LoadFile(string path)
        {
            Abilities.Clear();
            string fileText = System.IO.File.ReadAllText(path);

            List<KeyValue> keys = KVParser.Parse(fileText);

            parse(keys[0]);
        }

        private void parse(KeyValue abilsValue)
        {
            foreach (var kv in abilsValue.Children)
            {
                if (kv.Type == KvType.ParentKey)
                {
                    Abilities.Add(new AbilityClass()
                    {
                        Name = kv.Key,
                        AbilityValues = kv,
                    });
                }
            }
        }

        public void LocalizateAbilities(ref LocalizationFileClass locFile)
        {
            foreach (var abil in Abilities)
            {
                locFile.LocalizationKeys.Children.Add(new KeyValue()
                { // Имя способности
                    Key = "DOTA_Tooltip_ability_" + abil.Name,
                    Parent = locFile.LocalizationKeys,
                    Type = KvType.KeyValue,
                    Value = "",
                });

                locFile.LocalizationKeys.Children.Add(new KeyValue()
                { // описание способности
                    Key = "DOTA_Tooltip_ability_" + abil.Name + "_Description",
                    Parent = locFile.LocalizationKeys,
                    Type = KvType.KeyValue,
                    Value = "",
                });

                localizateAbilitiesSpecials(ref locFile, abil);
                localizateAbilitiesAuras(ref locFile, abil);
            }
        }

        private void localizateAbilitiesSpecials(ref LocalizationFileClass locFile, AbilityClass abil)
        {
            var specialValues = abil.AbilityValues.FindChildren("AbilitySpecial");
            if (specialValues == null)
                return;
            if (specialValues.Children == null)
                return;

            foreach (var kv in specialValues.Children)
            {
                foreach (var kvVar in kv.Children.Where(kvVar => kvVar.Key != "var_type"))
                {
                    locFile.LocalizationKeys.Children.Add(new KeyValue()
                    { // доп переменные
                        Key = "DOTA_Tooltip_ability_" + abil.Name + "_" + kvVar.Key,
                        Parent = locFile.LocalizationKeys,
                        Type = KvType.KeyValue,
                        Value = "",
                    });
                }
            }
        }

        private void localizateAbilitiesAuras(ref LocalizationFileClass locFile, AbilityClass abil)
        {
            var modeferValues = abil.AbilityValues.FindChildren("Modifiers");
            if (modeferValues == null)
                return;
            if (modeferValues.Children == null)
                return;

            foreach (var modKv in modeferValues.Children.Where(modKv => isAura(modKv)))
            {
                locFile.LocalizationKeys.Children.Add(new KeyValue()
                { // аура
                    Key = "DOTA_Tooltip_ability_" + modKv.Key,
                    Parent = locFile.LocalizationKeys,
                    Type = KvType.KeyValue,
                    Value = "",
                });
            }
        }

        private bool isAura(KeyValue modKv)
        {
            //if(modKv.HasKeyInChildren("IsHidden"))
            //    if (modKv.FindChildren("IsHidden").Value == "1")
            //        return false;

            if (modKv.HasKeyInChildren("Aura"))
                return true;

            if (modKv.HasKeyInChildren("IsDebuff"))
                if (modKv.FindChildren("IsDebuff").Value == "1")
                    return true;

            if (modKv.HasKeyInChildren("IsBuff"))
                if (modKv.FindChildren("IsBuff").Value == "1")
                    return true;

            return false;
        }
    }
}