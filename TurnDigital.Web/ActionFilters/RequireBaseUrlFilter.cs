using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TurnDigital.Domain.IO.Attributes;
using TurnDigital.Domain.IO.Interfaces;

namespace TurnDigital.Web.ActionFilters;

public class RequireBaseUrlFilter : IAsyncActionFilter
{
    private readonly IFileManager _fileManager;

    public RequireBaseUrlFilter(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        switch (resultContext.Result)
        {
            case ObjectResult objectResult:
            {
                var resultObject = objectResult.Value;

                if (resultObject is not null)
                {
                    await SetBaseUrl(resultObject);
                }

                return;
            }
            case ViewResult viewResult:
            {
                var model = viewResult.Model;

                if (model is not null)
                {
                    await SetBaseUrl(model);
                }

                break;
            }
        }
    }

    private async Task SetBaseUrl(object obj)
    {
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) && property.IsDefined(typeof(RequiresBaseUrl)))
            {
                var value = (string)property.GetValue(obj)!;

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var valueWithBaseUrl = await _fileManager.GetUrlAsync(value);

                property.SetValue(obj, valueWithBaseUrl);

                continue;
            }

            if (IsCollectionType(property.PropertyType))
            {
                if (property.GetValue(obj) is not IEnumerable collection)
                    continue;

                var tasks = new List<Task>();

                foreach (var item in collection)
                {
                    tasks.Add(SetBaseUrl(item));
                }

                await Task.WhenAll(tasks);

                continue;
            }

            if (IsComplexType(property.PropertyType))
            {
                var nestedObject = property.GetValue(obj);

                if (nestedObject is not null)
                    await SetBaseUrl(nestedObject);
            }
        }
    }

    private static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && !type.IsEnum && type != typeof(string) && !type.IsValueType;
    }

    private static bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }
}