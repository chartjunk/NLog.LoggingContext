# NLog.LoggingScope
`LoggingScope` aims to make it less of a pain to explore and discover the full story behind each log entry. By attaching context specific `ScopeIds` to entries, one can trivially filter through piles of trace and end up with only those entries that have a meaning in the current context.


# Getting started
NuGet package (https://www.nuget.org/packages/NLog.LoggingScope/) is available via NPM
```
install-package NLog.LoggingScope
```

# Main features
## Your log entries get enriched with `ScopeIds`
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
```
ScopeId                               ScopeName     Severity  Message
4e4e92ee-6c6c-48b0-8dc3-04d2cc4d1f31  MyScope       Debug     Hello
4e4e92ee-6c6c-48b0-8dc3-04d2cc4d1f31  MyScope       Debug     World!
f162e96b-9e64-4cc9-9e1d-f1105b32d204  AnotherScope  Info      Fizz
f162e96b-9e64-4cc9-9e1d-f1105b32d204  AnotherScope  Info      Buzz
```
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
```
ScopeId                               ScopeName  Severity  Message
3058f00b-7d09-4d70-9720-bbb7d5f6ac9a  Lorem      Trace     Ipsum
d98dc934-2675-4072-9b57-ced90a4071d6  Lorem      Trace     Ipsum
```

## Nesting scopes make them bound together with `ScopeIds`
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
```
ScopeId                               ScopeName  ParentScopeId                         Message
94bb82a3-7b63-4bb6-aa66-807f2a2d863d  TheParent                                        One
75ccddf9-d596-4ceb-b2ae-fe63b02b8b1b  TheChild   94bb82a3-7b63-4bb6-aa66-807f2a2d863d  Two
94bb82a3-7b63-4bb6-aa66-807f2a2d863d  TheParent                                        Three
```

These parent-child-connections form essentially a linked tree structure; the highest parent is the root. This provides nice options for *adjusting the focusing* one's log searches, focusing on smaller or larger contextes, whichever provides more insight.

## SQL-target
TODO

## NLog configuration
TODO

## Custom fields
TODO
