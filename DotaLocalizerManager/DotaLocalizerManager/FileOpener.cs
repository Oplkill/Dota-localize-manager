using System.Collections.Generic;
using System.Linq;

namespace DotaLocalizerManager
{
    public class FileOpener
    {
        public List<FileClass> Files = new List<FileClass>();

        public void LoadFile(string path, bool locked)
        {
            string fileText = System.IO.File.ReadAllText(path);

            FileClass fileClass = new FileClass {FilePathName = path, Locked = locked};

            var tokens = TokenAnalizer.AnalizeString(fileText);

            parseTokens(ref fileClass, tokens);

            Files.Add(fileClass);
        }

        public void CompareAllFiles()
        {
            List<string> keys = new List<string>();

            foreach (var pare in Files.SelectMany(file => file.LocalizeString.Where(pare => keys.FirstOrDefault(str => str == pare.Key) == null)))
            {
                keys.Add(pare.Key);
            }

            foreach (var file in Files)
            {
                foreach (var key in keys.Where(key => !file.LocalizeString.ContainsKey(key)))
                {
                    file.LocalizeString.Add(key, "");
                }
            }
        }

        public void SaveAllFiles()
        {
            foreach (var file in Files)
            {
                if(!file.Locked)
                    System.IO.File.WriteAllText(file.FilePathName, generateText(file));
            }
        }

        private string generateText(FileClass file)
        {
            string text = "";

            text += "\"lang\"\n";
            text += "{\n";
            text += "\t\"Language\"\t\"" + file.Language + "\"\n";
            text += "\t\"Tokens\"\n";
            text += "\t{\n";

            text = file.LocalizeString.Aggregate(text, (current, pare) => current + 
                ("\t\t\"" + pare.Key + "\"\t\"" + pare.Value + "\"\n"));

            text += "\t}\n";
            text += "}";

            return text;
        }

        private void parseTokens(ref FileClass fileClass, List<Token> tokens)
        {
            int n = 0;

            if(tokens[n].Tok != Toks.lang)
                return; //todo вставить ошибку

            n++;
            if(tokens[n].Tok != Toks.openAngleBracket)
                return; //todo вставить ошибку

            n++;
            if (tokens[n].Tok != Toks.language)
                return; //todo вставить ошибку

            n++;
            if(tokens[n].Tok != Toks.text)
                return; //todo вставить ошибку
            fileClass.Language = tokens[n].Value;

            n++;
            if(tokens[n].Tok != Toks.tokens)
                return; //todo вставить ошибку

            n++;
            if (tokens[n].Tok != Toks.openAngleBracket)
                return; //todo вставить ошибку

            n++;
            if (tokens[n].Tok != Toks.text && tokens[n].Tok != Toks.closeAngleBracket)
                return; //todo вставить ошибку

            while (tokens[n].Tok != Toks.closeAngleBracket)
            {
                if (tokens[n].Tok != Toks.text)
                    return; //todo вставить ошибку
                string str1 = tokens[n].Value;
                n++;
                if (tokens[n].Tok != Toks.text)
                    return; //todo вставить ошибку
                string str2 = tokens[n].Value;
                fileClass.LocalizeString.Add(str1, str2);
                n++;
            }
        }
    }
}