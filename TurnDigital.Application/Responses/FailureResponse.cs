using TurnDigital.Application.Responses.Enums;

namespace TurnDigital.Application.Responses;

public abstract class FailureResponse
{
    public abstract State State { get; }

    public abstract object GetResponseData();
}

public class FailureResponse<T> : FailureResponse 
{
    private readonly State _state;

    private readonly T _responseData;

    private FailureResponse(State state, T responseData)
    {
        _state = state;
        _responseData = responseData;
    }
    
    /// <summary>
    /// Creates an unauthorized instance of failure response.
    /// </summary>
    /// <returns>An instance of failure response with the state of unauthorized.</returns>

    public static FailureResponse<T> Unauthorized(T responseData) => new (State.Unauthorized, responseData);
    
    /// <summary>
    /// Creates a not found instance of failure response.
    /// </summary>
    /// <returns>An instance of failure response with the state of not found.</returns>
    public static FailureResponse<T> NotFound(T responseData) => new (State.NotFound, responseData);
    
    /// <summary>
    /// Creates a validation failure instance of failure response.
    /// </summary>
    /// <returns>An instance of failure response with the state of validation failure.</returns>
    public static FailureResponse<T> ValidationFailure(T responseData) => new (State.ValidationFailure, responseData);
    
    /// <summary>
    /// Creates an internal server error instance of failure response.
    /// </summary>
    /// <returns>An instance of failure response with the state of server error.</returns>
    public static FailureResponse<T> InternalServerError(T responseData) => new (State.InternalError, responseData);
    
    /// <summary>
    /// Creates a too many requests instance of failure response.
    /// </summary>
    /// <returns>An instance of failure response with the state of too many requests.</returns>
    public static FailureResponse<T> TooManyRequests(T responseData) => new (State.TooManyRequests, responseData);

    public override State State => _state;

    public override object GetResponseData() => ResponseData;

    public T ResponseData => _responseData;
}