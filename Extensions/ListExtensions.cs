using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Godot.Extensions;

public static class ListExtensions
{
    private static readonly Random Random = new Random();
    
    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}