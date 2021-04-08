# AspNetCore.ExceptionHandling Documentation
(c) 2020-2021 Singtone. All rights reserved.

## Overview
This library is a common exception handling module for ASP.NET Core.

## Install

Install as a NuGet package: [AspNetCore.ExceptionHandling](https://www.nuget.org/packages/AspNetCore.ExceptionHandling)

## Usage

### `UseApiExceptionHandler()`
### `UseApiExceptionHandler(logger)`
### `UseApiExceptionHandler(errorHandlingPath, logger)`
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseApiExceptionHandler("/Error", logger);
        app.UseHsts();
    }

    ...
}
```
Act as a global "catch-all" exception handler, to catch any uncaught exceptions and return http code 500. If a logger is passed in as parameter the exception details will be logged.
* `logger: ILogger`: The logger will be used to log exception details.
* `errorHandlingPath: string`: The default error handling path, such as a default api route or the default error web page.

#### Things to note:
* When the parameter `errorHandlingPath` is present, the function will return 500 when the request is Ajax request or has Json content; For other requests, mostly likely html, binary etc, the requests will be routed to the errorHandlingPath.

### `UseExceptionHandling()`
### `UseApiExceptionHandling()`
```
public void Configure(IApplicationBuilder app)
{
    ...
    app.UseHttpsRedirection();
    app.UseRouting();
    
    app.UseApiExceptionHandling();

    ...
}
```
Act as a ASP.NET Core Middleware to handle the exceptions as catch-all. `ILoggerFactory` is required via dependency injection and the exception details will always be logged.
* `UseExceptionHandling`: Http 500 will be returned for any request encountering exceptions.
* `UseApiExceptionHandling`: Http 500 will be returned only when a request is Ajax request or has Json content; Other requests will not be handled and will be passed to the next middleware.
