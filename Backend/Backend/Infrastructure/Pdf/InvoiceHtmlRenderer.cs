using System.Net;
using System.Text;
using Backend.Domain.Entities;

namespace Backend.Infrastructure.Pdf;

public static class InvoiceHtmlRenderer
{
    public static string Render1(Setting setting, Invoice invoice, IReadOnlyList<InvoiceItem> items)
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

    public static string Render(Setting setting, Invoice invoice, IReadOnlyList<InvoiceItem> items)
    {
        static string E(string? s) => WebUtility.HtmlEncode(s ?? "");
        static string Money(decimal v) => v.ToString("0.00");

        var sb = new StringBuilder();

        sb.AppendLine(@"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"" />
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
<title>Cash Memo</title>

<style>

    * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
    }

    :root {
      --accent: #000000;
      --accent-gray: #cccccc;
      --accent-light: #f5f5f5;
      --accent-mid: #404040;
      --accent-soft: #e5e5e5;
      --border-main: #000;
      --border-dark: #000000;
      --text-dark: #000000;
      --text-mid: #141516;
      --text-muted: #6b7280;
      --white: #ffffff;
      --bg-row: #fafafa;
    }

    body {
      font-family: Arial, sans-serif;
      font-size: 11px;
      color: var(--text-dark);
      background: #ffffff;
    }

    .page {
      width: 794px;
      min-height: 1123px;
      margin: 0 auto;
      padding: 16px 18px;
      background: var(--white);
    }

    /* ─── HEADER ─── */
    .header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 10px 12px 10px 10px;
      border: 1.5px solid var(--border-dark);
      border-bottom: 3px solid var(--accent);
      background: var(--white);
    }

    .header-left {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    /* ── LOGO: swap src when image is ready ── */
    .logo-img {
      width: 68px;
      height: 68px;
      object-fit: contain;
      display: block;
    }

    /* Fallback shown only when image src is empty/broken */
    .logo-fallback {
      width: 68px;
      height: 68px;
      border: 2px solid var(--accent);
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      border-radius: 4px;
      background: var(--accent-light);
    }

    .logo-fallback .logo-rk {
      font-size: 22px;
      font-weight: 900;
      color: var(--accent);
      line-height: 1;
      letter-spacing: -1px;
    }

    .logo-fallback .logo-sub {
      font-size: 6.5px;
      font-weight: 700;
      color: var(--accent);
      letter-spacing: 1px;
      text-align: center;
      margin-top: 2px;
    }

    .company-info h1 {
      font-size: 30px;
      font-weight: 900;
      letter-spacing: 2px;
      color: var(--accent);
      line-height: 1.1;
      text-transform: uppercase;
    }

    .company-info .tagline {
      font-size: 12px;
      color: var(--text-mid);
      margin-top: 3px;
    }

    .header-right {
      text-align: right;
      font-size: 12px;
      line-height: 1.9;
      color: var(--text-mid);
    }

    .header-right .contact-label {
      font-weight: 700;
      color: var(--accent);
    }

    /* ─── HINDI BANNER ─── */
    .hindi-banner {
      background: var(--accent-light);
      border: 1.5px solid var(--border-main);
      border-top: none;
      text-align: center;
      font-size: 11px;
      font-weight: 700;
      padding: 5px 8px;
      color: var(--accent);
    }

    /* ─── META ROW ─── */
    .invoice-meta-row {
      display: flex;
      border: 1.5px solid var(--border-dark);
      border-top: none;
      border-bottom: none;
      background: var(--white);
    }

    .gstin-cell {
      width: 210px;
      min-width: 210px;
      border-right: 1px solid var(--border-main);
      padding: 5px 10px;
      font-size: 10px;
      font-weight: 700;
      display: flex;
      align-items: center;
      color: var(--text-mid);
    }

    .gstin-cell span {
      color: var(--accent-mid);
      font-size: 10.5px;
    }

    .cash-memo-cell {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 20px;
      font-weight: 900;
      letter-spacing: 4px;
      color: var(--accent);
      border-right: 1px solid var(--border-main);
    }

    .original-cell {
      width: 190px;
      min-width: 190px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 10px;
      font-weight: 700;
      color: var(--text-mid);
      padding: 5px 8px;
      text-align: center;
    }

    /* ─── CUSTOMER + INVOICE DETAIL ─── */
    .details-row {
      display: flex;
      border: 1.5px solid var(--border-dark);
      border-bottom: none;
    }

    .customer-block {
      flex: 1;
      border-right: 1px solid var(--border-main);
    }

    .section-header {
      background: var(--accent-gray);
      color: var(--text-dark);
      font-weight: 700;
      font-size: 10.5px;
      padding: 4px 10px;
      text-align: center;
      letter-spacing: 0.5px;
    }

    .field-row {
      display: flex;
      padding: 3px 10px;
      border-bottom: 1px solid #eef3fb;
      line-height: 1.6;
    }

    .field-row:last-child {
      border-bottom: none;
    }

    .field-label {
      font-weight: 700;
      min-width: 85px;
      font-size: 10.5px;
      color: var(--accent);
    }

    .field-value {
      font-size: 10.5px;
      color: var(--text-dark);
    }

    .invoice-info-block {
      width: 300px;
      min-width: 300px;
    }

    .invoice-info-block table {
      width: 100%;
      border-collapse: collapse;
    }

    .invoice-info-block td {
      padding: 4px 10px;
      font-size: 10.5px;
      border-bottom: 1px solid #eef3fb;
      vertical-align: middle;
    }

    .invoice-info-block tr:last-child td {
      border-bottom: none;
    }

    .invoice-info-block td:first-child {
      border-right: 1px solid #eef3fb;
      font-weight: 700;
      width: 52%;
      color: var(--accent);
      background: var(--bg-row);
    }

    .invoice-info-block td strong {
      color: var(--accent-mid);
    }

    /* ─── ITEMS TABLE ─── */
    .items-table {
      width: 100%;
      border-collapse: collapse;
      border: 1.5px solid var(--border-dark);
      border-bottom: none;
    }

    .items-table th {
      background: var(--accent-gray);
      color: var(--text-dark);
      border-right: 1px solid #000000;
      padding: 6px 5px;
      text-align: center;
      font-size: 10.5px;
      font-weight: 700;
    }

    .items-table th:last-child {
      border-right: none;
    }

    .items-table td {
      border-right: 1px solid var(--border-main);
      border-bottom: 1px solid var(--border-main);
      padding: 5px 6px;
      font-size: 10.5px;
      vertical-align: top;
    }

    .items-table td:last-child {
      border-right: none;
    }

    .items-table tbody tr:nth-child(odd) td {
      background: var(--bg-row);
    }

    .items-table tbody tr:nth-child(even) td {
      background: var(--white);
    }

    .item-sr-no{
        width: 50px;
    }

    .td-center {
      text-align: center;
    }

    .td-right {
      text-align: right;
    }

    .items-body td {
      height: 50px;
      vertical-align: top;
    }

    .total-row td {
      background: var(--accent-light) !important;
      font-weight: 700;
      color: var(--accent);
      border-top: 1.5px solid var(--border-dark);
    }

    /* ─── BOTTOM ─── */
    .bottom-section {
      display: flex;
      border: 1.5px solid var(--border-dark);
      border-bottom: none;
    }

    .bottom-left {
      flex: 1;
      border-right: 1px solid var(--border-main);
      display: flex;
      flex-direction: column;
    }

    .total-words-box {
      border-bottom: 1px solid var(--border-main);
    }

    .total-words-box p {
      font-size: 10.5px;
      text-align: center;
      font-weight: 700;
      padding: 7px 8px;
      color: var(--accent);
      background: var(--accent-light);
      letter-spacing: 0.3px;
    }

    .bank-box {
      flex: 1;
    }

    .bank-details-inner {
      display: flex;
    }

    .bank-info {
      flex: 1;
      padding: 8px 10px;
    }

    .bank-row {
      display: flex;
      margin-bottom: 4px;
      font-size: 10.5px;
      line-height: 1.5;
    }

    .bank-label {
      font-weight: 700;
      min-width: 88px;
      color: var(--accent);
    }

    .bank-value {
      color: var(--text-dark);
    }

    .upi-box {
      width: 108px;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 8px;
      border-left: 1px solid var(--border-main);
      background: var(--bg-row);
    }

    .qr-placeholder {
      width: 82px;
      height: 82px;
      border: 1.5px dashed var(--border-dark);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 9px;
      color: var(--accent);
      text-align: center;
      margin-bottom: 5px;
      background: var(--white);
      border-radius: 4px;
    }

    .upi-label {
      font-size: 9.5px;
      font-weight: 700;
      color: var(--accent-mid);
      text-align: center;
    }

    /* ─── SUMMARY ─── */
    .bottom-right {
      width: 250px;
      min-width: 250px;
    }

    .summary-table {
      width: 100%;
      border-collapse: collapse;
    }

    .summary-table td {
      padding: 4px 10px;
      font-size: 10.5px;
      border-bottom: 1px solid #eef3fb;
      vertical-align: middle;
    }

    .summary-table .lbl {
      font-weight: 600;
      width: 62%;
      color: var(--text-mid);
    }

    .summary-table .val {
      text-align: right;
      color: var(--text-dark);
    }

    .summary-table tr:nth-child(odd) td {
      background: var(--bg-row);
    }

    .summary-table tr:nth-child(even) td {
      background: var(--white);
    }

    .summary-table .highlight-row td {
      background: var(--accent-soft) !important;
      font-weight: 700;
      color: var(--accent) !important;
      font-size: 11px;
    }

    .summary-table .balance-row td {
      background: var(--accent-mid) !important;
      color: var(--white) !important;
      font-weight: 900;
      font-size: 12px;
      border-top: 2px solid var(--border-dark);
      border-bottom: 2px solid var(--border-dark);
    }

    .eoe-row {
      text-align: right;
      font-size: 9px;
      padding: 2px 10px;
      font-style: italic;
      color: var(--text-muted);
    }

    .certified-row {
      font-size: 9px;
      text-align: center;
      padding: 3px 8px;
      color: var(--text-muted);
      border-top: 1px solid #eef3fb;
    }

    .for-company-row {
      font-size: 11px;
      font-weight: 700;
      text-align: center;
      padding: 3px 8px 5px;
      color: var(--accent);
    }

    /* ─── TERMS ─── */
    .terms-sig-row {
      display: flex;
      border: 1.5px solid var(--border-dark);
    }

    .terms-box {
      flex: 1;
      border-right: 1px solid var(--border-main);
      padding: 0 0 8px 0;
    }

    .terms-box ol {
      padding: 6px 8px 0 26px;
      font-size: 12px;
      line-height: 1.8;
      color: var(--text-mid);
    }

    .customer-sig {
      margin: 20px 0 0 10px;
      font-size: 10px;
      font-weight: 700;
      color: var(--accent);
      border-top: 1px dashed var(--border-main);
      padding-top: 4px;
      width: 130px;
    }

    .sig-box {
      width: 250px;
      min-width: 250px;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: flex-end;
      padding: 10px 14px;
      background: var(--bg-row);
    }

    .auth-sig-label {
      font-size: 10.5px;
      font-weight: 700;
      text-align: center;
      border-top: 1.5px solid var(--accent);
      color: var(--accent);
      width: 100%;
      padding-top: 5px;
      margin-top: 55px;
    }
</style>
</head>

<body>
<div class=""page"">");

        // HEADER
        sb.AppendLine($@"
<div class=""header"">
  <div class=""header-left"">

    <img class=""logo-img"" src="""" 
      onerror=""this.style.display='none'; this.nextElementSibling.style.display='flex';"" />
    <div class=""logo-fallback"" style=""display:none;"">
      <div class=""logo-rk"">RK</div>
      <div class=""logo-sub"">ELECTRONICS</div>
    </div>

    <div class=""company-info"">
      <h1>{E(setting.ShopName)}</h1>
      <p class=""tagline"">{E(setting.Address)}</p>
      <p class=""tagline"">State: Bihar (10)</p>
    </div>
  </div>

  <div class=""header-right"">
    <div><span class=""contact-label"">Ph: </span>{E(setting.Phone)}</div>
    <div><span class=""contact-label"">E-Mail: </span>{E(setting.Email)}</div>
  </div>
</div>");

        // HINDI BANNER
        sb.AppendLine(@"
<div class=""hindi-banner"">
यहाँ सभी कंपनियों के इलेक्ट्रॉनिक्स उत्पादों (LED TV, DTH, फ्रिज, वॉशिंग मशीन, इलेक्ट्रॉनिक आटा चक्की) की बिक्री एवं रिपेयरिंग की जाती है।
</div>");

        // META ROW
        sb.AppendLine($@"
<div class=""invoice-meta-row"">
  <div class=""gstin-cell""></div>
  <div class=""cash-memo-cell"">CASH MEMO</div>
  <div class=""original-cell"">ORIGINAL FOR RECIPIENT</div>
</div>");

        // DETAILS
        sb.AppendLine($@"
<div class=""details-row"">

  <div class=""customer-block"">
    <div class=""section-header"">Customer Detail</div>

    <div class=""field-row"">
      <div class=""field-label"">Name</div>
      <div class=""field-value"">{E(invoice.CustomerName)}</div>
    </div>

    <div class=""field-row"">
      <div class=""field-label"">Address</div>
      <div class=""field-value"">-</div>
    </div>

    <div class=""field-row"">
      <div class=""field-label"">Phone</div>
      <div class=""field-value"">{E(invoice.Mobile)}</div>
    </div>

    <div class=""field-row"">
      <div class=""field-label"">GSTIN</div>
      <div class=""field-value"">-</div>
    </div>

    <div class=""field-row"">
      <div class=""field-label"">Place of Supply</div>
      <div class=""field-value"">Bihar (10)</div>
    </div>
  </div>

  <div class=""invoice-info-block"">
    <div class=""section-header"">Invoice Detail</div>
    <table>
      <tr>
        <td>Invoice No.</td>
        <td><strong>{E(invoice.InvoiceNo)}</strong></td>
      </tr>
      <tr>
        <td>Invoice Date</td>
        <td>{invoice.CreatedAt:dd-MMM-yyyy}</td>
      </tr>
      <tr>
        <td>Payment Received</td>
        <td>{Money(invoice.PaidAmount ?? 0)}</td>
      </tr>
      <tr>
        <td>Total Outstanding</td>
        <td>{Money(invoice.BalanceAmount ?? 0)}</td>
      </tr>
    </table>
  </div>

</div>");

        // ITEMS
        sb.AppendLine(@"
        <table class=""items-table"">
            <thead>
                <tr>
                  <th class=""item-sr-no"">Sr. No.</th>
                  <th style=""text-align:left;"">Name of Product / Service</th>
                  <th>Qty</th>
                  <th style=""text-align:right;"">Rate</th>
                  <th style=""text-align:right;"">Discount</th>
                  <th style=""text-align:right;"">Total</th>
                </tr>
            </thead>
        <tbody>");

        int i = 1;
        decimal totalQty = 0;
        decimal totalRate = 0;
        decimal totalAmount = 0;
        decimal totalDiscount = 0;

        foreach (var it in items)
        {
            totalQty += it.Qty;
            totalRate += it.Rate;
            totalAmount += it.Amount;
            totalDiscount += it.DiscountValue ?? 0;

            string itemNote = string.Empty;
            if (!string.IsNullOrEmpty(it.ModelNo))
            {
                itemNote = itemNote + "Model: " + it.ModelNo + " ";
            }
            if (!string.IsNullOrEmpty(it.SerialNo))
            {
                itemNote = itemNote + "Serial No: " + it.SerialNo + " ";
            }
            if (!string.IsNullOrEmpty(it.WarrantyNote))
            {
                itemNote = itemNote + it.WarrantyNote + " ";
            }
            if (!string.IsNullOrEmpty(it.InvoiceNote))
            {
                itemNote = itemNote + it.InvoiceNote + " ";
            }

            sb.AppendLine($@"
            <tr class=""items-body"">
              <td class=""td-center item-sr-no"">{i++}</td>
              <td style=""padding-left:8px;""><b>{E(it.ProductName)}</b></br>{E(itemNote)}</td>
              <td class=""td-center"">{it.Qty:0.##}</td>
              <td class=""td-right"">{Money(it.Rate)}</td>
              <td class=""td-right"">{Money(it.DiscountValue ?? 0)}</td>
              <td class=""td-right"">{Money(it.Amount)}</td>
            </tr>");
        }

        // TOTAL ROW
        sb.AppendLine($@"
        <tr class=""total-row"">
          <td colspan=""2"" class=""td-right"">Total</td>
          <td class=""td-center"">{totalQty:0.##}</td>
          <td class=""td-right"">{Money(totalRate)}</td>
          <td class=""td-right"">{Money(totalDiscount)}</td>
          <td class=""td-right"">{Money(totalAmount)}</td>
        </tr>");

        sb.AppendLine("</tbody></table>");

        // BOTTOM
        sb.AppendLine($@"
<div class=""bottom-section"">

<div class=""bottom-left"">

  <div class=""total-words-box"">
    <div class=""section-header"">Total in Words</div>
    <p>{NumberToWords(invoice.TotalAmount)}</p>
  </div>

  <div class=""bank-box"">
    <div class=""section-header"">Bank Details</div>

    <div class=""bank-details-inner"">
      <div class=""bank-info"">
        <div class=""bank-row""><span class=""bank-label"">Name</span><span class=""bank-value"">{E(setting.BankName)}</span></div>
        <div class=""bank-row""><span class=""bank-label"">Branch</span><span class=""bank-value"">Darbhanga</span></div>
        <div class=""bank-row""><span class=""bank-label"">Acc. Name</span><span class=""bank-value"">{E(setting.AccountName)}</span></div>
        <div class=""bank-row""><span class=""bank-label"">Acc. Number</span><span class=""bank-value"">{E(setting.AccountNumber)}</span></div>
        <div class=""bank-row""><span class=""bank-label"">IFSC</span><span class=""bank-value"">{E(setting.IFSC)}</span></div>
        <div class=""bank-row""><span class=""bank-label"">UPI ID</span><span class=""bank-value"">{E(setting.UPI)}</span></div>
      </div>

      <div class=""upi-box"">
        <div class=""qr-placeholder"">QR Code</div>
        <div class=""upi-label"">Pay using UPI</div>
      </div>
    </div>

  </div>

</div>

<div class=""bottom-right"">
<table class=""summary-table"">
<tr><td class=""lbl"">Sub Total</td><td class=""val"">{Money(invoice.SubTotal)}</td></tr>
<tr><td class=""lbl"">Discount</td><td class=""val"">{Money(invoice.DiscountAmount ?? 0)}</td></tr>
<tr class=""highlight-row""><td>Total Amount</td><td class=""val"">{Money(invoice.TotalAmount)}</td></tr>
<tr class=""highlight-row""><td>Payment Received</td><td class=""val"">{Money(invoice.PaidAmount ?? 0)}</td></tr>
<tr class=""balance-row""><td>Balance</td><td class=""val"">{Money(invoice.BalanceAmount ?? 0)}</td></tr>
</table>

<div class=""eoe-row"">(E & O.E.)</div>
<div class=""certified-row"">Certified that the particulars given above are true and correct.</div>
<div class=""for-company-row"">For {E(setting.ShopName)}</div>
</div>

</div>");

        // TERMS
        sb.AppendLine(@"
<div class=""terms-sig-row"">
<div class=""terms-box"">
<div class=""section-header"">Terms and Conditions</div>
<ol>
<li>Goods once sold will not be taken back or exchanged.</li>
<li>Warranty as per manufacturer terms only.</li>
<li>Warranty will not be covered if the product is opened or tampered.</li>
<li>Our responsibility ceases once goods leave the premises.</li>
<li>Subject to Darbhanga (Bihar) jurisdiction.</li>
<li>Payment must be made as agreed.</li>
</ol>

<div class=""customer-sig"">Customer Signature</div>
</div>

<div class=""sig-box"">
<div class=""auth-sig-label"">Authorised Signatory</div>
</div>

</div>");

        sb.AppendLine("</div></body></html>");

        return sb.ToString();
    }

    private static string NumberToWords(decimal number)
    {
        return number.ToString("0.00") + " ONLY";
    }
}
