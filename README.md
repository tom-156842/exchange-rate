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

## OpenAPI

The web project exposes an OpenAPI definition that can be browsed by navigating to the Swagger page.

`https://localhost:7027/swagger`

## Example usage

### Example 1

Fetch the exchange rate history for specific dates.

- Source currency: SEK
- Target currency: NOK
- Dates: 2018-02-01, 2018-02-15, 2018-03-01

```
https://localhost:7027/api/exchangeratehistory?source=SEK&target=NOK&dates=2018-02-01&dates=2018-02-15&dates=2018-03-01
```

### Example 2

Fetch the exchange rate history for a large number of dates.

- Source currency: SEK
- Target currency: NOK
- Dates: Multiple

```
https://localhost:7027/api/ExchangeRateHistory?Dates=2018-02-01&Dates=2018-02-15&Dates=2018-03-01&Dates=2018-03-15&Dates=2018-04-01&Dates=2018-04-15&Dates=2018-05-01&Dates=2018-05-15&Dates=2018-06-01&Dates=2018-06-15&Dates=2018-07-01&Dates=2018-07-15&Dates=2018-08-01&Dates=2018-08-15&Dates=2018-09-01&Dates=2018-09-15&Dates=2018-10-01&Dates=2018-10-15&Dates=2018-11-01&Dates=2018-11-15&Dates=2018-12-01&Dates=2018-12-15&Dates=2019-01-01&Dates=2019-01-15&Dates=2019-02-01&Dates=2019-02-15&Dates=2019-03-01&Dates=2019-03-15&Dates=2019-04-01&Dates=2019-04-15&Dates=2019-05-01&Dates=2019-05-15&Dates=2019-06-01&Dates=2019-06-15&Dates=2019-07-01&Dates=2019-07-15&Dates=2019-08-01&Dates=2019-08-15&Dates=2019-09-01&Dates=2019-09-15&Dates=2019-10-01&Dates=2019-10-15&Dates=2019-11-01&Dates=2019-11-15&Dates=2019-12-01&Dates=2019-12-15&Dates=2020-01-01&Dates=2020-01-15&Dates=2020-02-01&Dates=2020-02-15&SourceCurrency=SEK&TargetCurrency=NOK
```

# Implementation Notes

- exchangerate.host responds with `HTTP OK` messages to endpoints that aren't valid, eg. `/api/not-a-valid-path`
- exchangerate.host will compensate when invalid dates or symbols are passed, sometimes assuming defaults
- The presence of `success = true` in the response body has been used to determine if a request is successful
- To accommodate a large number of requested dates, dates are grouped by year and segment of the year is requested and then mapped back to the requested dates to give consistent performance