using System.Diagnostics.CodeAnalysis;

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

    public bool IsSuccess([NotNullWhen(true)] out TSuccess? success, [NotNullWhen(false)] out TError? error)
    {
        if (_isSuccess)
        {
            success = _success!;
            error = default;
            return true;
        }

        success = default;
        error = _error!;
        return false;
    }

    public bool IsError([NotNullWhen(false)] out TSuccess? success, [NotNullWhen(true)] out TError? error) =>
        !IsSuccess(out success, out error);

    public static Result<TSuccess, TError> Ok(TSuccess success) => new(success);
    public static Result<TSuccess, TError> Fail(TError error) => new(error);

    public static implicit operator Result<TSuccess, TError>(TSuccess success) => Ok(success);

    public static implicit operator Result<TSuccess, TError>(TError error) => Fail(error);
}
