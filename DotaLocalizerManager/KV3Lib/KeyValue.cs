using System;
using System.Collections.Generic;
using System.Linq;

namespace KV3Lib
{
    public class KeyValue
    {
        public string Key;
        public string Value;
        public KvType Type;
        public List<KeyValue> Children;
        public KeyValue Parent;

        public bool HasKeyInChildren(string key)
        {
            return Children.FirstOrDefault(kv => kv.Key == key) != null;
        }

        public KeyValue FindChildren(string key)
        {
            return Children.FirstOrDefault(kv => kv.Key == key);
        }

        public int FindChildrenId(string key)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Key == key)
                    return i;
            }

            return -1;
        }

        public override string ToString()
        {
            string str = "";

            str += "\"" + Key + "\"";
            if (Type == KvType.KeyValue)
                str += "\t\"" + Value + "\"\n";
            else
            {
                str += "\n{\n";
                if (Children != null)
                {
                    foreach (var kv in Children)
                    {
                        str += kv.ToString();
                    }
                    str += "\n";
                }
                str += "}\n";
            }

            return str;
        }
    }

    public enum KvType
    {
        /// <summary>
        /// "Key" "Value"
        /// </summary>
        KeyValue,
        /// <summary>
        /// "Key"
        /// {
        /// }
        /// </summary>
        ParentKey,
    }
}