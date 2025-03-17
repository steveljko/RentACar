namespace RentACar.DTOs;

public class Error
{
    public Error(string message, string code)
    {
        Message = message;
        Code = code;
    }
    
    public string Message { get; }
    public string Code { get; }
}