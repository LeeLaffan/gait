namespace Gait.Utils;

public readonly struct Result<TSuccess, TError>
{
    private readonly TSuccess? _success;
    private readonly TError? _error;
    private readonly bool _isSuccess;

    private Result(TSuccess success)
    {
        _success = success;
        _error = default;
        _isSuccess = true;
    }

    private Result(TError error)
    {
        _success = default;
        _error = error;
        _isSuccess = false;
    }

    public bool IsSuccess => _isSuccess;

    public bool IsError => !_isSuccess;

    public TSuccess Value
    {
        get
        {
            if (!_isSuccess)
                throw new InvalidOperationException("Cannot access Success value when result is an error");
            return _success!;
        }
    }

    public TError Error
    {
        get
        {
            if (_isSuccess)
                throw new InvalidOperationException("Cannot access Error value when result is a success");
            return _error!;
        }
    }

    public static Result<TSuccess, TError> Ok(TSuccess success) => new(success);
    public static Result<TSuccess, TError> Fail(TError error) => new(error);

    public static implicit operator Result<TSuccess, TError>(TSuccess success) => Ok(success);

    public static implicit operator Result<TSuccess, TError>(TError error) => Fail(error);

    public void Match(Action<TSuccess> onSuccess, Action<TError> onError)
    {
        if (_isSuccess)
            onSuccess(_success!);
        else
            onError(_error!);
    }

    public TResult Match<TResult>(Func<TSuccess, TResult> onSuccess, Func<TError, TResult> onError) =>
        _isSuccess ? onSuccess(_success!) : onError(_error!);
}
