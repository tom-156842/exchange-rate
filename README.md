# Exchange Rate Assignment

This repo is a response to the take home assignment to implement a service to return historical exchange rate information.

# Exchange Rates Solution

The exchange rates solution includes several projects comprised of web, library and test components.

## Build and run

The solution is dependent on [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) which must be installed locally before continuing.

To build and run the solution from the command line:
1. Open a command prompt
2. Change directory to the `\src\ExchangeRates\ExchangeRates.Web`
3. Run `dotnet run`

To build and run the solution from Visual Studio 2022:
1. Change folder in file explorer to `\src\ExchangeRates`
2. Double-click the `ExchangeRates.sln` file to open the solution in Visual Studio
3. Ensure `ExchangeRates.Web` is the start-up project
4. Click run from the toolbar, or press F5

## Example usage

### Example 1

Fetch the exchange rate history for specific dates.

- Source currency: SEK
- Target currency: NOK
- Dates: 2018-02-01, 2018-02-15, 2018-03-01

```
https://localhost:7027/api/exchangeratehistory?source=SEK&target=NOK&dates=2018-02-01&dates=2018-02-15&dates=2018-03-01
```
