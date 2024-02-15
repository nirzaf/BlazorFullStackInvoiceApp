using System.Security.Claims;
using AutoMapper;
using BlazorInvoiceApp.Data;
using BlazorInvoiceApp.DTOS;
using Microsoft.EntityFrameworkCore;

namespace BlazorInvoiceApp.Repository;

public class CustomerRepository(ApplicationDbContext context, IMapper mapper)
    : GenericOwnedRepository<Customer, CustomerDTO>(context, mapper), ICustomerRepository
{
    public async Task Seed(ClaimsPrincipal? User)
    {
        string? userid = getMyUserId(User);
        if (userid is not null)
        {
            int count = await context.Customers
                .Where(c => c.UserId == userid).CountAsync();
            if (count == 0)
            {
                // seed some data.
                CustomerDTO defaultCustomer = new()
                {
                    Name = "My First Customer"
                };
                await AddMine(User, defaultCustomer);
            }
        }
    }
}