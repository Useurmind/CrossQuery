﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CrossQuery
{
    public abstract class QueryProviderBase : IQueryProvider
    {
        /// <summary>
        ///     Konstruiert ein <see cref="T:System.Linq.IQueryable" />-Objekt, das die Abfrage auswerten kann, die von einer
        ///     angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.Linq.IQueryable" />-Objekt, das die Abfrage auswerten kann, die von der angegebenen
        ///     Ausdrucksbaumstruktur dargestellt wird.
        /// </returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetElementType();

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), this, expression);
            }

            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        ///     Konstruiert ein <see cref="T:System.Linq.IQueryable`1" />-Objekt, das die Abfrage auswerten kann, die von
        ///     einer angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.Linq.IQueryable`1" />-Objekt, das die Abfrage auswerten kann, die von der angegebenen
        ///     Ausdrucksbaumstruktur dargestellt wird.
        /// </returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        /// <typeparam name="TElement">
        ///     Der Typ der Elemente des <see cref="T:System.Linq.IQueryable`1" />-Objekts, das
        ///     zurückgegeben wird.
        /// </typeparam>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        /// <summary>Führt die Abfrage aus, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.</summary>
        /// <returns>Der Wert, der aus der Ausführung der angegebenen Abfrage resultiert.</returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        /// <summary>Führt die stark typisierte Abfrage aus, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.</summary>
        /// <returns>Der Wert, der aus der Ausführung der angegebenen Abfrage resultiert.</returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        /// <typeparam name="TResult">Der Typ des Werts, der aus der Ausführung der Abfrage resultiert.</typeparam>
        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }

        public abstract object Execute(Expression expression);
    }
}