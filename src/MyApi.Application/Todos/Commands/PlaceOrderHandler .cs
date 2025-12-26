using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApi.Application.Todos.Commands.CreateTodo;
using System.Runtime.CompilerServices;
namespace MyApi.Application.Todos.Commands
{
    public record PlaceOrderCommand(
                int CustomerId,
                List<OrderItemDto> Items,
                string Currency
            ) : IRequest<Unit>;

    public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Unit>
    {
        private readonly IMediator _mediator;
        public PlaceOrderHandler(IMediator mediator) => _mediator = mediator;

        public async Task<Unit> Handle(PlaceOrderCommand request, CancellationToken ct)
        {
            await _mediator.Send(new CreateOrderCommand(), ct);
            await _mediator.Send(new ReserveInventoryCommand(), ct);
            await _mediator.Send(new CreatePaymentCommand(), ct);

            return Unit.Value;
        }
    }

}
