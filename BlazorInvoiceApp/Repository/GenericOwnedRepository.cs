using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using BlazorInvoiceApp.Data;
using BlazorInvoiceApp.DTOS;
using Microsoft.EntityFrameworkCore;

namespace BlazorInvoiceApp.Repository;

public class GenericOwnedRepository<TEntity, TDTO>(ApplicationDbContext context, IMapper mapper)
    : IGenericOwnedRepository<TEntity, TDTO>
    where TEntity : class, IEntity, IOwnedEntity
    where TDTO : class, IDTO, IOwnedDTO
{
    protected readonly ApplicationDbContext context = context;
    
    protected string? getMyUserId(ClaimsPrincipal? User)
    {
        Claim? uid = User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        return uid?.Value;
    }

    public virtual async Task<List<TDTO>> GetAllMine(ClaimsPrincipal? User)
    {
        string? userid = getMyUserId(User);
        if (userid is null) return [];
        List<TEntity> entities = await context.Set<TEntity>()
            .Where(e => e.UserId == userid).ToListAsync();
        List<TDTO> result = mapper.Map<List<TDTO>>(entities);
        return result;
    }

    public virtual async Task<TDTO> GetMineById(ClaimsPrincipal? User, string id)
    {
        string? userid = getMyUserId(User);
        if (userid is null) return null!;
        TEntity? entity = await context.Set<TEntity>()
            .Where(entity => entity.Id == id && entity.UserId == userid)
            .FirstOrDefaultAsync();
        TDTO result = mapper.Map<TDTO>(entity);
        return result;
    }

    public virtual async Task<TDTO> UpdateMine(ClaimsPrincipal? User, TDTO dto)
    {
        string? userid = getMyUserId(User);
        if (userid is null) return null!;
        TEntity? toUpdate =
            await context.Set<TEntity>().Where(entity => entity.Id == dto.Id && entity.UserId == userid)
                .FirstOrDefaultAsync();

        if (toUpdate is null) return null!;
        mapper.Map(dto, toUpdate);
        context.Entry(toUpdate).State = EntityState.Modified;
        TDTO result = mapper.Map<TDTO>(toUpdate);
        return result;
    }

    public virtual async Task<bool> DeleteMine(ClaimsPrincipal? User, string id)
    {
        string? userid = getMyUserId(User);
        if (userid is null) return false;
        TEntity? entity = await context.Set<TEntity>().Where(entity => entity.Id == id && entity.UserId == userid)
            .FirstOrDefaultAsync();
        if (entity is null) return false;
        context.Remove(entity);
        return true;
    }

    public virtual async Task<string> AddMine(ClaimsPrincipal? User, TDTO dto)
    {
        string? userid = getMyUserId(User);
        if (userid is null) return null!;
        dto.UserId = userid;
        dto.Id = Guid.NewGuid().ToString();
        TEntity toAdd = mapper.Map<TEntity>(dto);
        await context.Set<TEntity>().AddAsync(toAdd);
        return toAdd.Id;
    }

    protected async Task<List<TDTO>> GenericQuery(ClaimsPrincipal? User, Expression<Func<TEntity, bool>>? expression,
        List<Expression<Func<TEntity, object>>>? includes)
    {
        string? userid = getMyUserId(User);
        if (userid is null) return new List<TDTO>();
        IQueryable<TEntity> query = context.Set<TEntity>().Where(e => e.UserId == userid);
        if (expression is not null)
            query = query.Where(expression);

        if (includes is not null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        List<TEntity> entities = await query.ToListAsync();
        List<TDTO> result = mapper.Map<List<TDTO>>(entities);
        return result;
    }
}