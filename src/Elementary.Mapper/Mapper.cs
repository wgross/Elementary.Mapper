using Elementary.Mapper.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Elementary.Mapper
{
    public interface IMapper
    {
        D Map<S, D>(S source, D destination);
    }

    public class MapperBuilder
    {
        public class Mapper<S, D>
        {
            private readonly Func<S, D, D> mapperFunc;

            internal Mapper(Delegate mapperLambda)
            {
                this.mapperFunc = (Func<S, D, D>)mapperLambda;
            }

            public D Map(S source, D destination) => this.mapperFunc.Invoke(source, destination);
        }

        public class Config<S, D>
        {
            public IMapperPropertySelector PropertySelector { get; }

            public IEnumerable<IMapperValidator> Validators => this.validators;

            private List<IMapperValidator> validators = new List<IMapperValidator>();

            public Config(IMapperPropertySelector propertySelector)
            {
                this.PropertySelector = propertySelector;
                this.validators.Add(new RejectDifferentPropertyTypes());
                this.validators.Add(new RejectIncompleteDestination());
            }

            public Mapper<S, D> Build() => new Mapper<S, D>(CreateMapperLambda().Compile());

            private LambdaExpression CreateMapperLambda()
            {
                var instances = (
                    src: Expression.Parameter(typeof(S), "source"),
                    dst: Expression.Parameter(typeof(D), "destination")
                );

                var propertyPairs = ValidatedPropertyPairs();

                IEnumerable<Expression> makeBodyExpressions()
                {
                    foreach (var expr in propertyPairs.Select(properties => PropertyMapperExpression(instances.src, properties.src, instances.dst, properties.dst)))
                        yield return expr;
                    yield return Expression.Convert(instances.dst, typeof(D));
                }

                var body = Expression.Block(makeBodyExpressions().ToArray());

                return Expression.Lambda(body, instances.src, instances.dst);
            }

            private IEnumerable<(PropertyInfo src, PropertyInfo dst)> ValidatedPropertyPairs()
            {
                var propertyPairs = this.PropertySelector.SelectProperties<S, D>();
                var violations = this.validators
                    .SelectMany(v => v.Validate<S, D>(propertyPairs));
                if (violations.Any())
                    throw new InvalidMapperConfigException(violations);

                return propertyPairs;
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

        public Config<S, D> Map<S, D>()
        {
            if (typeof(S).GetCustomAttributes(typeof(DynamicAttribute), true).Any())
                throw new InvalidOperationException("source type must not be dynamic");
            return new Config<S, D>(new SelectValueTypeAndStringPropertiesOfIdenticalName());
        }
    }
}