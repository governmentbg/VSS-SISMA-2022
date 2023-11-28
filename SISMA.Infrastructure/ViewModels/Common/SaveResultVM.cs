namespace SISMA.Infrastructure.ViewModels.Common
{
    public class SaveResultVM
    {
        public bool IsSuccessfull { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public SaveResultVM()
        {

        }

        public SaveResultVM(bool isSuccessfull, string errorMessage = null)
        {
            IsSuccessfull = isSuccessfull;
            ErrorMessage = errorMessage;
        }
    }
}
