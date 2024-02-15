using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlazorInvoiceApp.Data;

public class InvoiceTerms : IEntity, IOwnedEntity
{
    public IdentityUser? User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;
}