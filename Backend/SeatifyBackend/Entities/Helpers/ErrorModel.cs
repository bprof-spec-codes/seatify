namespace Entities.Helpers;

public class ErrorModel
{
    public string ErrorMessage { get; set; }
    
    public ErrorModel(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}
