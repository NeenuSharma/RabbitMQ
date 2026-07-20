using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Infrastructure.Context;

namespace Order.Infrastructure.Repository;

public class OrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OrderData order)
    {
        _context.Orders.Add(order);

        await _context.SaveChangesAsync();
    }

    public async Task<OrderData?> GetAsync(int id)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}