using FluentValidation;
using MediatR;

/*
 * It is an implementation of a pipeline behavior in the MediatR library.
 * Pipeline behaviors allow us to inject logic into the MediatR request pipeline,
 * meaning they can perform actions before and/or after a request is handled by a request handler.
   
   The ValidationBehavior is responsible for validating incoming requests 
   before they are passed to the corresponding handlers (eg: addcommandhandler).
 * 
 */


namespace CleanArchitecture.Locations.Infrastructure.Behavior;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // Perform actions before the request is handled by the next handler
    // Modify or halt the execution of the pipeline
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Check if validator exists: If the validator is null,
        // the method immediately calls next() to proceed with the request handling without validation.
        if (validator == null)
            return await next();

        // Validate the request: If a validator exists, it runs the validation asynchronously
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        // If validation passes (validationResult.IsValid == true),
        // it calls next() to let the request proceed to the handler.
        // If validation fails, it throws a ValidationException with the validation errors
        if (validationResult.IsValid)
            return await next();

        // If validation fails, a FluentValidation.ValidationException is thrown.
        // This is caught by the global exception handler in the middleware,
        // which formats and sends an appropriate error response to the client.
        throw new ValidationException(validationResult.Errors);
    }
}