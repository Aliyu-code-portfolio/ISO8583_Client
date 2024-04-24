namespace ISO8583_Client_Demo.Helpers;

public enum MessageType
{
    AuthorizationRequest = 0100,
    AuthorizationResponse = 0110,
    FinancialTransactionReq = 0200,
    FinancialTransactionResponse = 0210,
    ReversalAdvice = 0420,
    ReversalAdviceRepeat = 0421,
    ReversalAdviceResponse = 0430,
    NetworkManagementRequest = 0800,
    NetworkManagementReqResponse = 0810
}
