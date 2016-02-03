using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KV3Lib
{
    public static class KVParser
    {
        public static List<KeyValue> Parse(string code)
        {
            int n = 0;
            return ParseIt(code, ref n, null);
        }

        private static List<KeyValue> ParseIt(string code, ref int n, KeyValue parent)
        {
            List<KeyValue> keys = new List<KeyValue>();

            Token tok = new Token() {Tok = Toks.nil};
            while (tok.Tok != Toks.eof)
            {
                tok = GetToken(code, ref n);
                if(tok.Tok == Toks.eof)
                    break;
                if (tok.Tok == Toks.closeAngleBracket)
                {
                    return keys.Count > 0 ? keys : null;
                }
                string key = tok.Value;
                tok = GetToken(code, ref n);
                string value = tok.Value;
                KeyValue KValue = new KeyValue()
                {
                    Key = key,
                    Parent = parent,
                    Value = value,
                };
                if (tok.Tok == Toks.openAngleBracket)
                {
                    KValue.Type = KvType.ParentKey;
                    KValue.Children = ParseIt(code, ref n, KValue);
                }
                else
                {
                    KValue.Type = KvType.KeyValue;
                }
                keys.Add(KValue);
            }

            return keys;
        }

        private static bool isSpace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\r')
                return true;
            if (c == '\n')
            { return true; }

            return false;
        }

        private static Token GetToken(string code, ref int n)
        {
            ToStart: //goto!

            //eof
            if (n >= code.Length)
                return new Token() { Tok = Toks.eof };
            //------------------

            //Spaces
            if (isSpace(code[n]))
            {
                n++;
                goto ToStart; // Повторная проверка
            }
            //------------------

            //Comments
            if (code[n] == '/')
            {
                n++; //todo вставить проверку на ошибку не-до коммента
                n++;

                while (code[n] != '\n') //todo добавить другие окончания строк и конца файла
                {
                    n++;
                }
                n++;

                goto ToStart; // Повторная проверка
            }
            //------------------

            //Symbols
            switch (code[n])
            {
                case '{':
                    n++;
                    return new Token() { Tok = Toks.openAngleBracket };

                case '}':
                    n++;
                    return new Token() { Tok = Toks.closeAngleBracket };
            }
            //------------------

            //Texts
            if (code[n] == '\"')
            {
                string text = "";
                n++;
                while (code[n] != '\"')
                {
                    if (code[n] == '\\' && code[n + 1] == '\"')
                    {
                        n++;
                        text += '\\';
                    }
                    text += code[n];
                    n++;
                }
                n++;
                return new Token() { Tok = Toks.text, Value = text };
            }
            //------------------

            return null; //todo добавить исключение
        }

        private class Token
        {
            public Toks Tok;
            public string Value;
        }

        private enum Toks
        {
            nil,
            openAngleBracket, closeAngleBracket,
            text,
            eof,
        }
    }
}
