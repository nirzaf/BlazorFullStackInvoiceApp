namespace BlazorInvoiceApp.DTOS;

public class InvoiceTermsDTO : IDTO, IOwnedDTO
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = null!;
}