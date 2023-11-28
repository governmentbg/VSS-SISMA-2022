namespace SISMA.Core.Models.Common
{
    public class ContextHelpVM
    {

        public string Title { get; set; }
        public string HelpFile { get; set; }

        public ContextHelpVM(string title, string helpFile)
        {
            Title = title;
            HelpFile = helpFile;
        }
    }
}
