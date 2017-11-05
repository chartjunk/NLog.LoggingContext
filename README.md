# NLog.LoggingScope
`LoggingScope` aims to make it less of a pain to explore and discover the full story behind log entries. By attaching context specific `ScopeIds` to entries, one can trivially filter through piles of trace and end up with only those entries that have a meaning in the context that is under investigation.


# Getting started
NuGet package (https://www.nuget.org/packages/NLog.LoggingScope/) is available via NPM:
```
install-package NLog.LoggingScope
```

# Main features
## Your log entries get enriched with `ScopeIds`!
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
`ScopeIds` being unique for each scope, one is able to find out what happened in each block *during a single execution* by searching log entries by a `ScopeId`. A new `ScopeId` is generated for each instance of `LoggingScope`:
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

## Nested scopes are bound together using `ScopeIds`
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

Essentially these parent-child-connections form a linked tree structure in which the highest parent is the root. This provides options for *adjusting the focus* of one's log searches. One may search for log entries by only the lowest child's `ScopeId` or involve parents' `ScopeIds` to the search, broadening the focus.

# SQL target
`NLog.LoggingScope` makes it effortless to target one's logs to an SQL database table. The default log table schema is:
```SQL
CREATE TABLE mySchema.MyLogTable
(
    [Id] IDENTITY BIGINT PRIMARY KEY,
    [ScopeId] CHAR(36) NOT NULL,
    [ScopeName] VARCHAR(128),
    [Level] VARCHAR(16),
    [Message] NVARCHAR(MAX),
    [Exception] NVARCHAR(MAX),
    [InnerException] NVARCHAR(MAX),
    [ParentScopeId] CHAR(36),
    [TopmostParentScopeId] CHAR(36) NOT NULL
)
``` 
Logging to this table is enabled by adding the following NLog target to your NLog config:
```XML
<target xsi:type="DefaultLoggingScopeDbTarget" 
        name="SomeNameForMyTarget" 
        connectionString="Connection string"
        schemaTableName="mySchema.MyLogTable"
        dbProvider="A fully qualified name for the DB provider"/>
```
If preferred, `connectionStringName` attribute can be use instead of `connectionString`. If logging should target Microsoft SQL Server, `dbProvider` should be set to value `sqlserver`. If multiple `DefaultLoggingContextDbTargets` are used within the same database, one may use the *shared configuration* pattern by adding the following values to `App.config`:
```XML
<appSettings>
    <add key="NLog.LoggingScope:ConnectionString" value="Connection string"/>
    <add key="NLog.LoggingScope:DbProvider" value="A fully qualified name for the DB provider"/>
</appSettings>
```
In this case, the NLog target configuration reduces to:
```XML
<target xsi:type="DefaultLoggingScopeDbTarget" name="CommonLogTarget" schemaTableName="dbo.CommonLog"/>
<target xsi:type="DefaultLoggingScopeDbTarget" name="UserInterfaceLogTarget" schemaTableName="dbo.UILog"/>
```
Once again, for the shared configuration `NLog.LoggingScope:ConnectionStringName` may be used instead of `NLog.LoggingScope:ConnectionString`.

# Custom fields
TODO

# Custom target
TODO
