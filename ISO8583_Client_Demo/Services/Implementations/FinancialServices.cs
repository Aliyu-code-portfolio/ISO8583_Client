using BIM_ISO8583.NET;
using ISO8583_Client_Demo.Helpers;
using ISO8583_Client_Demo.Services.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ISO8583_Client_Demo.Services.Implementations;

internal sealed class FinancialServices : IFinancialServices
{
    public async Task<bool> CreditWorthyCheck(/*AuthRequestDto authRequest*/)
    {
        var authRequest = new AuthRequestDto
        {
            AccountNumber = "0123456789",
            ProCode = "311000",
            AmountTransaction = "100000",
            CardExpDate = "2408",
            MerchantType = "6011",
            PosEntryMode = "011",
            //CardSequenceNumber = "777",
            PosConditionCode = "00",
            PosPinCaptureCode = "12",
            AmountTransactionFee = "D1000",
            AcqInstIdCode = "123b567890",
            //Track2Data =PAN+"D"+CardExpDate+ServiceRestrictionCode,
            RetrievalRefNumber = GenerateRandomNumber(3, 13),
            //ServiceRestrictionCode ="907",
            CardAcceptorTerminalId = "234567891Q#4",
            CardAcceptorIdCode = "0012345678@$",
            CardAcceptorNameLocation = "34 Joseph Lambo Street, Lagos State ,L, N,",
            CurrencyCodeTransaction = "566",
            //PinData = "",
            //SecRelatedCtrlInfo = "",
            //AdditionalAmounts = "",
            //IntCircuitCardSysRelData = "",
            //MsgReasonCode ="",
            TransEchoData = "Echo",
            //PaymtInfo = "",
            //PrivFieldMgtData ="",
            //AccId1 ="",
            //AccId2 ="",
            PosDataCode = "010101114004101",
            //NearFieldCommData ="",
            SecMsgHashValue = ""
        };
        var isoMsg = GenerateIsoMessage<AuthRequestDto>(130, MessageType.AuthorizationRequest, authRequest);
        var asciiMsg = Encoding.ASCII.GetBytes($"{isoMsg}<EOF>");
        var response = await SendClientRequest(asciiMsg);
        var unpackedResponse = Unpack(response);
        var customerAccountBalance = Convert.ToInt32(unpackedResponse[8]);
        Console.WriteLine($"Customer Account Balance -> {customerAccountBalance}\n");

        if (unpackedResponse[39] == "00")
        {
            return true;
        }
        
        return false;
    }
    private string GenerateIsoMessage<T>(byte n_DataElement, MessageType mTI, T payLoad)
    {
        string[] DE = new string[n_DataElement];
        DateTime currentDateTime = DateTime.UtcNow.AddHours(1);
        string transmissionDateTime = ConvertDateTimeFormat(currentDateTime);
        string sysTraceAuditNum = GenerateSysTraceAuditNum();
        string localTime = GetTimeOnly(currentDateTime);
        string localDate = GetDateOnly(currentDateTime);
        ISO8583 iso8583 = new();
        switch (mTI)
        {
            case MessageType.AuthorizationRequest:
                string MTI = "0100";
                if (payLoad is AuthRequestDto authRequest)
                {
                    DE[2] = authRequest.AccountNumber;
                    DE[3] = authRequest.ProCode;
                    DE[4] = authRequest.AmountTransaction;
                    DE[7] = transmissionDateTime;
                    DE[11] = sysTraceAuditNum;
                    DE[12] = localTime;
                    DE[13] = localDate;
                    DE[14] = authRequest.CardExpDate;
                    DE[18] = authRequest.MerchantType;
                    DE[22] = authRequest.PosEntryMode;
                    if (authRequest.CardSequenceNumber is not null) DE[23] = authRequest.CardSequenceNumber;
                    DE[25] = authRequest.PosConditionCode;
                    if (authRequest.PosPinCaptureCode is not null) DE[26] = authRequest.PosPinCaptureCode;
                    DE[28] = authRequest.AmountTransactionFee;
                    DE[32] = authRequest.AcqInstIdCode;
                    if (authRequest.Track2Data is not null) DE[35] = authRequest.Track2Data;
                    DE[37] = authRequest.RetrievalRefNumber;
                    if (authRequest.ServiceRestrictionCode is not null) DE[40] = authRequest.ServiceRestrictionCode;
                    DE[41] = authRequest.CardAcceptorTerminalId;
                    DE[42] = authRequest.CardAcceptorIdCode;
                    DE[43] = authRequest.CardAcceptorNameLocation;
                    DE[49] = authRequest.CurrencyCodeTransaction;
                    if (authRequest.PinData is not null) DE[52] = authRequest.PinData;
                    if (authRequest.SecRelatedCtrlInfo is not null) DE[53] = authRequest.SecRelatedCtrlInfo;
                    if (authRequest.AdditionalAmounts is not null) DE[54] = authRequest.AdditionalAmounts;
                    if (authRequest.IntCircuitCardSysRelData is not null) DE[55] = authRequest.IntCircuitCardSysRelData;
                    if (authRequest.MsgReasonCode is not null) DE[56] = authRequest.MsgReasonCode;
                    if (authRequest.TransEchoData is not null) DE[59] = authRequest.TransEchoData;
                    if (authRequest.PaymtInfo is not null) DE[60] = authRequest.PaymtInfo;
                    if (authRequest.PrivFieldMgtData is not null) DE[62] = authRequest.PrivFieldMgtData;
                    if (authRequest.AccId1 is not null) DE[102] = authRequest.AccId1;
                    if (authRequest.AccId2 is not null) DE[103] = authRequest.AccId2;
                    DE[123] = authRequest.PosDataCode;
                    if (authRequest.NearFieldCommData is not null) DE[124] = authRequest.NearFieldCommData;
                    if (authRequest.SecMsgHashValue is not null) DE[128] = authRequest.SecMsgHashValue;
                    else DE[64] = authRequest.SecMsgHashValue;
                    return iso8583.Build(DE, MTI);
                }
                else
                {
                    // Handle the case when payLoad is not of type AuthRequestDto
                    // For example, you can throw an exception or return a default value
                    throw new ArgumentException("Payload is not of type AuthRequestDto");
                }
                break;
        }
        throw new ArgumentException("Invalid MessageType or Payload type");

    }
    private static string[] Unpack(string resData)
    {
        ISO8583 iso8583 = new ISO8583();

        string[] DE;

        DE = iso8583.Parse(resData);

        return DE;
    }
    private static async Task<string> SendClientRequest(byte[] asciiMsg)
    {
        byte[] bytes = new byte[1024];
        string response = string.Empty;

        try
        {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[1];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 9000);

            // Create a TCP/IP  socket.
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                // Connect to Remote EndPoint
                 sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Send the data through the socket.
                int bytesSent = await sender.SendAsync(asciiMsg);

                // Receive the response from the remote device.
                int bytesRecieved = await sender.ReceiveAsync(bytes);

                response = Encoding.ASCII.GetString(bytes, 0, bytesRecieved);

                Console.WriteLine("Echoed test = {0}", response);
                Console.ReadKey();
                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                return response;
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return response;
    }
    private string ConvertDateTimeFormat(DateTime dateTime)
    {
        var month = dateTime.Month;
        var day = dateTime.Day;
        var hr = dateTime.Hour;
        var minute = dateTime.Minute;
        var second = dateTime.Second;
        var stringDateTime = string.Concat(month, day, hr, minute, second);
        return stringDateTime;
    }
    private string GenerateSysTraceAuditNum()
    {
        string randomNumber = GenerateRandomNumber(2, 7);
        return randomNumber;
    }
    private string GenerateRandomNumber(byte start, byte end)
    {
        Random random = new();
        var randomNumber = random.Next(start, end);
        return randomNumber.ToString();

    }
    private string GetTimeOnly(DateTime dateTime)
    {
        var hr = dateTime.Hour;
        var min = dateTime.Minute;
        var sec = dateTime.Second;
        var stringTime = string.Concat(hr, min, sec);
        return stringTime;
    }
    private string GetDateOnly(DateTime dateTime)
    {
        var month = dateTime.Month;
        var day = dateTime.Day;
        var stringDate = string.Concat(month, day);
        return stringDate;
    }
    //private Task ConnectToServerSocket(string ipAddress, string portNumber)
    //{
    //    //var host = Dns.GetHostAddresses("");
    //}
}
