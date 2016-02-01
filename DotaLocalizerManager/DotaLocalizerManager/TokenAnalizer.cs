using System.Collections.Generic;

namespace DotaLocalizerManager
{
    public class TokenAnalizer
    {
        public static List<Token> AnalizeString(string code)
        {
            List<Token> tokens = new List<Token>();

            int n = 0;
            while (code.Length > n)
            {
                var tok = GetToken(code, ref n);
                tokens.Add(tok);
                if(tok == null || tok.Tok == Toks.eof)
                    break;
            }

            return tokens;
        }

        private static bool isSpace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\r')
                return true;
            if (c == '\n')
            { return true; }

            return false;
        }

        private static Token isReserved(string text)
        {
            string text2 = text.ToLower();
            switch (text2)
            {
                case "lang":
                    return new Token() {Tok = Toks.lang};

                case "language":
                    return new Token() {Tok = Toks.language};

                case "tokens":
                    return new Token() {Tok = Toks.tokens};
            }

            return new Token() {Tok = Toks.text, Value = text};
        }

        private static Token GetToken(string code, ref int n)
        {
            ToStart: //goto!

            //eof
            if (n >= code.Length)
                return new Token() {Tok = Toks.eof};
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
                    return new Token() {Tok = Toks.openAngleBracket};

                case '}':
                    n++;
                    return new Token() {Tok = Toks.closeAngleBracket};
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
                return isReserved(text);
            }
            //------------------

            return null; //todo добавить исключение
        }
    }

    public class Token
    {
        public Toks Tok;
        public string Value;
    }

    public enum Toks
    {
        openAngleBracket, closeAngleBracket,
        language,
        tokens,
        lang,
        text, 
        eof,
    }
}