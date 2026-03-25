using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Backend.Domain.Entities;
using Backend.Infrastructure.Db;

namespace Backend.Infrastructure.Repositories;

public sealed record InvoiceListResponse(int TotalCount, IReadOnlyList<Invoice> Items);
public sealed record InvoiceDetailsResponse(Invoice Invoice, IReadOnlyList<InvoiceItem> Items);

public interface IInvoicesRepository
{
    Task<(int InvoiceId, string InvoiceNo)> CreateAsync(
        int? customerId,
        string? customerName,
        string? mobile,
        string? discountType,
        decimal? discountValue,
        decimal paidAmount,
        string? paymentMode,
        string? notes,
        DataTable itemsTable,
        CancellationToken cancellationToken);

    Task<InvoiceListResponse> ListAsync(string? invoiceNo, string? customer, string? mobile, int page, int pageSize, CancellationToken cancellationToken);
    Task<InvoiceDetailsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
}

public sealed class InvoicesRepository(IDbConnectionFactory connectionFactory) : IInvoicesRepository
{
    public async Task<(int InvoiceId, string InvoiceNo)> CreateAsync(
        int? customerId,
        string? customerName,
        string? mobile,
        string? discountType,
        decimal? discountValue,
        decimal paidAmount,
        string? paymentMode,
        string? notes,
        DataTable itemsTable,
        CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("CustomerId", customerId);
        parameters.Add("CustomerName", customerName);
        parameters.Add("Mobile", mobile);
        parameters.Add("DiscountType", discountType);
        parameters.Add("DiscountValue", discountValue);
        parameters.Add("PaidAmount", paidAmount);
        parameters.Add("PaymentMode", paymentMode);
        parameters.Add("Notes", notes);

        parameters.Add(
            "Items",
            itemsTable.AsTableValuedParameter("dbo.InvoiceItemType")
        );


        var row = await connection.QuerySingleAsync(
            new CommandDefinition(
                "[dbo].[spInvoices_Create]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return ((int)row.InvoiceId, (string)row.InvoiceNo);
    }

    public async Task<InvoiceListResponse> ListAsync(string? invoiceNo, string? customer, string? mobile, int page, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                "[dbo].[spInvoices_List]",
                new { InvoiceNo = invoiceNo, Customer = customer, Mobile = mobile, Page = page, PageSize = pageSize },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        var total = await multi.ReadSingleAsync<int>();
        var items = (await multi.ReadAsync<Invoice>()).AsList();
        return new InvoiceListResponse(total, items);
    }

    public async Task<InvoiceDetailsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                "[dbo].[spInvoices_GetById]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        var invoice = await multi.ReadSingleOrDefaultAsync<Invoice>();
        if (invoice is null) return null;
        var items = (await multi.ReadAsync<InvoiceItem>()).AsList();
        return new InvoiceDetailsResponse(invoice, items);
    }
}
