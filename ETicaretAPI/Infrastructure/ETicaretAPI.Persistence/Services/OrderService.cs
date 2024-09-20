using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories.CompletedOrderRepository;
using ETicaretAPI.Application.Repositories.OrderRepository;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompletedOrderWriteRepository completedOrderWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            var orderCode = (new Random().NextDouble() * 10000).ToString();
            orderCode = orderCode.Substring(orderCode.IndexOf(".") + 1, (orderCode.Length - orderCode.IndexOf(".")) - 1);

            await _orderWriteRepository.AddAsync(new()
            {
                Address = createOrder.Address,
                Id = Guid.Parse(createOrder.BasketId),
                Description = createOrder.Description,
                OrderCode = orderCode
            });

            await _orderWriteRepository.SaveAsync();
        }

        public async Task<ListOrder> GetAllOrdersAsync(int page, int size)
        {
            var query = _orderReadRepository.Table
                             .Include(o => o.Basket)
                             .ThenInclude(b => b.User)
                             .Include(o => o.Basket)
                                 .ThenInclude(b => b.BasketItems)
                                 .ThenInclude(bi => bi.Product);

            var data = query.Skip(page * size).Take(size);

            var data2 = from order in data
                        join completedOrder in _completedOrderReadRepository.Table
                           on order.Id equals completedOrder.OrderId into co
                        from _co in co.DefaultIfEmpty()
                        select new
                        {
                            order.Id,
                            order.CreatedDate,
                            order.OrderCode,
                            order.Basket,
                            Completed = _co != null ? true : false
                        };

            return new()
            {
                TotalOrderCount = await query.CountAsync(),
                Orders = await data2.
                            Select(o => new
                            {
                                Id = o.Id,
                                OrderCode = o.OrderCode,
                                CreatedDate = o.CreatedDate,
                                TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                                UserName = o.Basket.User.UserName,
                                Completed = o.Completed
                            }).
                            ToListAsync()
            };
        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = _orderReadRepository.Table
                               .Include(o => o.Basket)
                                     .ThenInclude(b => b.BasketItems)
                                          .ThenInclude(bi => bi.Product);
            var data2 = await (from order in data
                               join completedOrder in _completedOrderReadRepository.Table
                                   on order.Id equals completedOrder.OrderId into co
                               from _co in co.DefaultIfEmpty()
                               select new
                               {
                                   order.Id,
                                   order.CreatedDate,
                                   order.OrderCode,
                                   order.Basket,
                                   Completed = _co != null ? true : false,
                                   order.Address,
                                   order.Description
                               }).FirstOrDefaultAsync(o => o.Id == Guid.Parse(id)); ;

            return new()
            {
                Address = data2.Address,
                BasketItems = data2.Basket.BasketItems.Select(bi => new
                {
                    bi.Product.Name,
                    bi.Product.Price,
                    bi.Quantity
                }),
                OrderCode = data2.OrderCode,
                CreatedDate = data2.CreatedDate,
                Id = data2.Id.ToString(),
                Description = data2.Description,
                Completed = data2.Completed
            };
        }

        public async Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.Table
                                    .Include(o => o.Basket)
                                    .ThenInclude(b => b.User)
                                    .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));
            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new()
                {
                    OrderId = Guid.Parse(id)
                });
                return (await _completedOrderWriteRepository.SaveAsync() > 0, new()
                {
                    OrderCode = order.OrderCode,
                    OrderDate = order.CreatedDate,
                    UserNameSurname = order.Basket.User.NameSurname,
                    Email = order.Basket.User.Email
                });
            }
            return (false, null);
        }
    }
}
