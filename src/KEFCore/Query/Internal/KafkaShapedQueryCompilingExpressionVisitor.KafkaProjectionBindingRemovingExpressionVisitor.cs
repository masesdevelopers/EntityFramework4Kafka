// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace MASES.EntityFrameworkCore.Kafka.Query.Internal;

public partial class KafkaShapedQueryCompilingExpressionVisitor
{
    private sealed class KafkaProjectionBindingRemovingExpressionVisitor : KafkaProjectionBindingRemovingExpressionVisitorBase
    {
        private readonly SelectExpression _selectExpression;

        public KafkaProjectionBindingRemovingExpressionVisitor(
            SelectExpression selectExpression,
            ParameterExpression jObjectParameter,
            bool trackQueryResults)
            : base(jObjectParameter, trackQueryResults)
        {
            _selectExpression = selectExpression;
        }

        protected override ProjectionExpression GetProjection(ProjectionBindingExpression projectionBindingExpression)
            => _selectExpression.Projection[GetProjectionIndex(projectionBindingExpression)];

        private int GetProjectionIndex(ProjectionBindingExpression projectionBindingExpression)
            => projectionBindingExpression.ProjectionMember != null
                ? _selectExpression.GetMappedProjection(projectionBindingExpression.ProjectionMember).GetConstantValue<int>()
                : (projectionBindingExpression.Index
                    ?? throw new InvalidOperationException(CoreStrings.TranslationFailed(projectionBindingExpression.Print())));
    }
}
