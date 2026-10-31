using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class Gene
{
    public readonly byte Value;
    public readonly string Key;
    public readonly Color ReferenceColor;

    public static IReadOnlyCollection<Type> AllTypes => RegisteredGeneTypes;
    private static readonly HashSet<Type> RegisteredGeneTypes = new();

    public Gene(byte value, string key, string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out var color) is false)
        {
            throw new ArgumentException("Invalid hex color.", nameof(hexColor));
        }

        Value = value;
        Key = key;
        ReferenceColor = color;

        RegisteredGeneTypes.Add(GetType());
    }

    public override string ToString()
    {
        return Key;
    }

    public static IEnumerable<T> GetAll<T>() where T : Gene
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(field => field.GetValue(null))
            .Cast<T>();
    }

    public static string[] GetAllKeys<T>() where T : Gene
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(field => ((Gene)field.GetValue(null)).Key)
            .ToArray();
    }

    public static T GetGeneByIndex<T>(int index) where T : Gene
    {
        return GetAll<T>().First(gene => gene.Value == index);
    }

    public static IEnumerable<Type> GetAllDerivedTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(Gene)) && type.IsAbstract is false);
    }

    protected static void Register<T>() where T : Gene
    {
        Type type = typeof(T);
        RegisteredGeneTypes.Add(type);
    }
}