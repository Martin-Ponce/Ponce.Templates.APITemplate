using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;

namespace Ponce.Templates.APITemplate.Domain.Validators.Entities;

public static class ExampleItemValidator
{
    public static string ValidateName(this string? name) =>
        name.ValidateLength(
            ExampleItemLimits.MAX_LENGTH_NAME,
            StringValidator.ExceptionWhenOutOfRangeArguments(nameof(name), ExampleItemLimits.MAX_LENGTH_NAME),
            ExampleItemLimits.MIN_LENGTH_NAME);

    public static string? ValidateDescription(this string? description)
    {
        if (string.IsNullOrEmpty(description))
            return description;
        return description.ValidateLength(
            ExampleItemLimits.MAX_LENGTH_DESCRIPTION,
            StringValidator.ExceptionWhenOutOfRangeArguments(nameof(description), ExampleItemLimits.MAX_LENGTH_DESCRIPTION));
    }
}
