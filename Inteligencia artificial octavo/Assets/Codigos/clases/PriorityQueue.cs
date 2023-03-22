using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    public List<Node> nodes = new List<Node>();

    public int Count
    {
        get { return nodes.Count; }
    }

    public void Add(Node in_node)
    {
        nodes.Add(in_node);
    }

    //Meter un elemento en cualquier lugar (inicio, medio o final)
    public void Insert(int in_iPriority, Node in_node)
    {
        //Inserta a in_node en la posici�n de la lista donde haya alg�n elemento con prioridad mayor
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].g_Cost > in_node.g_Cost)
            {
                nodes.Insert(i, in_node);
                return;
            }
        }
        //Si nunca encontr� a alguien con mayor costo que �l, entonces in_node es el de mayor costo
        //y debe ir hasta atr�s de la lista de prioridad
        nodes.Add(in_node);
    }

    public Node Dequeue()
    {
        Node out_node = nodes[0];
        nodes.RemoveAt(0);
        return out_node;
    }

    public Node GetAt(int i)
    {
        return nodes[i];
    }

    public void Remove(Node in_node)
    {
        nodes.Remove(in_node);
    }

    public bool Contains(Node in_node)
    {
        return nodes.Contains(in_node);
    }
}