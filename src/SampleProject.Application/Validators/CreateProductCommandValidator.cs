using FluentValidation;
using SampleProject.Application.Commands.Product;
using SampleProject.Domain.ValueObjects;

namespace SampleProject.Application.Validators;

/// <summary>
/// CreateProductCommand 驗證器
///
/// 驗證策略：
/// - FluentValidation：進行早期輸入驗證（基本格式檢查）
/// - Value Objects：進行領域不變量驗證（詳細業務規則）
///
/// 這樣設計的好處：
/// 1. 早期失敗：在進入 Handler 前就能發現明顯錯誤
/// 2. 更好的 API 錯誤訊息：FluentValidation 提供結構化錯誤
/// 3. 領域保護：即使繞過 FluentValidation，Value Objects 仍會驗證
/// 4. 規則一致性：使用 ValidationRules 常數確保兩層驗證規則一致
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        // 使用 ValidationRules 常數確保與 Value Objects 驗證規則一致
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(ValidationRules.ProductName.MaxLength)
            .WithMessage($"Product name cannot exceed {ValidationRules.ProductName.MaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationRules.ProductDescription.MaxLength)
            .WithMessage($"Product description cannot exceed {ValidationRules.ProductDescription.MaxLength} characters");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(ValidationRules.Money.MinAmount)
            .WithMessage($"BasePrice cannot be negative");

        // 注意：詳細的領域驗證（如名稱格式、價格範圍等）由 Value Objects 負責
        // 這樣即使繞過 FluentValidation，領域層仍能保護自己
    }
}
