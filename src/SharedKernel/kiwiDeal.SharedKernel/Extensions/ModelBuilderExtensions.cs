using System.Linq.Expressions;
using System.Reflection;
using kiwiDeal.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace kiwiDeal.SharedKernel.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(property), parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        return modelBuilder;
    }

    public static ModelBuilder ApplyStronglyTypedIdConverters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var type = property.ClrType;
                if (!typeof(IStronglyTypedId).IsAssignableFrom(type))
                    continue;

                var fromMethod = type.GetMethod("From", BindingFlags.Public | BindingFlags.Static, [typeof(Guid)]);
                if (fromMethod is null)
                    continue;

                var idParam = Expression.Parameter(type, "id");
                var valueExpr = Expression.Property(idParam, nameof(IStronglyTypedId.Value));
                var toProvider = Expression.Lambda(
                    typeof(Func<,>).MakeGenericType(type, typeof(Guid)),
                    valueExpr, idParam);

                var guidParam = Expression.Parameter(typeof(Guid), "value");
                var fromCall = Expression.Call(fromMethod, guidParam);
                var fromProvider = Expression.Lambda(
                    typeof(Func<,>).MakeGenericType(typeof(Guid), type),
                    fromCall, guidParam);

                var converterType = typeof(ValueConverter<,>).MakeGenericType(type, typeof(Guid));
                var converter = (ValueConverter)Activator.CreateInstance(
                    converterType, toProvider, fromProvider, null)!;

                property.SetValueConverter(converter);
            }
        }

        return modelBuilder;
    }
}
