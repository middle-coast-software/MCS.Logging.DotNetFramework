# MCS.Logging.DotNetFramework
A logging library that uses Serilog for logging application data.

This library generalizes a collection of useful logging tools to reduce some of the repeat work associated with implementing Serilog in a project. It provides easy to use performance, usage, diagnostic, and exception logging methods. 

## Supported Serilog Sinks
The following are supported with drop in app.config entries (documentation pending as the package develops). They can be used individually or in combinations as you choose.
1. File
2. MS SQL Server

## General
The sink you wish to use are registered via a list entered as an appsetting entry with name "McsFileLogDestinationTypes".
```
<add key="McsFileLogDestinationTypes" value="<!-- File Sql -->\" />
```
The list may be either comma-delimited or space-delimited and capitalization does not matter. You can you one or more of the options. Including none will result in no logs being created but should not result in operational failure.

## Current Configurations
### File
File sinks are supported with a single app.config appsetting entry to specify the folder location in which log files should be placed.
```
<add key="LogFolderLocation" value="<!-- relative or absolute folder location goes here-->\" />
```
Four files will be published for each day. One each for performance, usage, diagnostic, and errors. Each of them will end with a date in the format MMddyyyy.

### SQL
MS Sql Logs are enabled with 2 app.config entries. A connection string named "LogConnection" and an appsetting to control the log batch size. 
```

<appSettings>
  ...
  <add key="LogBatchSize" value="1"/>
  ...
</appSettings>

<connectionStrings>
    <add connectionString="<!-- connection here -->" providerName="System.Data.SqlClient" name="LogConnection"/>
</connectionStrings>


```
Log batch size corresponds directly the option of the same name found in the Serilog sink library for MSSql. It corresponds to the number of log entries which will be held in memory before pushing them all to the log in a batch. This will default to 1 if not provided, but setting the number higher can relieve network pressure in cases of high throughput.
