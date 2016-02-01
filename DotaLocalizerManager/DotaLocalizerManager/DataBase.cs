namespace DotaLocalizerManager
{
    public class DataBase
    {
        public FileOpener FileOpener;

        public string NewFormName;
        private const string ProgrammName = "Localizer - ";

        public bool Edited {
            get { return edited; }
            set
            {
                if(edited == value)
                    return;
                edited = value;
                NewFormName = ProgrammName;
                NewFormName += (edited) ? "*" : "";
            }
        }

        private bool edited;

        public void SaveFiles()
        {
            FileOpener.SaveAllFiles();
        }
    }
}