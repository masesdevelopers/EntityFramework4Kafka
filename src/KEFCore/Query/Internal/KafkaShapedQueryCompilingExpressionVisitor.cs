// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json.Linq;

#nullable disable

namespace MASES.EntityFrameworkCore.KNet.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public partial class KafkaShapedQueryCompilingExpressionVisitor : ShapedQueryCompilingExpressionVisitor
{
    private readonly ISqlExpressionFactory _sqlExpressionFactory;
    private readonly IQuerySqlGeneratorFactory _querySqlGeneratorFactory;
    private readonly Type _contextType;
    private readonly bool _threadSafetyChecksEnabled;
    private readonly string _partitionKeyFromExtension;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public KafkaShapedQueryCompilingExpressionVisitor(
        ShapedQueryCompilingExpressionVisitorDependencies dependencies,
        KafkaQueryCompilationContext kafkaQueryCompilationContext,
        ISqlExpressionFactory sqlExpressionFactory,
        IQuerySqlGeneratorFactory querySqlGeneratorFactory)
        : base(dependencies, kafkaQueryCompilationContext)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _querySqlGeneratorFactory = querySqlGeneratorFactory;
        _contextType = kafkaQueryCompilationContext.ContextType;
        _threadSafetyChecksEnabled = dependencies.CoreSingletonOptions.AreThreadSafetyChecksEnabled;
        _partitionKeyFromExtension = kafkaQueryCompilationContext.PartitionKeyFromExtension;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override Expression VisitShapedQuery(ShapedQueryExpression shapedQueryExpression)
    {
        var jObjectParameter = Expression.Parameter(typeof(JObject), "jObject");

        var shaperBody = shapedQueryExpression.ShaperExpression;
        shaperBody = new JObjectInjectingExpressionVisitor().Visit(shaperBody);
        shaperBody = InjectEntityMaterializers(shaperBody);

        switch (shapedQueryExpression.QueryExpression)
        {
            case SelectExpression selectExpression:
                shaperBody = new KafkaProjectionBindingRemovingExpressionVisitor(
                        selectExpression, jObjectParameter,
                        QueryCompilationContext.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                    .Visit(shaperBody);

                var shaperLambda = Expression.Lambda(
                    shaperBody,
                    QueryCompilationContext.QueryContextParameter,
                    jObjectParameter);

                return Expression.New(
                    typeof(QueryingEnumerable<>).MakeGenericType(shaperLambda.ReturnType).GetConstructors()[0],
                    Expression.Convert(
                        QueryCompilationContext.QueryContextParameter,
                        typeof(KafkaQueryContext)),
                    Expression.Constant(_sqlExpressionFactory),
                    Expression.Constant(_querySqlGeneratorFactory),
                    Expression.Constant(selectExpression),
                    Expression.Constant(shaperLambda.Compile()),
                    Expression.Constant(_contextType),
                    Expression.Constant(_partitionKeyFromExtension, typeof(string)),
                    Expression.Constant(
                        QueryCompilationContext.QueryTrackingBehavior == QueryTrackingBehavior.NoTrackingWithIdentityResolution),
                    Expression.Constant(_threadSafetyChecksEnabled));

            case ReadItemExpression readItemExpression:
#if ENABLE_READITEM_QUERYING_ENUMERABLE
                shaperBody = new KafkaProjectionBindingRemovingReadItemExpressionVisitor(
                        readItemExpression, jObjectParameter,
                        QueryCompilationContext.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                    .Visit(shaperBody);

                var shaperReadItemLambda = Expression.Lambda(
                    shaperBody,
                    QueryCompilationContext.QueryContextParameter,
                    jObjectParameter);

                return Expression.New(
                    typeof(ReadItemQueryingEnumerable<>).MakeGenericType(shaperReadItemLambda.ReturnType).GetConstructors()[0],
                    Expression.Convert(
                        QueryCompilationContext.QueryContextParameter,
                        typeof(KafkaQueryContext)),
                    Expression.Constant(readItemExpression),
                    Expression.Constant(shaperReadItemLambda.Compile()),
                    Expression.Constant(_contextType),
                    Expression.Constant(
                        QueryCompilationContext.QueryTrackingBehavior == QueryTrackingBehavior.NoTrackingWithIdentityResolution),
                    Expression.Constant(_threadSafetyChecksEnabled));
#else
                throw new NotImplementedException("ReadItemExpression");
#endif
            default:
                throw new NotSupportedException(CoreStrings.UnhandledExpressionNode(shapedQueryExpression.QueryExpression));
        }
    }
}
