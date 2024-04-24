using ISO8583_Client_Demo.Services.Implementations;
using ISO8583_Client_Demo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");
var serviceProvider = new ServiceCollection()
            .AddTransient<IFinancialServices, FinancialServices>() // Register the interface with its implementation
            .BuildServiceProvider();

// Resolve the service
var finServices = serviceProvider.GetService<IFinancialServices>();
finServices.CreditWorthyCheck();
