# NLog.LoggingScope
[![NuGet](https://img.shields.io/nuget/dt/NLog.LoggingScope.svg)](https://www.nuget.org/packages/NLog.LoggingScope)

`LoggingScope` aims to make it less of a pain to explore and discover the full story behind log entries. By attaching context specific `ScopeIds` to entries, one can trivially filter through piles of trace and end up with only those entries that have a meaning in the context under investigation.


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
Since `ScopeIds` are unique for each scope, it is possible to figure out what happened in each block *during a single execution* by searching log entries by a `ScopeId`. A new `ScopeId` is generated for each instance of `LoggingScope`:
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

Essentially these parent-child-connections form a linked tree structure in which the highest parent is the root. This provides options for *adjusting the focus* of log searches. Log entries can be searched by only the `ScopeId` of the lowest child, or involve `ScopeIds` of the parents to the search, which broadens the focus.

# SQL target
`NLog.LoggingScope` makes it effortless to target logging to a SQL database table. The default log table schema is:
```SQL
CREATE TABLE mySchema.MyLogTable
(
    [Id] BIGINT IDENTITY PRIMARY KEY,
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
Logging to this table is enabled by adding the following NLog target to the NLog config:
```XML
<target xsi:type="DefaultLoggingScopeDbTarget" 
        name="SomeNameForMyTarget" 
        connectionString="Connection string"
        schemaTableName="mySchema.MyLogTable"
        dbProvider="A fully qualified name for the DB provider"/>
```
If preferred, `connectionStringName` attribute can be used instead of `connectionString`. If Microsoft SQL Server is used, `dbProvider` should be set to value `sqlserver`. If multiple `DefaultLoggingContextDbTargets` are used within the same database, configuration can be shared between targets by adding the following values to `App.config`:
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
Once again, for the shared configuration, `NLog.LoggingScope:ConnectionStringName` may be used instead of `NLog.LoggingScope:ConnectionString`.

# Custom targets
`LoggingScope` provides tools for creating customised target for different scenarios. For instance, if information about the user of an application is wanted to be included to the trace, the following target can be inherited from the default target:

```C#
using NLog.Targets;

[Target("UserDbLoggingScopeTarget")]
public UserLoggingTarget : DefaultLoggingContextDbTarget
{
  public UserLoggingTarget()
  {
    AddGdcColumn("AD_UserName");
  }
}
```

Let's assume that the target is declared in an assembly called `MyApp.MyAssembly`. The target can be used in `NLog.config` by adding the assembly as an extension:
```XML
<nlog>
  <extensions>
    <add assembly="MyApp.MyAssembly"/>
  </extensions>

  <targets>
    <target xsi:type="UserDbLoggingScopeTarget" name="MyUserLoggingTarget" schemaTableName="dbo.UserLog"/>
  </targets>
  
  <rules>
    <logger name="*" levels="Trace,Debug,Fatal,Error,Warn,Info" writeTo="MyUserLoggingTarget"/>
  </rules>
</nlog>
```

This implies that there should be the following table in the database:
```SQL
CREATE TABLE dbo.UserLog
(
    [Id] BIGINT IDENTITY PRIMARY KEY,
    [ScopeId] CHAR(36) NOT NULL,
    [ScopeName] VARCHAR(128),
    [Level] VARCHAR(16),
    [Message] NVARCHAR(MAX),
    [Exception] NVARCHAR(MAX),
    [InnerException] NVARCHAR(MAX),
    [ParentScopeId] CHAR(36),
    [TopmostParentScopeId] CHAR(36) NOT NULL,
    [AD_UserName] VARCHAR(128)
)
```

Now, the `AD_UserName` is logged whenever it is declared for the current scope with `.Set` method:

```C#
var currentAdUserName = 'who.ever@corporation.com'
using(new LoggingScope("MoneyMakingApp").Set("AD_UserName", currentAdUserName))
{
  Logger.Trace("Business as usual");
}
```
which leads to the following log entry:
```
ScopeId                               ScopeName       Message            AD_UserName
59c8369d-e8bd-4478-9221-4888f28abe97  MoneyMakingApp  Business as usual  who.ever@corporation.com
```
There are also strongly typed alternatives for the mentioned methods:
```C#
AddGdcColumn<UserLog>(a => a.AD_UserName)
// and
new LoggingScope("MoneyMakingApp").WithSchema<UserLog>(w => w.Set(s => s.AD_UserName, currentAdUserName))
```
