using MediatR;
using BookStation.Application.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookStation.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Bỏ qua query — chỉ command mới cần SaveChanges
        if (typeof(TRequest).Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
            return await next();

        // Với command, wrap trong transaction/SaveChanges
        var response = await next();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return response;
    }
}