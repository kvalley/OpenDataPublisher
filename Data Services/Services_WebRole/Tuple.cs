using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.DataServices
{
/// <summary>
/// A tuple comprising two items.
/// </summary>
/// <typeparam name="T1">The type of the first item in the tuple.</typeparam>
/// <typeparam name="T2">The type of the second item in the tuple.</typeparam>
/// 
	public struct Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
	{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tuple{T1,T2}"/> class.
    /// </summary>
    /// <param name="first">The first item in the tuple.</param>
    /// <param name="second">The second item in the tuple.</param>
    public Tuple(T1 first, T2 second)
    {
        m_t1 = first;
        m_t2 = second;
    }
 
    /// <summary>
    /// Gets the first item in the tuple.
    /// </summary>
    /// <value>The first item in the tuple.</value>
    public T1 First
    {
        get { return m_t1; }
    }
 
    /// <summary>
    /// Gets the second item in the tuple.
    /// </summary>
    /// <value>The second item in the tuple.</value>
    public T2 Second
    {
        get { return m_t2; }
    }
 
    /// <summary>
    /// Indicates whether the current tuple is equal to another tuple.
    /// </summary>
    /// <param name="other">A tuple to compare with this tuple.</param>
    /// <returns>true if the current tuple is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
    public bool Equals(Tuple<T1, T2> other)
    {
        return EqualityComparer<T1>.Default.Equals(m_t1, other.m_t1) &&
            EqualityComparer<T2>.Default.Equals(m_t2, other.m_t2);
    }
 
    /// <summary>
    /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
    /// </summary>
    /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
    /// <returns>
    /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        return obj is Tuple<T1, T2> && Equals((Tuple<T1, T2>) obj);
    }
 
    /// <summary>
    /// Returns a hash code for this tuple.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Object"/>.</returns>
    public override int GetHashCode()
    {
        return EqualityComparer<T1>.Default.GetHashCode(m_t1) ^
            EqualityComparer<T2>.Default.GetHashCode(m_t2);
    }
 
    /// <summary>
    /// Compares two tuples for equality.
    /// </summary>
    /// <param name="left">The first tuple.</param>
    /// <param name="right">The second tuple.</param>
    /// <returns><c>true</c> if the tuples are equal; false otherwise.</returns>
    public static bool operator ==(Tuple<T1, T2> left, Tuple<T1, T2> right)
    {
        return left.Equals(right);
    }
 
    /// <summary>
    /// Compares two tuples for inequality.
    /// </summary>
    /// <param name="left">The first tuple.</param>
    /// <param name="right">The second tuple.</param>
    /// <returns><c>true</c> if the tuples are not equal; false otherwise.</returns>
    public static bool operator !=(Tuple<T1, T2> left, Tuple<T1, T2> right)
    {
        return !left.Equals(right);
    }
 
    /// <summary>
    /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    /// </summary>
    /// <returns>A <see cref="String"/> that represents the current <see cref="Object"/>.</returns>
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "({0},{1})", m_t1, m_t2);
    }
 
    readonly T1 m_t1;
    readonly T2 m_t2;
	}

/// <summary>
/// Methods for creating and manipulating Tuples.
/// </summary>
	public static class Tuple
	{
    /// <summary>
    /// Creates a new <see cref="Tuple{T1,T2}"/>.
    /// </summary>
    /// <param name="first">The first item in the tuple.</param>
    /// <param name="second">The second item in the tuple.</param>
    /// <returns>A new tuple consisting of the specified two items.</returns>
    public static Tuple<T1, T2> Create<T1, T2>(T1 first, T2 second)
    {
        return new Tuple<T1, T2>(first, second);
    }
 
    /// <summary>
    /// Compares the specified tuples by comparing their first component, and then (if that is equal)
    /// the second component.
    /// </summary>
    /// <param name="left">The left tuple.</param>
    /// <param name="right">The right tuple.</param>
    /// <returns>A 32-bit signed integer that indicates the relative order of the tuples being compared.</returns>
    public static int CompareTo<T1, T2>(this Tuple<T1, T2> left, Tuple<T1, T2> right)
    {
        int result = Comparer<T1>.Default.Compare(left.First, right.First);
        return result == 0 ? Comparer<T2>.Default.Compare(left.Second, right.Second) : result;
    }
	}
}
