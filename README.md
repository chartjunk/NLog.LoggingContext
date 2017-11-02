# NLog.LoggingScope
`LoggingScope` aims to make it less of a pain to explore and discover the full story behind each log entry. By attaching context specific `ScopeIds` to entries, one can trivially filter through piles of trace and end up with only those entries that have a meaning in the current context.


# Getting started
NuGet package (https://www.nuget.org/packages/NLog.LoggingScope/) is available via NPM
```
install-package NLog.LoggingScope
```

# Main features
## Enrich your log entries with `ScopeIds`
They are uniform within blocks of code:

```C#
using(new LoggingScope("MyScope"))
{
  Logger.Debug("Hello");
  Logger.Debug("World!");
}

using(new LoggingScope("AnotherScope"))
{
  Logger.Info("Fizz");
  Logger.Info("Buzz");
}
```
##### Result:
<table>
  <tr>
    <th>ScopeId</th>
    <th>ScopeName</th>
    <th>Severity</th>
    <th>Message</th>
  </tr>
  <tr>
    <td>4e4e92ee-6c6c-48b0-8dc3-04d2cc4d1f31</td>
    <td>MyScope</td>
    <td>Debug</td>
    <td>Hello</td>
  </tr>
  <tr>
    <td>4e4e92ee-6c6c-48b0-8dc3-04d2cc4d1f31</td>
    <td>MyScope</td>
    <td>Debug</td>
    <td>World</td>
  </tr>
  <tr>
    <td>f162e96b-9e64-4cc9-9e1d-f1105b32d204</td>
    <td>AnotherScope</td>
    <td>Info</td>
    <td>Fizz</td>
  </tr>
  <tr>
    <td>f162e96b-9e64-4cc9-9e1d-f1105b32d204</td>
    <td>AnotherScope</td>
    <td>Info</td>
    <td>Buzz</td>
  </tr>
</table>

Being unique for each scope, one is able to find out what happened in each block *during a single execution* by searching log entries by a `ScopeId`. A new `ScopeId` is generated for each instance of `LoggingScope`:
```C#
public class MyApp
{
  private static Logger Logger = LogManager.GetCurrentClassLogger();
  public void Execute()
  {
    using(new LoggingScope("Lorem"))
      Logger.Trace("Ipsum");
  }
}

var myApp = new MyApp();
myApp.Execute();
myApp.Execute();
```



##### Result:
<table>
  <tr>
    <th>ScopeId</th>
    <th>ScopeName</th>
    <th>Severity</th>
    <th>Message</th>
  </tr>
  <tr>
    <td>3058f00b-7d09-4d70-9720-bbb7d5f6ac9a</td>
    <td>Lorem</td>
    <td>Trace</td>
    <td>Ipsum</td>
  </tr>
  <tr>
    <td>d98dc934-2675-4072-9b57-ced90a4071d6</td>
    <td>Lorem</td>
    <td>Trace</td>
    <td>Ipsum</td>
  </tr>
</table>
</br>

## Nested scopes are attached to each other with a parent-child relationship:
```C#
using(new LoggingScope("TheParent"))
{
  Logger.Debug("One");
  using(new LoggingScope("TheChild"))
  {
    Logger.Debug("Two");
  }
  Logger.Debug("Three");
}
```
##### Result:
<table>
  <tr>
    <th>ScopeId</th>
    <th>ScopeName</th>
    <th>ParentScopeId</th>
    <th>Message</th>
  </tr>
  <tr>
    <td>94bb82a3-7b63-4bb6-aa66-807f2a2d863d</td>
    <td>TheParent</td>
    <td/>
    <td>One</td>
  </tr>
  <tr>
    <td>75ccddf9-d596-4ceb-b2ae-fe63b02b8b1b</td>
    <td>TheChild</td>
    <td>94bb82a3-7b63-4bb6-aa66-807f2a2d863d</td>
    <td>Two</td>
  </tr>
  <tr>
    <td>94bb82a3-7b63-4bb6-aa66-807f2a2d863d</td>
    <td>TheParent</td>
    <td/>
    <td>Three</td>
  </tr>
</table>

## SQL-target
TODO

## NLog configuration
TODO

## Custom fields
TODO
