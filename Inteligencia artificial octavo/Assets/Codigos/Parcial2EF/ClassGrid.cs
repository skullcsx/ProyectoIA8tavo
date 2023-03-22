using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Edge
//{
//    public Node A;
//    public Node B;
//    public float fCost;
//}

public class Node
{
    public int x;
    public int y;

    // public List<Node> Neighbors;
    public Node Parent;


    // Este es para a* y djikstra.
    public float f_Cost;  // El costo Final de este nodo, el cual es g_Cost + h_Cost
    public float g_Cost;  // el costo de haber llegado a este nodo (terraincost + g_Cost del padre)
    public float h_Cost;  // El costo asociado a la heurística del algoritmo de pathfinding.

    public float fTerrainCost;  // El costo en sí de pararse en este nodo.


    public bool bWalkable;  // Se puede caminar sobre este nodo o no.

    public Node(int in_x, int in_y)
    {
        this.x = in_x;
        this.y = in_y;
        this.Parent = null;
        this.g_Cost = int.MaxValue;
        this.f_Cost = int.MaxValue;
        this.h_Cost = int.MaxValue;
        this.fTerrainCost = 10;
        this.bWalkable = true;
    }

    public override string ToString()
    {
        return x.ToString() + ", " + y.ToString();
    }
}

public class ClassGrid
{
    public int iHeight;
    public int iWidth;

    private float fTileSize;
    private Vector3 v3OriginPosition;

    public Node[,] Nodes;
    public TextMesh[,] debugTextArray;

    public bool bShowDebug = true;
    public GameObject debugGO = null;

    // int -> default = 0
    // bool -> default = false...

    public ClassGrid(int in_height, int in_width, float in_fTileSize = 10.0f, Vector3 in_v3OriginPosition = default)
    {
        iHeight = in_height;
        iWidth = in_width;

        InitGrid();
        this.fTileSize = in_fTileSize;
        this.v3OriginPosition = in_v3OriginPosition;

        if (bShowDebug)
        {

            debugGO = new GameObject("GridDebugParent");

            debugTextArray = new TextMesh[iHeight, iWidth];

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    debugTextArray[y, x] = CreateWorldText2(Nodes[y, x].ToString(),
                    debugGO.transform, GetWorldPosition(x, y) + new Vector3(fTileSize * 0.5f, fTileSize * 0.5f),
                    30, Color.white, TextAnchor.MiddleCenter);
                    //// Dibujamos líneas en el mundo para crear nuestra cuadrícula.
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, iHeight), GetWorldPosition(iWidth, iHeight), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(iWidth, 0), GetWorldPosition(iWidth, iHeight), Color.white, 100f);
        }

    }

    public void InitGrid()
    {
        Nodes = new Node[iHeight, iWidth];

        for (int y = 0; y < iHeight; y++)
        {
            for (int x = 0; x < iWidth; x++)
            {
                Nodes[y, x] = new Node(x, y);
            }
        }
    }

    // Quiero encontrar un camino de start a end.
    public List<Node> DepthFirstSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {
        Node StartNode = GetNode(in_startY, in_startX);
        Node EndNode = GetNode(in_endY, in_endX);

        if (StartNode == null || EndNode == null)
        {
            // Mensaje de error.
            Debug.LogError("Invalid end or start node in DepthFirstSearch");
            return null;
        }

        Stack<Node> OpenList = new Stack<Node>();
        List<Node> ClosedList = new List<Node>();

        OpenList.Push(StartNode);

        while (OpenList.Count > 0)
        {
            // Mientras haya nodos en la lista abierta, vamos a buscar un camino.
            // Obtenemos el primer nodo de la Lista Abierta
            Node currentNode = OpenList.Pop();
            Debug.Log("Current Node is: " + currentNode.x + ", " + currentNode.y);

            // Checamos si ya llegamos al destino.
            if (currentNode == EndNode)
            {
                // Encontramos un camino.
                Debug.Log("Camino encontrado");
                // Necesitamos construir ese camino. Para eso hacemos backtracking.
                List<Node> path = Backtrack(currentNode);
                EnumeratePath(path);
                return path;
            }

            // Otra posible solución
            if (ClosedList.Contains(currentNode))
            {
                continue;
            }

            ClosedList.Add(currentNode);

            // Vamos a visitar a todos sus vecinos.
            List<Node> currentNeighbors = GetNeighbors(currentNode);
            //foreach ( Node neighbor in currentNeighbors ) 
            //{
            //    if (ClosedList.Contains(neighbor))
            //        continue;
            //    // Si no lo contiene, entonces lo agregamos a la lista Abierta
            //    neighbor.Parent = currentNode;
            //    OpenList.Push(neighbor);
            //}

            //// meterlos a la pila en el orden inverso para que al sacarlos nos den el orden "normal".
            for (int x = currentNeighbors.Count - 1; x >= 0; x--)
            {
                // Solo queremos nodos que no estén en la lista cerrada (la cerrada contiene nodos ya visitados).
                if (currentNeighbors[x].bWalkable &&
                    !ClosedList.Contains(currentNeighbors[x]))
                {
                    // Neighbours[x].gCost = CurrentTile.gCost + 1;
                    currentNeighbors[x].Parent = currentNode;  // Le asignas a todos estos nodos su padre.
                    OpenList.Push(currentNeighbors[x]);
                }
            }
            // foreach (Node n in OpenList)
            //    Debug.Log("n Node is: " + n.x + ", " + n.y);

        }

        Debug.LogError("No path found between start and end.");
        return null;
    }

    public List<Node> BreadthFirstSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {
        Node StartNode = GetNode(in_startY, in_startX);
        Node EndNode = GetNode(in_endY, in_endX);

        if (StartNode == null || EndNode == null)
        {
            // Mensaje de error.
            Debug.LogError("Invalid end or start node in DepthFirstSearch");
            return null;
        }

        Queue<Node> OpenList = new Queue<Node>();
        List<Node> ClosedList = new List<Node>();

        OpenList.Enqueue(StartNode);

        while (OpenList.Count > 0)
        {
            // Mientras haya nodos en la lista abierta, vamos a buscar un camino.
            // Obtenemos el primer nodo de la Lista Abierta
            Node currentNode = OpenList.Dequeue();
            Debug.Log("Current Node is: " + currentNode.x + ", " + currentNode.y);

            // Checamos si ya llegamos al destino.
            if (currentNode == EndNode)
            {
                // Encontramos un camino.
                Debug.Log("Camino encontrado");
                // Necesitamos construir ese camino. Para eso hacemos backtracking.
                List<Node> path = Backtrack(currentNode);
                EnumeratePath(path);
                return path;
            }

            // Otra posible solución
            if (ClosedList.Contains(currentNode))
            {
                continue;
            }

            ClosedList.Add(currentNode);

            // Vamos a visitar a todos sus vecinos.
            List<Node> currentNeighbors = GetNeighbors(currentNode);
            foreach (Node neighbor in currentNeighbors)
            {
                if (ClosedList.Contains(neighbor))
                    continue;
                // Si no lo contiene, entonces lo agregamos a la lista Abierta
                neighbor.Parent = currentNode;
                OpenList.Enqueue(neighbor);
            }

            string RemainingNodes = "Nodes in open list are: ";
            foreach (Node n in OpenList)
                RemainingNodes += "(" + n.x + ", " + n.y + ") - ";
            Debug.Log(RemainingNodes);

        }

        Debug.LogError("No path found between start and end.");
        return null;
    }



    public List<Node> BestFirstSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {
        Node StartNode = GetNode(in_startY, in_startX);
        Node EndNode = GetNode(in_endY, in_endX);

        if (StartNode == null || EndNode == null)
        {
            // Mensaje de error.
            Debug.LogError("Invalid end or start node in BestFirstSearch");
            return null;
        }

        PriorityQueue OpenList = new PriorityQueue();
        List<Node> ClosedList = new List<Node>();

        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            // Mientras haya nodos en la lista abierta, vamos a buscar un camino.
            // Obtenemos el primer nodo de la Lista Abierta
            Node currentNode = OpenList.Dequeue();
            Debug.Log("Current Node is: " + currentNode.x + ", " + currentNode.y);

            // Checamos si ya llegamos al destino.
            if (currentNode == EndNode)
            {
                // Encontramos un camino.
                Debug.Log("Camino encontrado");
                // Necesitamos construir ese camino. Para eso hacemos backtracking.
                List<Node> path = Backtrack(currentNode);
                EnumeratePath(path);
                return path;
            }

            // Checamos si ya está en la lista cerrada
            if (ClosedList.Contains(currentNode))
            {
                continue;
            }

            ClosedList.Add(currentNode);

            // Vamos a visitar a todos sus vecinos.
            List<Node> currentNeighbors = GetNeighbors(currentNode);
            foreach (Node neighbor in currentNeighbors)
            {
                if (ClosedList.Contains(neighbor))
                    continue;
                // Si no lo contiene, entonces lo agregamos a la lista Abierta
                neighbor.Parent = currentNode;
                int dist = GetDistance(neighbor, EndNode);
                Debug.Log("dist between " + neighbor.x + ", " + neighbor.y + "and goal is: " + dist);
                neighbor.g_Cost = dist;
                OpenList.Insert(dist, neighbor);
            }

            // foreach (Node n in OpenList)
            //    Debug.Log("n Node is: " + n.x + ", " + n.y);

        }

        Debug.LogError("No path found between start and end.");
        return null;
    }



    public List<Node> DjikstraSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {
        Node StartNode = GetNode(in_startY, in_startX);
        Node EndNode = GetNode(in_endY, in_endX);

        if (StartNode == null || EndNode == null)
        {
            // Mensaje de error.
            Debug.LogError("Invalid end or start node in BestFirstSearch");
            return null;
        }

        PriorityQueue OpenList = new PriorityQueue();
        List<Node> ClosedList = new List<Node>();

        StartNode.g_Cost = 0;
        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            // Mientras haya nodos en la lista abierta, vamos a buscar un camino.
            // Obtenemos el primer nodo de la Lista Abierta
            Node currentNode = OpenList.Dequeue();
            Debug.Log("Current Node is: " + currentNode.x + ", " + currentNode.y);

            // Checamos si ya llegamos al destino.
            // Por motivos didácticos sí lo vamos a terminar al llegar al nodo objetivo.
            if (currentNode == EndNode)
            {
                // Encontramos un camino.
                Debug.Log("Camino encontrado");
                // Necesitamos construir ese camino. Para eso hacemos backtracking.
                List<Node> path = Backtrack(currentNode);
                EnumeratePath(path);
                return path;
            }

            // Checamos si ya está en la lista cerrada
            // NOTA: Aquí VOLVEREMOS DESPUÉS 27 de febrero 2023
            if (ClosedList.Contains(currentNode))
            {
                continue;
            }

            ClosedList.Add(currentNode);

            // Vamos a visitar a todos sus vecinos.
            List<Node> currentNeighbors = GetNeighbors(currentNode);
            foreach (Node neighbor in currentNeighbors)
            {
                if (ClosedList.Contains(neighbor))
                    continue;  // podríamos cambiar esto de ser necesario.

                float fCostoTentativo = neighbor.fTerrainCost + currentNode.g_Cost;

                // Si no lo contiene, entonces lo agregamos a la lista Abierta
                // Si ya está en la lista abierta, hay que dejar solo la versión de ese nodo con el 
                // menor costo.
                if (OpenList.Contains(neighbor))
                {
                    // Checamos si este neighbor tiene un costo MENOR que el que ya está en la lista abierta
                    if (fCostoTentativo < neighbor.g_Cost)
                    {
                        // Entonces lo tenemos que remplazar en la lista abierta.
                        OpenList.Remove(neighbor);
                    }
                    else
                    {
                        continue;
                    }
                }

                neighbor.Parent = currentNode;
                neighbor.g_Cost = fCostoTentativo;
                OpenList.Insert((int)fCostoTentativo, neighbor);
            }

            // foreach (Node n in OpenList)
            //    Debug.Log("n Node is: " + n.x + ", " + n.y);

        }

        Debug.LogError("No path found between start and end.");
        return null;
    }


    public List<Node> AStarSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {
        Node StartNode = GetNode(in_startX, in_startY);
        Node EndNode = GetNode(in_endX, in_endY);

        if (StartNode == null || EndNode == null)
        {
            // Mensaje de error.
            Debug.LogError("Invalid end or start node in BestFirstSearch");
            return null;
        }

        PriorityQueue OpenList = new PriorityQueue();
        List<Node> ClosedList = new List<Node>();

        StartNode.g_Cost = 0;
        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            // Mientras haya nodos en la lista abierta, vamos a buscar un camino.
            // Obtenemos el primer nodo de la Lista Abierta
            Node currentNode = OpenList.Dequeue();
            Debug.Log("Current Node is: " + currentNode.x + ", " + currentNode.y);

            // Checamos si ya llegamos al destino.
            // Por motivos didácticos sí lo vamos a terminar al llegar al nodo objetivo.
            if (currentNode == EndNode)
            {
                // Encontramos un camino.
                Debug.Log("Camino encontrado");
                // Necesitamos construir ese camino. Para eso hacemos backtracking.
                List<Node> path = Backtrack(currentNode);
                EnumeratePath(path);
                return path;
            }

            // Checamos si ya está en la lista cerrada
            // NOTA: Aquí VOLVEREMOS DESPUÉS 27 de febrero 2023
            if (ClosedList.Contains(currentNode))
            {
                continue;
            }

            ClosedList.Add(currentNode);

            // Vamos a visitar a todos sus vecinos.
            List<Node> currentNeighbors = GetNeighbors(currentNode);
            foreach (Node neighbor in currentNeighbors)
            {
                if (ClosedList.Contains(neighbor))
                    continue;  // podríamos cambiar esto de ser necesario.


                float fCostoTentativo = neighbor.fTerrainCost + currentNode.g_Cost;

                // Si no lo contiene, entonces lo agregamos a la lista Abierta
                // Si ya está en la lista abierta, hay que dejar solo la versión de ese nodo con el 
                // menor costo.
                if (OpenList.Contains(neighbor))
                {
                    // Checamos si este neighbor tiene un costo MENOR que el que ya está en la lista abierta
                    if (fCostoTentativo < neighbor.g_Cost)
                    {
                        // Entonces lo tenemos que remplazar en la lista abierta.
                        OpenList.Remove(neighbor);
                    }
                    else
                    {
                        continue;
                    }
                }

                neighbor.Parent = currentNode;
                neighbor.g_Cost = fCostoTentativo;
                neighbor.h_Cost = GetDistance(neighbor, EndNode);
                neighbor.f_Cost = neighbor.g_Cost + neighbor.h_Cost;
                OpenList.Insert((int)neighbor.f_Cost, neighbor);
            }

            foreach (Node n in OpenList.nodes)
                Debug.Log("n Node is: " + n.x + ", " + n.y + ", value= " + n.f_Cost);

        }

        Debug.LogError("No path found between start and end.");
        return null;
    }




    public Node GetNode(int x, int y)
    {
        // Checamos si las coordenadas dadas son válidas dentro de nuestra cuadrícula.
        if (x < iWidth && x >= 0 && y < iHeight && y >= 0)
        {
            return Nodes[y, x];
        }

        // Debug.LogError("Invalid coordinates in GetNode");
        return null;
    }

    public List<Node> GetNeighbors(Node in_currentNode)
    {
        List<Node> out_Neighbors = new List<Node>();
        // Visitamos al nodo de arriba:
        int x = in_currentNode.x;
        int y = in_currentNode.y;
        if (GetNode(y + 1, x) != null)
        {
            out_Neighbors.Add(Nodes[y + 1, x]);
        }

        // Checamos nodo a la izquierda.
        if (GetNode(y, x - 1) != null)
        {
            out_Neighbors.Add(Nodes[y, x - 1]);
        }

        // Checamos a la derecha
        if (GetNode(y, x + 1) != null)
        {
            out_Neighbors.Add(Nodes[y, x + 1]);
        }

        // Checamos abajo
        if (GetNode(y - 1, x) != null)
        {
            out_Neighbors.Add(Nodes[y - 1, x]);
        }

        return out_Neighbors;
    }

    public List<Node> Backtrack(Node in_node)
    {
        List<Node> out_Path = new List<Node>();
        Node current = in_node;
        while (current.Parent != null)
        {
            out_Path.Add(current);
            current = current.Parent;
        }
        out_Path.Add(current);
        out_Path.Reverse();

        return out_Path;
    }

    // Enumera un camino en el orden que tiene y lo muestra en los debugTextArray.
    public void EnumeratePath(List<Node> in_path)
    {
        int iCounter = 0;
        foreach (Node n in in_path)
        {
            iCounter++;
            debugTextArray[n.y, n.x].text = n.ToString() +
                Environment.NewLine + "step: " + iCounter.ToString();
        }
    }

    public int GetDistance(Node in_a, Node in_b)
    {
        // FALTÓ QUE SEAN INT BIEN
        int x_diff = Math.Abs(in_a.x - in_b.x);
        int y_diff = Math.Abs(in_a.y - in_b.y);
        int xy_diff = Math.Abs(x_diff - y_diff);

        return 14 * Math.Min(x_diff, y_diff) + 10 * xy_diff;
    }


    public static TextMesh CreateWorldText2(string in_text, Transform in_parent = null,
        Vector3 in_localPosition = default, int in_iFontSize = 32, Color in_color = default,
        TextAnchor in_textAnchor = TextAnchor.UpperLeft, TextAlignment in_textAlignment = TextAlignment.Left)
    {
        // if (in_color == null) in_color = Color.white;
        GameObject MyObject = new GameObject(in_text, typeof(TextMesh));
        MyObject.transform.parent = in_parent;
        MyObject.transform.localPosition = in_localPosition;

        //agregue el boxcollider por medio de codigo
        MyObject.AddComponent<BoxCollider>();
        //cree una referencia para poder manipular las propiedades del box collider
        BoxCollider bc_BXcollider = MyObject.GetComponent<BoxCollider>();
        //le di un tamaño utilizando la informacion del inspector
        bc_BXcollider.size = new Vector3(5f, 5f, 0);
        //puse su centro 
        bc_BXcollider.center = Vector3.zero;
        //hice que fuera trigger para que el agente pudiera pasar por los nodos
        bc_BXcollider.isTrigger = true;

        TextMesh myTM = MyObject.GetComponent<TextMesh>();
        myTM.text = in_text;
        myTM.anchor = in_textAnchor;
        myTM.alignment = in_textAlignment;
        myTM.fontSize = in_iFontSize;
        myTM.color = in_color;

        return myTM;
    }


    public Vector3 GetWorldPosition(int x, int y)
    {
        // Nos regresa la posición en mundo del Tile/cuadro especificado por X y Y.
        // Por eso lo multiplicamos por el fTileSize
        // (dado que tienen lo mismo de alto y ancho cada cuadro)
        // y finalmente sumamos la posición de origen del grid.
        return new Vector3(x, y) * fTileSize + v3OriginPosition;
    }

    public static TextMesh CreateWorldText(string in_text, Transform in_parent = null,
    Vector3 in_localPosition = default, int in_iFontSize = 32,
    Color in_color = default, TextAnchor in_textAnchor = TextAnchor.UpperLeft,
    TextAlignment in_textAlignment = TextAlignment.Left)
    {
        if (in_color == null) in_color = Color.white;

        // Creamos un GameObject (GO) que tendrá el componente TextMesh donde se mostrará el texto deseado
        GameObject tempGO = new GameObject("World Text", typeof(TextMesh));
        tempGO.transform.SetParent(in_parent);
        tempGO.transform.localPosition = in_localPosition;

        // Inicializamos el componente TextMesh, que es el que realmente se encarga de mostrar en 
        // pantalla el texto que queremos (p.e. el valor del tile, etc.)
        TextMesh textMesh = tempGO.GetComponent<TextMesh>();
        textMesh.anchor = in_textAnchor;
        textMesh.alignment = in_textAlignment;
        textMesh.text = in_text;
        textMesh.fontSize = in_iFontSize;
        textMesh.color = in_color;
        return textMesh;
    }

    // Función que convierte una lista de nodos a una lista de puntos en espacio de mundo.
    public List<Vector3> ConvertBacktrackToWorldPos(List<Node> in_path, bool in_shiftToMiddle = true)
    {
        List<Vector3> WorldPositionPoints = new List<Vector3>();

        // Convertimos cada nodo dentro de in_path a una posición en el espacio de mundo.
        foreach (Node n in in_path)
        {
            Vector3 position = GetWorldPosition(n.x, n.y);
            // Si el parámetro in_shiftToMiddle es true, entonces le añadimos para que vaya al centro del nodo.
            if (in_shiftToMiddle)
            {
                position += new Vector3(fTileSize * 0.5f, fTileSize * 0.5f, 0.0f);
            }

            WorldPositionPoints.Add(position);
        }

        return WorldPositionPoints;
    }

}
