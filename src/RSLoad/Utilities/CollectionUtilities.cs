// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RSLoad.Utilites
{
    /// <summary>
    /// Collection Extension
    /// </summary>
    public static class CollectionExtension
    {
        private static bool _seedInitialized;
        private static Random _random;
        public const int StaticRandomSeed = 123456;

        /// <summary>
        /// Returns the global static random object. Use this in all code that employs
        /// randomization. The intent is that if everyone uses this same object for
        /// all random operations, that a random test run may be reproduced simply
        /// by using the same random seed.
        /// </summary>
        public static Random Random
        {
            get
            {
                if (!_seedInitialized)
                {
                    _random = new Random(StaticRandomSeed);
                    _seedInitialized = true;
                }

                return _random;
            }
        }

        #region IEnumerable extensions
        /// <summary>
        /// Return random item from collection
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="enumerable">Enumerable value</param>
        /// <returns>Random item from collection</returns>
        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            int c = Random.Next(enumerable.Count());
            var found = enumerable.Skip(c).First();
            return found;
        }

        /// <summary>
        /// Return random part of collection in random order
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="enumerable">Enumerable value</param>
        /// <returns>Random items from collection</returns>
        public static IEnumerable<T> GetRandomSequence<T>(this IEnumerable<T> enumerable)
        {
            Random r = Random;
            var randSequence = enumerable.OrderBy(e => r.Next()).Take(r.Next(1, enumerable.Count() + 1));
            return randSequence.ToList();
        }

        /// <summary>
        /// Return up to n items collection in random order
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="enumerable">Enumerable value</param>
        /// <param name="n">upper limit of items returned</param>
        /// <returns>Random item from collection</returns>
        /// <remarks>if n is smaller than source size, returns n items, otherwise source size</remarks>
        public static IEnumerable<T> GetRandomSequence<T>(this IEnumerable<T> enumerable, int n)
        {
            var randSequence = enumerable.OrderBy(e => Random.Next())
                .Take(Math.Min(n, enumerable.Count()));
            return randSequence.ToList();
        }

        /// <summary>
        /// Return up to n items collection in random order
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="enumerable">Enumerable value</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>Random items from collection</returns>
        public static IEnumerable<T> RandomWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return enumerable.GetRandomSequence(enumerable.Count()).Where(predicate);
        }

        /// <summary>
        /// Returns a collection of random items satisfying the predicate. If a collection of
        /// size numToReturn cannot be created, this function will throw.
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="enumerable">Enumerable value</param>
        /// <param name="predicate">Items that satisfy this predicate will be returned.</param>
        /// <param name="numToReturn">Exact number of items that will be returned.</param>
        /// <exception cref="System.Exception">If less than numToReturn items are found that match the given predicate.</exception>
        /// <returns>A random collection of items that satisfy the given predicate.</returns>
        public static IEnumerable<T> RandomWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, int numToReturn)
        {
            var randomCollection = RandomWhere(enumerable, predicate);
            if (randomCollection.Count() < numToReturn)
                throw new Exception(string.Format("Could not find {0} items that match the given predicate.", numToReturn));

            return randomCollection.Take(numToReturn);
        }

        /// <summary>
        /// Extension providing a .ForEach(..) method for IEnumerable.
        /// http://blogs.msdn.com/b/ericlippert/archive/2009/05/18/foreach-vs-foreach.aspx
        /// </summary>
        /// <typeparam name="T">Type of item in the collection.</typeparam>
        /// <param name="collection">Collection whose elements will be used to invoke the action on.</param>
        /// <param name="action">Action to be invoked against each item in the collection.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
                action(item);
        }

        /// <summary>
        /// Converts the given IEnumerable of primitive types to an IEnumerable of the equivalent
        /// nullable primitive type.
        /// </summary>
        /// <typeparam name="T">Primitive type to be converted to nullable.</typeparam>
        /// <param name="nonNullableCollection">IEnumerable of primitive types.</param>
        /// <returns>An IEnumerable of nullable primitives, copied from the given non nullable collection.</returns>
        public static IEnumerable<T?> ToNullable<T>(this IEnumerable<T> nonNullableCollection) where T : struct
        {
            foreach (T item in nonNullableCollection)
                yield return (T?)item;
        }

        /// <summary>
        /// Converts the given IEnumerable of nullable primitive types to an IEnumerable of the
        /// equivalent non-nullable primitive type.  Note that nulls in the given collection will
        /// be returned as the default value (default(T)) for the given type T.
        /// </summary>
        /// <typeparam name="T">Primitive type to be converted from nullable.</typeparam>
        /// <param name="nullableCollection">IEnumerable of nullable primitive types.</param>
        /// <returns>An IEnumerable of non-nullable primitive types, copied from the given nullable collection.</returns>
        public static IEnumerable<T> ToNonNullable<T>(this IEnumerable<T?> nullableCollection) where T : struct
        {
            foreach (T? item in nullableCollection)
            {
                if (item == null)
                    yield return default(T);
                else
                    yield return (T)item;
            }
        }

        /// <summary>
        /// Converts the given IEnumerable of strings to a single, newline separated string. Each string in the
        /// input collection will have its own line in the resulting string.
        /// </summary>
        /// <param name="inputStrings">The list of strings that will be combined into one newline separated string.</param>
        /// <returns>A newline separated string containing the given strings, each on their own line.</returns>
        public static string ToNewLineSeparatedString(this IEnumerable<string> inputStrings)
        {
            StringBuilder newlineSeparatedString = new StringBuilder();
            foreach (var inputString in inputStrings)
                newlineSeparatedString.AppendFormat("{0}{1}", inputString, Environment.NewLine);

            return newlineSeparatedString.ToString();
        }
        #endregion

        #region ICollection extensions
        /// <summary>
        /// Adds a value to a collection if it is not null
        /// </summary>
        /// <typeparam name="T">Type of elements in the colleciton</typeparam>
        /// <param name="collection">Collection to potentially add to</param>
        /// <param name="value">Value to check if null and add</param>
        /// <param name="additionalValues">Additional values to potentially add</param>
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value, params T[] additionalValues)
        {
            if (null != value)
                collection.Add(value);

            foreach (T additionalValue in additionalValues)
            {
                if (null != additionalValue)
                    collection.Add(additionalValue);
            }
        }
        #endregion

        #region IList extensions
        /// <summary>
        /// Replaces all occurences of 'oldValue' in the given list with 'newValue'.
        /// If no matches for 'oldValue' are found, the collection remains unmodified.
        /// </summary>
        /// <typeparam name="T">Type of items in the list.</typeparam>
        /// <param name="list">List whose values will be replaced.</param>
        /// <param name="oldValue">Value that will be replaced.</param>
        /// <param name="newValue">New value that will replace the old value.</param>
        /// <returns>A reference to the given list.</returns>
        public static IList<T> Replace<T>(this IList<T> list, T oldValue, T newValue)
        {
            List<int> indecies = new List<int>();
            int index = 0;
            foreach (T item in list)
            {
                if (item.Equals(oldValue))
                    indecies.Add(index++);
            }

            foreach (int i in indecies)
            {
                list.RemoveAt(i);
                list.Insert(i, newValue);
            }

            return list;
        }

        public static void AddOrReplace(this IDictionary dictionary, object key, object value)
        {
            if (dictionary.Contains(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
        #endregion
    }

    /// <summary>
    /// Useful little class for foreach'ing over a bunch of items
    /// that you do not have in a collection.
    /// <example>
    /// int x = 0;
    /// int y = 3;
    /// int z = -1;
    /// foreach (int num in Enumerate.Items(x, y, z))
    /// {
    ///     Console.WriteLine(num);
    /// }
    /// </example>
    /// </summary>
    public static class Enumerate
    {
        /// <summary>
        /// Returns an IEnumerable that can be used to iterate over each item
        /// given in the parameter list.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">Items to iterate over.</param>
        /// <returns>An IEnumerable to iterate over the given items.</returns>
        public static IEnumerable<T> Items<T>(params T[] items)
        {
            foreach (T item in items)
                yield return item;
        }
    }
}
