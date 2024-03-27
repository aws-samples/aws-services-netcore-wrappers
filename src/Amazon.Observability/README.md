# App

This is a nuget package to provide functionality of sending logs and metrics and Amazon Cloudwatch service, traces are sent to AWS X-Ray service.

# Usage
builder.Services
    .AddOtelInstrumentation(builder.Configuration)
    .AddAspNetCoreInstrumentationSupport() //This is optional and used if the consumer is an ASP.NET Core application.
    .AddTraceSampling() //This is optional and used if head sampling needs to be done with the traces
    .Build();
