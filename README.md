# MCS.Logging.DotNetFramework
A logging library that uses Serilog for logging application data.

Documentation is in no way complete at this time, but this should get you started until completeness can be approached (if not fully achieved...alas software is software).

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
The list may be either comma-delimited or space-delimited and capitalization does not matter. You can use one or more of the options. Including none will result in no logs being created but should not result in operational failure.

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

## How To Actually Log Sh*...stuff
Now for the good stuff, it's time to hook all the pipes together.
In your startup class you'll need to register a global exception handler. 

For an API this is pretty simple. Unless you for some reason need to log performance or usage, but we'll get to that.
#### Error Logging

In the .Net Framework version of this library, logging is chiefly done through static classes.
Just toss your errors into a logdetail object like the one above, and jam that into the McsLogger's WriteError method.


```
// error log detail
var eld = new LogDetail(){};

McsLogger.WriteError(eld);

```

#### Performance Tracking
Perf logging is really the only bit that needs some finesse here. I suggest perhaps writing it into a wrapper class if possible. 
The general stratefy is to new up a PerfTracker object (it starts measuring when this occurs), then perform your logic, then tell the PerfTracker to stop. 
It will save a log when the object is told to stop.

'''

var pt = new PerfTracker("performance metric name", "userId if available", "UserName if available",
                "Location", "Product", "Layer");


        ...perform some logic



pt.Stop();

'''

Note that the input parameters to the PerfTracker object match with properties of the general LogDetail object. This should be useful to build into a reusable code segment.

#### Usage Tracking
You should be able to add a method attribute to your controller classes.
Something like this should do it.
'''

'''


#### Diagnostic Logging
Diagnostic logging is supported with static helper that you can add wherever needed. Note that it does need an HttpContext, so it's not meant to be put just anywhere, but we are talking about web apps here so it didn't feel like a stretch.
'''

    McsLogger.WriteDiagnostic(ld);

'''