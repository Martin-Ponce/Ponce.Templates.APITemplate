using Ponce.Templates.APITemplate.API.Helpers.Strategies.MultiResults;

namespace Ponce.Templates.APITemplate.API.Helpers.Factories;

public static class MultiResponseStrategyFactory
{
    public static IMultiResponseStrategy GetStrategy(string responseFormat) =>
        responseFormat switch
        {
            ApiConstants.ResponseFormats.EXTENDED_MULTI_RESULT => new ExtendedMultiResponseStrategy(),
            ApiConstants.ResponseFormats.SIMPLE_MULTI_RESULT => new SimpleMultiResponseStrategy(),
            _ => new ExtendedMultiResponseStrategy()
        };
}
