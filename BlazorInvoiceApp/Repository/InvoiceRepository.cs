using System.Security.Claims;
using AutoMapper;
using BlazorInvoiceApp.Data;
using BlazorInvoiceApp.DTOS;
using Microsoft.EntityFrameworkCore;

namespace BlazorInvoiceApp.Repository;

public class InvoiceRepository(ApplicationDbContext context, IMapper mapper)
    : GenericOwnedRepository<Invoice, InvoiceDTO>(context, mapper), IInvoiceRepository
{
    public async Task DeleteWithLineItems(ClaimsPrincipal? User, string invoiceId)
    {
        string? userid = getMyUserId(User);
        var lineItems = await context.InvoicesLineItems.Where(i => i.InvoiceId == invoiceId && i.UserId == userid)
            .ToListAsync();
        foreach (InvoiceLineItem lineItem in lineItems) context.InvoicesLineItems.Remove(lineItem);

        Invoice? invoice = await context.Invoices.Where(i => i.Id == invoiceId && i.UserId == userid)
            .FirstOrDefaultAsync();
        if (invoice != null) context.Invoices.Remove(invoice);
    }

    public async Task<List<InvoiceDTO>> GetAllMineFlat(ClaimsPrincipal? User)
    {
        string? userid = getMyUserId(User);
        var q = context.Invoices.Where(i => i.UserId == userid)
            .Include(i => i.InvoiceLineItems)
            .Include(i => i.InvoiceTerms)
            .Include(i => i.Customer)
            .Select(i => new InvoiceDTO
            {
                Id = i.Id,
                CreateDate = i.CreateDate,
                InvoiceNumber = i.InvoiceNumber,
                Description = i.Description,
                CustomerId = i.CustomerId,
                CustomerName = i.Customer!.Name,
                InvoiceTermsId = i.InvoiceTermsId,
                InvoiceTermsName = i.InvoiceTerms!.Name,
                Paid = i.Paid,
                Credit = i.Credit,
                TaxRate = i.TaxRate,
                UserId = i.UserId,
                InvoiceTotal = i.InvoiceLineItems.Sum(a => a.TotalPrice)
            });
        
        List<InvoiceDTO> results = await q.ToListAsync();
        return results;
    }
}