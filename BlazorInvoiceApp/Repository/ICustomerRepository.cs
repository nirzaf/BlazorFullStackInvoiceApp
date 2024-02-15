using System.Security.Claims;
using BlazorInvoiceApp.Data;
using BlazorInvoiceApp.DTOS;

namespace BlazorInvoiceApp.Repository;

public interface ICustomerRepository : IGenericOwnedRepository<Customer, CustomerDTO>
{
    public Task Seed(ClaimsPrincipal? User);
}