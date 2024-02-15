using System.Security.Claims;
using BlazorInvoiceApp.Data;
using BlazorInvoiceApp.DTOS;

namespace BlazorInvoiceApp.Repository;

public interface IInvoiceRepository : IGenericOwnedRepository<Invoice, InvoiceDTO>
{
    public Task<List<InvoiceDTO>> GetAllMineFlat(ClaimsPrincipal? User);
    public Task DeleteWithLineItems(ClaimsPrincipal? User, string invoiceId);
}