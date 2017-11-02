# NLog.LoggingScope

Available via NPM
```
install-package NLog.LoggingScope
```

### ENRICH your log entries with an ID that is uniform within a block of code

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
Resulting log entries:
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
