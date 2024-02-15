using AutoMapper;
using BlazorInvoiceApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlazorInvoiceApp.Repository;

public class RepositoryCollection : IRepositoryCollection
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;


    public RepositoryCollection(IDbContextFactory<ApplicationDbContext> dbFactory, IMapper mapper)
    {
        context = dbFactory.CreateDbContext();
        this.mapper = mapper;
        Invoice = new InvoiceRepository(context, mapper);
        Customer = new CustomerRepository(context, mapper);
        InvoiceTerms = new InvoiceTermsRepository(context, mapper);
        InvoiceLineItem = new InvoiceLineItemRepository(context, mapper);
    }

    public IInvoiceRepository Invoice { get; }
    public ICustomerRepository Customer { get; }
    public IInvoiceTermsRepository InvoiceTerms { get; }
    public IInvoiceLineItemRepository InvoiceLineItem { get; }

    public void Dispose()
    {
        context.Dispose();
    }

    public async Task<int> Save()
    {
        try
        {
            return await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            foreach (EntityEntry item in ex.Entries)
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.CurrentValues.SetValues(item.OriginalValues);
                        item.State = EntityState.Unchanged;
                        throw new RepositoryUpdateException();
                    case EntityState.Deleted:
                        item.State = EntityState.Unchanged;
                        throw new RepositoryDeleteException();
                    case EntityState.Added:
                        throw new RepositoryAddException();
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        return 0;
    }
}