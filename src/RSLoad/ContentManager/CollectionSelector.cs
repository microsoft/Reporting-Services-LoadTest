// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using RSLoad.Utilites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RSLoad
{
    /// <summary>
    /// Interface to GetItems from a List
    /// </summary>
    public interface ICollectionSelector
    {
        /// <summary>
        /// Return an item from the List
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="list">list to select an item</param>
        /// <returns>One item from the list</returns>
        T GetItem<T>(IEnumerable<T> list);
    }

    /// <summary>
    /// Random Selector from the List
    /// </summary>
    public class RandomSelector : ICollectionSelector
    {
        /// <summary>
        /// Return an item from the List
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="list">list to select an item</param>
        /// <returns>One item from the list</returns>
        public T GetItem<T>(IEnumerable<T> list)
        {
            return list.GetRandom();
        }
    }

    /// <summary>
    /// Sequential Selector from the list
    /// </summary>
    public class SequentialSelector : ICollectionSelector
    {
        private Dictionary<string, ListState> collectionCache = new Dictionary<string, ListState>();

        /// <summary>
        /// Return an item from the List
        /// </summary>
        /// <typeparam name="T">The given type</typeparam>
        /// <param name="list">list to select an item</param>
        /// <returns>One item from the list</returns>
        public T GetItem<T>(IEnumerable<T> list)
        {
            if (list.Count()  == 0)
                throw new IndexOutOfRangeException("You can't Get an item from a empty List");

            ListState collectionState;
            StackTrace stackTrace = new StackTrace();
            MethodBase callerMethod = stackTrace.GetFrame(1).GetMethod();
            string fullQualifiedCallerMethodName = callerMethod.DeclaringType.FullName  + "." + callerMethod.Name;

            string collectionCallerKey = fullQualifiedCallerMethodName + list.GetHashCode().ToString();
            if (!collectionCache.TryGetValue(collectionCallerKey, out collectionState))
            {
                collectionState = new ListState(list, fullQualifiedCallerMethodName);
                collectionCache.Add(collectionCallerKey, collectionState);
            }

            // Verify the stored collection is the same than the passed to the method
            if (Object.ReferenceEquals(list, collectionState.ObservedListType))
                throw new Exception(String.Format("The reference of the collection passed as paramater {0} doesn't match the stored reference {1} of the stored state", list, collectionState.ObservedListType)); 

            // Circular List Behavior, if reac h the end of the list start again
            if (!collectionState.Enumerator.MoveNext())
            {
                collectionState.Enumerator.Reset();
                collectionState.Enumerator.MoveNext();
            }

            T returnItem = (T)collectionState.Enumerator.Current;
                
            return returnItem;
        }
    }

    /// <summary>
    /// Keeps the state of the list 
    /// </summary>
    public class ListState
    {
        private IEnumerator _enumerator;
        private string _caller;
        private IEnumerable _observedList;

        /// <summary>
        /// Constructor for the listState, keeps a independent enumerator for the list
        /// </summary>
        /// <param name="list">List to keep the enumerator state</param>
        /// <param name="callerMethod">String to identify the caller of the list selector</param>
        public ListState(IEnumerable list, string callerMethod)
        {
            _enumerator = list.GetEnumerator();
            _caller = callerMethod;
            _observedList = list;
        }

        /// <summary>
        /// Observed List Type
        /// </summary>
        /// <returns>The type of the observer list</returns>
        public Type ObservedListType
        {
            get
            {
                return _observedList.GetType(); 
            }
        }

        /// <summary>
        /// Enumerator of the collection
        /// </summary>
        public IEnumerator Enumerator 
        {
            get
            {
                return _enumerator;
            }
        }

        /// <summary>
        /// Name of the method which invokes belongs the state
        /// different methods can use the same collection but each one iterates on it differently
        /// Referenced stored for debugging purposes
        /// </summary>
        public string Caller
        {
            get
            {
                return _caller;
            }
        }
    }
}
