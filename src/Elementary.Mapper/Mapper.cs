using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Elementary.Mapper
{
    public interface IMapper<S, D>
    {
        D Map(S s, D d);
    }

    public class MapperBuilder
    {
        private class Mapper<S, D> : IMapper<S, D>
        {
            public D Map(S s, D d)
            {
                return d;
            }
        }

        public class Config<S, D>
        {
            public Action<S, D> Build() => (Action<S, D>)CreateMapperLambda().Compile();

            private static LambdaExpression CreateMapperLambda()
            {
                var instances = (
                    src: Expression.Parameter(typeof(S), "source"),
                    dst: Expression.Parameter(typeof(D), "destination")
                );

                var propertyPairs = typeof(S)
                    .GetProperties()
                    .Select(p => (src: p, dst: typeof(D).GetProperty(p.Name)));

                var body = Expression.Block(propertyPairs
                    .Select(properties => PropertyMapperExpression(instances.src, properties.src, instances.dst, properties.dst))
                    .ToArray());

                return Expression.Lambda(body, instances.src, instances.dst);
            }

            private static MethodCallExpression PropertyMapperExpression(ParameterExpression sourceParameter, PropertyInfo sourceProperty, ParameterExpression destinationParameter, PropertyInfo destinationProperty)
                => SetPropertyValueExpression(
                    instance: destinationParameter,
                    property: destinationProperty,
                    value: GetPropertyValueExpression(
                        instance: sourceParameter,
                        property: sourceProperty));

            private static UnaryExpression GetPropertyValueExpression(ParameterExpression instance, PropertyInfo property)
                => Expression.Convert(Expression.Property(instance, property: property), type: property.PropertyType);

            private static MethodCallExpression SetPropertyValueExpression(ParameterExpression instance, PropertyInfo property, Expression value)
                => Expression.Call(instance, property.GetSetMethod(), Expression.Convert(value, property.PropertyType));

            private static LambdaExpression PropertyGetterLambda(PropertyInfo property)
            {
                var instanceParameter = Expression.Parameter(property.DeclaringType, "instance");
                UnaryExpression coercePropertyValue = GetPropertyValueExpression(instanceParameter, property);

                return Expression.Lambda(coercePropertyValue, instanceParameter);
            }

            private static LambdaExpression PropertySetterLambda(PropertyInfo propertyInfo)
            {
                var parameters = (
                    instance: Expression.Parameter(propertyInfo.DeclaringType, "instance"),
                    value: Expression.Parameter(typeof(object), "value")
                );

                return Expression.Lambda(
                    body: SetPropertyValueExpression(parameters.instance, propertyInfo, parameters.value),
                    parameters.instance,
                    parameters.value);
            }
        }

        public Config<S, D> MapStrict<S, D>()
        {
            return new Config<S, D>();
        }
    }
}