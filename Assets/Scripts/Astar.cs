using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
public enum TileType { START, GOAL, ROCK, GRASS, PATH }

public class Astar : MonoBehaviour
{
    [SerializeField]
    private GameObject Cat;

    [SerializeField]
    private BoundsInt limitsBounds;

    private TileType tileType;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile[] tiles;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private GameObject loseText;

    [SerializeField]
    private GameObject winText;

    [SerializeField]
    private GameObject gameComplete;

    [SerializeField]
    private GameObject gameLose;

    [SerializeField]
    private AudioSource rockSound;

    [SerializeField]
    private AudioSource catSound;

    private Vector3Int startPos, goalPos;

    List<Vector3Int> rockList = new List<Vector3Int>();

    List<Vector3Int> borderNodesList = new List<Vector3Int>();

    private Node current;

    private HashSet<Node> openList;

    private HashSet<Node> closedList;

    private int numberOfCellBounds = 0;
    
    private int indexCellBound = 0;

    private bool path = false;
    
    private Stack<Vector3Int> bestPath;
    
    private int minLenghtPath = int.MaxValue;

    private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

    private bool gameEnded = false;

    private int moves = 0;

    private void Awake()
    {
        generateRocks();
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null && !gameEnded)
            {
                Vector3 mouseWordPos = camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int clickPos = tilemap.WorldToCell(mouseWordPos);

                Vector3Int CatPosition = tilemap.WorldToCell(Cat.transform.position);
                CatPosition.z = 0;

                TileBase targetTile = tilemap.GetTile(tilemap.WorldToCell(mouseWordPos));

                //If player is clicking in a Rock or in the Cat then its click is ignored
                if (targetTile != tiles[(int)TileType.ROCK] && !CatPosition.Equals(clickPos))
                {
                    bestPath = null;
                    minLenghtPath = int.MaxValue;
                    path = false;

                    AstarDebugger.MyInstance.cleanTileMap();
                    ChangeTile(clickPos);
                    current = null;
                    Algorithm();
                }
            }
        }
    }

    //This method is called in every turn
    private void Initialize()
    {
        Vector3Int CatPosition = tilemap.WorldToCell(Cat.transform.position);
        tilemap.SetTile(CatPosition, tiles[(int)TileType.START]);
        startPos = new Vector3Int(CatPosition.x, CatPosition.y, 0);

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();

        //Find all posible goals the cat can find
        for(int x = limitsBounds.min.x; x < limitsBounds.max.x; x++)
        {
            for(int y = limitsBounds.min.y; y < limitsBounds.max.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if(tile == tiles[(int)TileType.GOAL])
                {
                    borderNodesList.Add(cellPosition);
                }
            }
        }
        numberOfCellBounds = borderNodesList.Count;

        current = GetNode(startPos);

        //Adding start to the open list
        openList.Add(current);
    }

    //Flow of A* Algorithm
    private void Algorithm()
    {
        if( current == null) //Only first time
        {
            Initialize();
        }

        //Check all posible routes
        while(indexCellBound < numberOfCellBounds)
        {
            cleanLists();
            current = GetNode(startPos);
            goalPos = borderNodesList.ElementAt(indexCellBound);
            indexCellBound++;
            openList.Add(current);
            path = false;

            while (openList.Count > 0 && !path)
            {
                List<Node> neighbors = FindNeighbors(current.Position);
            
                ExamineNeighbors(neighbors, current);

                UpdateCurrentTile(ref current);

                path = GeneratePath(current);
            }
        }


        if(openList.Count == 0 && path==false && bestPath == null)
        {
            TextMeshProUGUI textM = (TextMeshProUGUI)winText.GetComponent("TextMeshProUGUI");
            textM.text = moves.ToString();
            gameComplete.SetActive(true);
        }

        //Then it still can move to the bounds of the tilemap
        if( bestPath != null)
        { 
            Debug.Log(bestPath.ElementAt(0));

            //Set new goalPosition
            goalPos = bestPath.Last();

            //Move Cat and Start point
            moveCat(bestPath.ElementAt(0));
        }

        //AstarDebugger.MyInstance.CreateTiles(openList, closedList, allNodes, startPos, goalPos, bestPath);
    }


    private bool checkGoalPosition(Vector3Int position)
    {
        TileBase nextTile = tilemap.GetTile(position);

        if(nextTile == tiles[(int)TileType.GOAL])
        {
            return true;
        }
        return false;
    }


    //Clean all parameters used to find a path (It's called each time is calculated a new path for a bound cell)
    private void cleanLists()
    {
        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();
        allNodes = new Dictionary<Vector3Int, Node>();
    }

    public void AnimatorCat(Vector3Int pos)
    {
        CatController catCtr = (CatController)Cat.GetComponent("CatController");
        if (pos.x < startPos.x)//left
        {
            Cat.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }else{
            Cat.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }

        catCtr.resetAnimation();

        if (pos.y == startPos.y)//lateral left
        {
            catCtr.animator.SetBool(CatController.LEFT_ANIMATION, true);
        }
        if (pos.y < startPos.y)//lateral down left
        {
            catCtr.animator.SetBool(CatController.LEFT_DOWN_ANIMATION, true);
        }
        if (pos.y > startPos.y)//lateral top left
        {
            catCtr.animator.SetBool(CatController.LEFT_TOP_ANIMATION, true);
        }
    }

    public void moveCat(Vector3Int pos)
    {
        if(checkGoalPosition(pos))
        {
            gameEnded = true;
            TextMeshProUGUI textM = (TextMeshProUGUI)loseText.GetComponent("TextMeshProUGUI");
            textM.text = moves.ToString();
            gameLose.SetActive(true);
        }
        moves++;
        AnimatorCat(pos);
        //Move Cat to the first position of the best path
        Vector3 catPos = tilemap.CellToWorld(pos);
        Cat.transform.position = new Vector3(catPos.x, catPos.y+0.4f, 10);

        //Move Start point under the cat
        startPos = new Vector3Int(pos.x, pos.y, 0);
    }


    //Find Neighbors to a specific cell in the Tilemap
    private List<Node> FindNeighbors(Vector3Int parentPosition)
    {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighborPos = new Vector3Int(parentPosition.x - x, parentPosition.y - y, parentPosition.z);

                if (y == 0 && x == 0)
                    continue;

                //Representation of a hexagonal grid into a rectangular grid
                if(parentPosition.y % 2 == 0)
                {
                    if (x == -1 && y != 0)
                        continue;
                }

                if (parentPosition.y % 2 != 0)
                {
                    if (x == 1 && y != 0)
                        continue;
                }

                if (neighborPos == startPos)
                    continue;

                if (!tilemap.GetTile(neighborPos))
                    continue;

                if (rockList.Contains(neighborPos))
                    continue;

                Node neighbor = GetNode(neighborPos);
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }


    //Add all neighbors to the main list
    private void ExamineNeighbors(List<Node> neighbors, Node current)
    {
        for(int i=0; i<neighbors.Count; i++)
        {
            Node neighbor = neighbors[i];

            int gScore = DetermineGScore(neighbors[i].Position, current.Position);

            if (openList.Contains(neighbor))
            {
                //Then this node it's a better path
                if (current.G + gScore < neighbor.G)
                {
                    CalcValues(current, neighbor, gScore);
                }
            }
            else if (!closedList.Contains(neighbor))
            {
                CalcValues(current, neighbor, gScore);
                openList.Add(neighbor);
            }
        }
    }


    private void CalcValues(Node parent, Node neighhbor, int cost)
    {
        neighhbor.Parent = parent;

        neighhbor.G = parent.G + cost;

        neighhbor.H = (Mathf.Abs(neighhbor.Position.x - goalPos.x) + Mathf.Abs(neighhbor.Position.y - goalPos.y));

        neighhbor.F = neighhbor.G + neighhbor.H;
    }


    //For hexagonal grid all direccion have a weight of 1
    private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
    {
        //In exagonal 
        int gScore = 1;
        return gScore;
    }


    private void UpdateCurrentTile(ref Node current)
    {
        openList.Remove(current);
        closedList.Add(current);

        if(openList.Count> 0)
        {
            current = openList.OrderBy(x => x.F).First();
        }
    }


    private Node GetNode(Vector3Int position)
    {
        if(allNodes.ContainsKey(position))
        {
            //If the node already exists (already calculated)
            return allNodes[position];
        }
        else
        {
            //if the node doesn't exists (we don't want to create a node for each tile, it's very expensive)
            Node node = new Node(position);
            allNodes.Add(position, node);
            return node;
        }
    }


    public void ChangeTileType(TileButton button)
    {
        tileType = button.MyTiletype;
    }

    private void ChangeTile(Vector3Int clickPos)
    {
        rockList.Add(clickPos);
        tilemap.SetTile(clickPos, tiles[(int)TileType.ROCK]);
        rockSound.Play();
    }

    //Check if we already reach to the goal, and returns a stack with the path (in reverse).
    private bool GeneratePath(Node current)
    {
        if(current.Position == goalPos)
        {
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

            while(current.Position != startPos)
            {
                finalPath.Push(current.Position);
                current = current.Parent;
            }

            if (finalPath.Count < minLenghtPath)
            {
                minLenghtPath = finalPath.Count;
                //Debug.Log(minLenghtPath);
                bestPath = finalPath;
            }

            return true;
        }

        return false;
    }

    private void generateRocks()
    {
        int numberOfRocks = Random.Range(4, 6);

        for(int i=0; i<numberOfRocks; i++)
        {
            Vector3Int position = new Vector3Int(
                Random.Range(limitsBounds.min.x, limitsBounds.max.x),
                Random.Range(limitsBounds.min.y, limitsBounds.max.y),
                0
            );

            tilemap.SetTile(position, tiles[(int)TileType.ROCK]);

            rockList.Add(position);
        }
    }
}
