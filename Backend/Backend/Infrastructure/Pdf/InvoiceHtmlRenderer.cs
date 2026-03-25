using System.Net;
using System.Text;
using Backend.Domain.Entities;

namespace Backend.Infrastructure.Pdf;

public static class InvoiceHtmlRenderer
{
    public static string Render(Setting setting, Invoice invoice, IReadOnlyList<InvoiceItem> items)
    {
        static string E(string? s) => WebUtility.HtmlEncode(s ?? "");
        static string Money(decimal v) => v.ToString("0.00");

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html><head><meta charset=\"utf-8\"/>");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/>");
        sb.AppendLine("<title>Cash Memo</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
            :root{ --ink:#111827; --muted:#6b7280; --line:#e5e7eb; }
            *{ box-sizing:border-box; }
            body{ font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin:0; color:var(--ink); }
            .page{ padding:24px; }
            .header{ display:flex; justify-content:space-between; align-items:flex-start; gap:16px; border-bottom:2px solid var(--ink); padding-bottom:12px; }
            .shop h1{ margin:0; font-size:26px; letter-spacing:0.2px; }
            .shop .sub{ margin-top:4px; font-size:12px; color:var(--muted); line-height:1.4; }
            .title{ text-align:center; font-weight:800; letter-spacing:2px; margin:14px 0 10px; }
            .grid{ display:grid; grid-template-columns:1.2fr 0.8fr; gap:12px; }
            .box{ border:1px solid var(--line); border-radius:10px; padding:12px; }
            .kv{ display:grid; grid-template-columns:120px 1fr; row-gap:6px; column-gap:10px; font-size:12px; }
            .kv .k{ color:var(--muted); }
            table{ width:100%; border-collapse:collapse; margin-top:12px; font-size:12px; }
            thead th{ text-align:left; padding:10px 8px; border-bottom:1px solid var(--line); color:var(--muted); font-weight:700; }
            tbody td{ padding:10px 8px; border-bottom:1px solid var(--line); vertical-align:top; }
            .num{ text-align:right; white-space:nowrap; }
            .summary{ margin-top:12px; display:flex; justify-content:flex-end; }
            .summary .box{ width:320px; }
            .sumrow{ display:flex; justify-content:space-between; font-size:12px; padding:6px 0; }
            .sumrow.total{ font-weight:800; border-top:1px solid var(--line); margin-top:6px; padding-top:10px; }
            .footer{ margin-top:18px; display:grid; grid-template-columns:1fr 220px; gap:16px; align-items:end; }
            .terms{ font-size:11px; color:var(--muted); white-space:pre-wrap; }
            .sign{ border-top:1px solid var(--line); padding-top:10px; text-align:center; font-size:12px; color:var(--muted); }
        ");
        sb.AppendLine("</style></head><body><div class=\"page\">");

        sb.AppendLine("<div class=\"header\">");
        sb.AppendLine("<div class=\"shop\">");
        sb.AppendLine($"<h1>{E(setting.ShopName)}</h1>");
        sb.AppendLine($"<div class=\"sub\">{E(setting.Address)}<br/>{E(setting.Phone)}</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"box\" style=\"padding:10px 12px;\">");
        sb.AppendLine("<div class=\"kv\">");
        sb.AppendLine($"<div class=\"k\">Invoice No</div><div><b>{E(invoice.InvoiceNo)}</b></div>");
        sb.AppendLine($"<div class=\"k\">Date</div><div>{E(invoice.CreatedAt?.ToString("dd-MMM-yyyy HH:mm"))}</div>");
        sb.AppendLine("</div></div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"title\">CASH MEMO</div>");

        sb.AppendLine("<div class=\"grid\">");
        sb.AppendLine("<div class=\"box\">");
        sb.AppendLine("<div style=\"font-weight:800; margin-bottom:8px;\">Customer</div>");
        sb.AppendLine("<div class=\"kv\">");
        sb.AppendLine($"<div class=\"k\">Name</div><div>{E(invoice.CustomerName)}</div>");
        sb.AppendLine($"<div class=\"k\">Mobile</div><div>{E(invoice.Mobile)}</div>");
        sb.AppendLine("</div></div>");

        sb.AppendLine("<div class=\"box\">");
        sb.AppendLine("<div style=\"font-weight:800; margin-bottom:8px;\">Payment</div>");
        sb.AppendLine("<div class=\"kv\">");
        sb.AppendLine($"<div class=\"k\">Mode</div><div>{E(invoice.PaymentMode)}</div>");
        sb.AppendLine($"<div class=\"k\">Paid</div><div>{Money(invoice.PaidAmount ?? 0)}</div>");
        sb.AppendLine("</div></div>");
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<table>");
        sb.AppendLine("<thead><tr>");
        sb.AppendLine("<th>Item</th><th>Model</th><th>Serial</th><th class=\"num\">Qty</th><th class=\"num\">Rate</th><th class=\"num\">Discount</th><th class=\"num\">Amount</th>");
        sb.AppendLine("</tr></thead><tbody>");

        foreach (var it in items)
        {
            var discLabel = string.IsNullOrWhiteSpace(it.DiscountType) || it.DiscountValue is null
                ? ""
                : it.DiscountType == "%" ? $"{it.DiscountValue:0.##}%" : $"{Money(it.DiscountValue.Value)}";

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td><div style=\"font-weight:700\">{E(it.ProductName)}</div><div style=\"color:var(--muted); font-size:11px\">{E(it.InvoiceNote)}</div></td>");
            sb.AppendLine($"<td>{E(it.ModelNo)}</td>");
            sb.AppendLine($"<td>{E(it.SerialNo)}</td>");
            sb.AppendLine($"<td class=\"num\">{it.Qty:0.##}</td>");
            sb.AppendLine($"<td class=\"num\">{Money(it.Rate)}</td>");
            sb.AppendLine($"<td class=\"num\">{E(discLabel)}</td>");
            sb.AppendLine($"<td class=\"num\">{Money(it.Amount)}</td>");
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</tbody></table>");

        sb.AppendLine("<div class=\"summary\"><div class=\"box\">");
        sb.AppendLine($"<div class=\"sumrow\"><div>Subtotal</div><div>{Money(invoice.SubTotal)}</div></div>");
        sb.AppendLine($"<div class=\"sumrow\"><div>Discount</div><div>{Money(invoice.DiscountAmount ?? 0)}</div></div>");
        sb.AppendLine($"<div class=\"sumrow total\"><div>Total</div><div>{Money(invoice.TotalAmount)}</div></div>");
        sb.AppendLine($"<div class=\"sumrow\"><div>Paid</div><div>{Money(invoice.PaidAmount ?? 0)}</div></div>");
        sb.AppendLine($"<div class=\"sumrow\"><div>Balance</div><div>{Money(invoice.BalanceAmount ?? 0)}</div></div>");
        sb.AppendLine("</div></div>");

        sb.AppendLine("<div class=\"footer\">");
        sb.AppendLine($"<div class=\"terms\"><b>Terms</b>\n{E(setting.Terms)}</div>");
        sb.AppendLine("<div class=\"sign\">Signature</div>");
        sb.AppendLine("</div>");

        if (!string.IsNullOrWhiteSpace(setting.FooterMessage))
        {
            sb.AppendLine($"<div style=\"margin-top:12px; font-size:11px; color:var(--muted); text-align:center;\">{E(setting.FooterMessage)}</div>");
        }

        sb.AppendLine("</div></body></html>");
        return sb.ToString();
    }
}
