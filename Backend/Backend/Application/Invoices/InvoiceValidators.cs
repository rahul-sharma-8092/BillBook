using FluentValidation;

namespace Backend.Application.Invoices;

public sealed class InvoiceCreateDtoValidator : AbstractValidator<InvoiceCreateDto>
{
    public InvoiceCreateDtoValidator()
    {
        RuleFor(x => x.Items).NotNull().Must(x => x.Count > 0).WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).SetValidator(new InvoiceItemInputDtoValidator());

        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountValue).GreaterThanOrEqualTo(0).When(x => x.DiscountValue is not null);
        RuleFor(x => x.DiscountType)
            .Must(t => t is null || t == "RS" || t == "%")
            .WithMessage("DiscountType must be RS or %.")
            .When(x => x.DiscountType is not null);
    }
}

public sealed class InvoiceItemInputDtoValidator : AbstractValidator<InvoiceItemInputDto>
{
    public InvoiceItemInputDtoValidator()
    {
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Qty).GreaterThan(0);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountValue).GreaterThanOrEqualTo(0).When(x => x.DiscountValue is not null);
        RuleFor(x => x.DiscountType)
            .Must(t => t is null || t == "RS" || t == "%")
            .WithMessage("DiscountType must be RS or %.")
            .When(x => x.DiscountType is not null);
    }
}

