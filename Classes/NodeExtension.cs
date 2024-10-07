using System;
using System.Linq;
using Godot;

public static class NodeExtension
{
    public static void RemoveAllChildren(this Node node)
    {
        foreach(Node child in node.GetChildren())
        {
            node.RemoveChild(child);
        }
    }
}
