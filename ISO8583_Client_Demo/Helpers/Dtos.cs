namespace ISO8583_Client_Demo.Helpers;

public class AuthRequestDto
{
    public string? AccountNumber { get; set; }
    public string? ProCode { get; set; }
    public string? AmountTransaction { get; set; }
    public string? CardExpDate { get; set; }
    public string? MerchantType { get; set; }
    public string? PosEntryMode { get; set; }
    public string? CardSequenceNumber { get; set; }
    public string? PosConditionCode { get; set; }
    public string? PosPinCaptureCode { get; set; }
    public string? AmountTransactionFee { get; set; }
    public string? AcqInstIdCode { get; set; }
    public string? Track2Data { get; set; }
    public string? RetrievalRefNumber { get; set; }
    public string? ServiceRestrictionCode { get; set; }
    public string? CardAcceptorTerminalId { get; set; }
    public string? CardAcceptorIdCode { get; set; }
    public string? CardAcceptorNameLocation { get; set; }
    public string? CurrencyCodeTransaction { get; set; }
    public string? PinData { get; set; }
    public string? SecRelatedCtrlInfo { get; set; }
    public string? AdditionalAmounts { get; set; }
    public string? IntCircuitCardSysRelData { get; set; }
    public string? MsgReasonCode { get; set; }
    public string? TransEchoData { get; set; }
    public string? PaymtInfo { get; set; }
    public string? PrivFieldMgtData { get; set; }
    public string? AccId1 { get; set; }
    public string? AccId2 { get; set; }
    public string? PosDataCode { get; set; }
    public string? NearFieldCommData { get; set; }
    public string? SecMsgHashValue { get; set; }

}
