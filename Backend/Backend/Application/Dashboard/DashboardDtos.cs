namespace Backend.Application.Dashboard;

public sealed record DashboardSummaryDto(decimal TodaySales, int TotalInvoices);
public sealed record SalesPointDto(DateTime Date, decimal Total);
public sealed record MonthlyPointDto(string YearMonth, decimal Total);

