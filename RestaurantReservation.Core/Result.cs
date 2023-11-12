using System.Runtime.Serialization;

public class Result : IDisposable
{
    protected Result()
    {
    }

    public static Result PrepareFailure(string message, params string[] parameters)
    {
        return new Result
        {
            Message = message,
            Params = parameters
        };
    }

    public static Result PrepareFailure(string message)
    {
        return new Result
        {
            Message = message
        };
    }

    public static Result PrepareSuccess()
    {
        return new Result
        {
            Success = true,
            Message = ""
        };
    }

    public static Result PrepareSuccess(string message)
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    public static Result PrepareResult(Result result)
    {
        return new Result
        {
            Success = result.Success,
            Message = result.Message
        };
    }

    public bool Success { get; set; }
    [IgnoreDataMember]
    public bool Failed => !Success;
    public string Message { get; set; }

    public string[] Params { get; set; }

    ///////////////////////////////////////////////////////////////////////////////////////////
    //Dispose
    ///////////////////////////////////////////////////////////////////////////////////////////

    // Flag: Has Dispose already been called?
    protected bool Disposed;

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool aDisposing)
    {
        try
        {
            if (Disposed)
                return;

            if (aDisposing)
            {

            }

            // Free any unmanaged objects here.
            //
            Disposed = true;
        }
        catch
        {
            // ignored
        }
    }

    ~Result()
    {
        Dispose(false);
    }
}

public class Result<T> : Result
{
    private Result()
    {
        Message = "";
        Payload = default;
    }

    public Result(Result result)
    {
        Success = result.Success;
        Message = result.Message;
        Params = result.Params;
        Payload = default;
    }

    public new static Result<T> PrepareFailure(string message, params string[] param)
    {
        return new Result<T>
        {
            Message = message,
            Payload = default,
            Params = param
        };
    }

    public static Result<T> PrepareFailure(Result result)
    {
        return new Result<T>
        {
            Message = result.Message,
            Payload = default,
            Params = result.Params
        };
    }

    public static Result<T> PrepareFailure(string message, T payload, params string[] parameters)
    {
        return new Result<T>
        {
            Message = message,
            Payload = payload,
            Params = parameters
        };
    }

    public static Result<T> PrepareSuccess(T payload)
    {
        return new Result<T>
        {
            Success = true,
            Message = "",
            Payload = payload
        };
    }

    public new static Result<T> PrepareResult(Result result)
    {
        return new Result<T>
        {
            Success = result.Success,
            Message = result.Message,
            Payload = default
        };
    }

    public static Result<T> PrepareResult(Result result, T payload)
    {
        return new Result<T>
        {
            Success = result.Success,
            Message = result.Message,
            Payload = payload
        };
    }

    public T Payload { get; set; }

    ///////////////////////////////////////////////////////////////////////////////////////////
    //Dispose
    ///////////////////////////////////////////////////////////////////////////////////////////

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        try
        {
            if (Disposed)
                return;

            if (disposing)
            {
                if (!typeof(T).IsPrimitive)
                {
                    var vDisposable = Payload as IDisposable;
                    vDisposable?.Dispose();
                }
            }

            // Free any unmanaged objects here.
            //
            Disposed = true;
        }
        catch
        {
            // ignored
        }
    }

    ~Result()
    {
        Dispose(false);
    }
}