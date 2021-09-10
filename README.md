# Middleware

A basic middleware pipeline based on [ASP.NET Core Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/).

See ASP.NET Core's [ApplicationBuilder](https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http/src/Builder/ApplicationBuilder.cs), [UseExtensions](https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http.Abstractions/src/Extensions/UseExtensions.cs), and [RunExtensions](https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http.Abstractions/src/Extensions/RunExtensions.cs) classes for reference.

Code:
```csharp
var pipeline = new Pipeline()
   .Use((context, next) =>
   {
       Console.WriteLine($"Middleware 1: {context.Data++}");
       return next();
   })
   .Use((context, next) =>
   {
       Console.WriteLine($"Middleware 2: {context.Data++}");
       return next();
   })
   .Run(context =>
   {
       Console.WriteLine($"Middleware 3: {context.Data++}");
       return Task.CompletedTask;
   });

var app = pipeline.Build();

await app(new());
```

Output:
```
Middleware 1: 0
Middleware 2: 1
Middleware 3: 2
```
