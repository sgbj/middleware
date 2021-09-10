var pipeline = new Pipeline()
    .Use((context, next) =>
    {
        Console.WriteLine($"Middleware 1: {context.Data}");
        return next();
    })
    .Use((context, next) =>
    {
        Console.WriteLine($"Middleware 2: {context.Data}");
        return next();
    })
    .Run(context =>
    {
        Console.WriteLine($"Middleware 3: {context.Data}");
        return Task.CompletedTask;
    });

var app = pipeline.Build();

await app(new() { Data = "Example" });

class PipelineContext
{
    public string? Data { get; set; }
}

delegate Task PipelineDelegate(PipelineContext context);

class Pipeline
{
    private readonly List<Func<PipelineDelegate, PipelineDelegate>> _components = new();

    public Pipeline Use(Func<PipelineDelegate, PipelineDelegate> middleware)
    {
        _components.Add(middleware);
        return this;
    }

    public Pipeline Use(Func<PipelineContext, Func<Task>, Task> middleware)
    {
        return Use(next =>
        {
            return context =>
            {
                return middleware(context, () => next(context));
            };
        });
    }

    public Pipeline Use(Func<PipelineContext, PipelineDelegate, Task> middleware)
    {
        return Use(next => context => middleware(context, next));
    }

    public Pipeline Run(PipelineDelegate handler)
    {
        return Use(_ => handler);
    }

    public PipelineDelegate Build()
    {
        PipelineDelegate app = context => Task.CompletedTask;

        for (var c = _components.Count - 1; c >= 0; c--)
        {
            app = _components[c](app);
        }

        return app;
    }
}
