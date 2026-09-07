using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;

namespace Ponce.Templates.APITemplate.Domain.Validators.Entities;

public static class ItemValidator
{
    public static string ValidateName(this string? name) =>
        name.ValidateLength(
            ItemLimits.MAX_LENGTH_NAME,
            StringValidator.ExceptionWhenOutOfRangeArguments(nameof(name), ItemLimits.MAX_LENGTH_NAME),
            ItemLimits.MIN_LENGTH_NAME);

    public static string? ValidateDescription(this string? description)
    {
        if (string.IsNullOrEmpty(description))
            return description;
        return description.ValidateLength(
            ItemLimits.MAX_LENGTH_DESCRIPTION,
            StringValidator.ExceptionWhenOutOfRangeArguments(nameof(description), ItemLimits.MAX_LENGTH_DESCRIPTION));
    }
}
