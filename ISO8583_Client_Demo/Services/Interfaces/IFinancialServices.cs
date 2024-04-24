namespace ISO8583_Client_Demo.Services.Interfaces;

public interface IFinancialServices
{
    Task<bool> CreditWorthyCheck(/*AuthRequestDto authRequest*/);
}
