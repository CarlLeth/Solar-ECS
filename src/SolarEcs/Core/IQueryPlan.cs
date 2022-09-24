﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public interface IQueryPlan<TKey, out TResult>
    {
        IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate);

        IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery { get; }
        QueryPlanState State { get; }
    }

    public interface IQueryPlan<out TResult> : IQueryPlan<Guid, TResult>
    {
    }

    public interface IOrderedQueryPlan<TKey, out TResult> : IQueryPlan<TKey, TResult>
    {
    }

    public interface IOrderedQueryPlan<out TResult> : IQueryPlan<TResult>, IOrderedQueryPlan<Guid, TResult>
    {
    }

    public interface IJoinedQueryPlan<TLeftKey, TRightKey, TJoinKey, TSelectedKey, out TResult> : IQueryPlan<TSelectedKey, TResult>
    {
        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys in the starting (outer/left) query.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TLeftKey, TResult> TakeLeftKey();

        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys in the joined (inner/right) query.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TRightKey, TResult> TakeRightKey();

        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys generated by the key selector expressions used in the join.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TJoinKey, TResult> TakeJoinKey();
    }

    public interface IJoinedQueryPlan<TRightKey, TJoinKey, out TResult> : IQueryPlan<TResult>
    {
        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys in the starting (outer/left) query.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TResult> TakeLeftKey();

        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys in the joined (inner/right) query.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TRightKey, TResult> TakeRightKey();

        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys generated by the key selector expressions used in the join.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TJoinKey, TResult> TakeJoinKey();
    }

    public interface ILeftJoinedQueryPlan<TLeftKey, TJoinKey, TSelectedKey, out TResult> : IQueryPlan<TSelectedKey, TResult>
    {
        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys in the starting (left) query.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TLeftKey, TResult> TakeLeftKey();

        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys generated by the key selector expressions used in the join.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TJoinKey, TResult> TakeJoinKey();
    }

    public interface ILeftJoinedQueryPlan<TJoinKey, out TResult> : IQueryPlan<TResult>
    {
        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys in the starting (left) query.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TResult> TakeLeftKey();

        /// <summary>
        /// Forces the keys of the elements returned by the join to be equal to the keys generated by the key selector expressions used in the join.
        /// </summary>
        /// <returns></returns>
        IQueryPlan<TJoinKey, TResult> TakeJoinKey();
    }

    public enum QueryPlanState
    {
        Immaterial,
        Materialized,
        Empty
    }
}