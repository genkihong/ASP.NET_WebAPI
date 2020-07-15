using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http.Controllers;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace Rocket.Common
{
  public class PatchBinding : BodyAndUriParameterBinding
  {
    public PatchBinding(HttpParameterDescriptor descriptor)
      : base(descriptor)
    {
      // Called once body has been parsed.
      BoundBodyContentToModel += (sender, eventArgs) =>
      {
        var rawContent = new StreamReader(eventArgs.Content).ReadToEnd();

        // We want **just** the values passed into the request.
        var dictionary = JObject.Parse(rawContent);

        foreach (var kvp in dictionary)
        {
          ((IPatchState)eventArgs.Model).AddBoundProperty(kvp.Key);
        }
      };

      // Called with every value in the RouteData collection.
      BoundUriKeyToModel += (sender, eventArgs) =>
      {
        ((IPatchState)eventArgs.Model).AddBoundProperty(eventArgs.Key);
      };
    }
  }

  public interface IPatchState<TRequest, TModel>
    where TRequest : class, IPatchState<TRequest, TModel>, new()
  {
    TRequest AddPatchStateMapping<TProperty>(
      Expression<Func<TRequest, TProperty>> propertyExpression,
      Action<TModel> propertyToModelMapping = null);
    void Patch(TModel model);
  }

  public interface IPatchState<TRequest>
  {
    bool IsBound<TProperty>(Expression<Func<TRequest, TProperty>> propertyExpression);
  }

  public interface IPatchState
  {
    void AddBoundProperty(string propertyName);
  }
}