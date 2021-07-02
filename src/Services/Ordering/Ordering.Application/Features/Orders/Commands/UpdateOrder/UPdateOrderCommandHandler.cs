﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UPdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UPdateOrderCommandHandler> _logger;

        public UPdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<UPdateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
            if(orderToUpdate == null)
            {
                //_logger.LogError("Order doest not exist on database");
                throw new NotFoundException(nameof(Order), request.Id);
            }

           await _orderRepository.UpdateAsync(_mapper.Map<Order>(orderToUpdate));
            _logger.LogError("Order {orderToUpdate.Id} is successfully updated");
            return Unit.Value;
        }
    }
}
