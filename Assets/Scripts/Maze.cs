using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Graph<T>
{
    public Graph() { }
    public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges)
    {
        foreach (var vertex in vertices)
            AddVertex(vertex);

        foreach (var edge in edges)
            AddEdge(edge);
    }

    public Dictionary<T, HashSet<T>> AdjacencyList { get; } = new Dictionary<T, HashSet<T>>();

    public void AddVertex(T vertex)
    {
        if (AdjacencyList.ContainsKey(vertex)) return;
        AdjacencyList[vertex] = new HashSet<T>();
    }

    public void AddEdge(Tuple<T, T> edge)
    {
        if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
        {
            AdjacencyList[edge.Item1].Add(edge.Item2);
            AdjacencyList[edge.Item2].Add(edge.Item1);
        }
    }
}

public class Maze : MonoBehaviour {

    public IntVector2 size;

    public MazeCell cellPrefab;

    public float generationStepDelay;

    public MazePassage passagePrefab;

    public MazeDoor doorPrefab;

    public GameObject noisePrefab;

    [Range(0f, 1f)]
    public float doorProbability;

    public MazeWall[] wallPrefabs;

    public MazeRoomSettings[] roomSettings;

    private MazeCell[,] cells;

    private List<MazeRoom> rooms = new List<MazeRoom>();
    
    private Graph<MazeRoom> roomGraph = new Graph<MazeRoom>();

    private Dictionary<string, MazePassage> roomConnections = new Dictionary<string, MazePassage>();

    private List<MazeDoor> allDoors = new List<MazeDoor>();

    private int rmct = 0;

    private GameObject lastNoise = null;

    private MazeDoor rightDoor = null;

    private float starttime;

    public void loadDoors()
    {
        int i = 1;
        foreach (MazeDoor passage in allDoors)
        {
            //doot doot
            passage.name = i.ToString();
            i++;
            roomGraph.AddVertex(passage.cell.room);
            roomGraph.AddVertex(passage.otherCell.room);
            roomGraph.AddEdge(new Tuple<MazeRoom, MazeRoom>(passage.cell.room, passage.otherCell.room));
            string a = passage.cell.room.name;
            string b = passage.otherCell.room.name;
            if (string.Compare(a, b) > 0)
            {
                roomConnections[a + b] = passage;
            }
            else
            {
                roomConnections[b + a] = passage;
            }
        }
    }

    public bool makePath()
    {
        MazeCell a = cells[0, 0];
        MazeCell b = cells[size.x - 1, size.z - 1];
        return makePath(a.room, b.room);
    }

    public bool makePath(MazeRoom start)
    {
        MazeCell b = cells[size.x - 1, size.z - 1];
        return makePath(start, b.room);
    }

    public bool doorSignal(MazeDoor md)
    {
        return md.name.Equals(rightDoor.name);
    }

    public bool makePath(MazeRoom start, MazeRoom end)
    {
        List<MazeDoor> doors = new List<MazeDoor>();
        Func<MazeRoom, IEnumerable<MazeRoom>> spf = ShortestPathFunction(roomGraph, start);
        MazeRoom prev = null;
        foreach (MazeRoom curr in spf(end))
        {
            if(prev is null)
            {
                prev = curr;
                continue;
            }
            string a = prev.name;
            string b = curr.name;
            if (string.Compare(a, b) > 0)
            {
                doors.Add((MazeDoor)roomConnections[a+b]);
            }
            else
            {
                doors.Add((MazeDoor)roomConnections[b + a]);
            }

            //prev.Show();
            //curr.Show();
            prev = curr;
        }

        if (lastNoise != null)
        {
            Destroy(lastNoise);
            lastNoise = null;
        }

        if (doors.Count == 0) {
            float delt = Time.fixedTime - starttime;
            //GUI.Box(new Rect(0, 0, Screen.width / 2, Screen.height / 2), "You reached the final room in " +
                //delt.ToString() + " seconds!");
            return true; 
        }
        MazeDoor next = doors[0];
        rightDoor = next;



        lastNoise = Instantiate(noisePrefab);
        lastNoise.transform.position = next.transform.position;
        //MazeCell ncell = next.cell;
        //print(ncell.name);
        //ncell.Show();
        AudioSource aud = lastNoise.GetComponent<AudioSource>();
        aud.enabled = true;
        aud.Play();

        //foreach(MazeDoor md in doors)
        //{
        //    //print(md.cell.name);
        //    md.cell.Show();
        //    md.otherCell.Show();
        //    md.gameObject.SetActive(true);
        //}
        //print(doors.Count);
        //foreach(String s in roomConnections.Keys)
        //{
        //    print(s + ", " + roomConnections[s].cell.name + roomConnections[s].otherCell.name);
        //}
        return false;
    }

    public Func<T, IEnumerable<T>> ShortestPathFunction<T>(Graph<T> graph, T start)
    {
        var previous = new Dictionary<T, T>();

        var queue = new Queue<T>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            foreach (var neighbor in graph.AdjacencyList[vertex])
            {
                if (previous.ContainsKey(neighbor))
                    continue;

                previous[neighbor] = vertex;
                queue.Enqueue(neighbor);
            }
        }

        Func<T, IEnumerable<T>> shortestPath = v => {
            var path = new List<T> { };

            var current = v;
            while (!current.Equals(start))
            {
                path.Add(current);
                current = previous[current];
            };

            path.Add(start);
            path.Reverse();

            return path;
        };

        return shortestPath;
    }


    public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}

    public IntVector2 ZeroCoordinates
    {
        get
        {
            return new IntVector2(0, 0);
        }
    }

    public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}

	public IEnumerator Generate () {
		WaitForSecondsRealtime delay = new WaitForSecondsRealtime(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		while (activeCells.Count > 0) {
			yield return delay;
			DoNextGenerationStep(activeCells);
		}
		for (int i = 0; i < rooms.Count; i++) {
			rooms[i].Hide();
		}
        starttime = Time.fixedTime;
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		MazeCell newCell = CreateCell(RandomCoordinates);
		newCell.Initialize(CreateRoom(-1));
		activeCells.Add(newCell);
	}

	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];
		if (currentCell.IsFullyInitialized) {
			activeCells.RemoveAt(currentIndex);
			return;
		}
		MazeDirection direction = currentCell.RandomUninitializedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
		if (ContainsCoordinates(coordinates)) {
			MazeCell neighbor = GetCell(coordinates);
			if (neighbor == null) {
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			}
			else if (currentCell.room.settingsIndex == neighbor.room.settingsIndex) {
				CreatePassageInSameRoom(currentCell, neighbor, direction);
			}
			else {
				CreateWall(currentCell, neighbor, direction);
			}
		}
		else {
			CreateWall(currentCell, null, direction);
		}
	}

	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		return newCell;
	}

	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
		MazePassage passage = Instantiate(prefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(prefab) as MazePassage;
		if (passage is MazeDoor) {
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
            allDoors.Add((MazeDoor)passage);
		}
		else {
			otherCell.Initialize(cell.room);
		}
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	private void CreatePassageInSameRoom (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
		if (cell.room != otherCell.room) {
			MazeRoom roomToAssimilate = otherCell.room;
            if (!cell.room.Assimilate(roomToAssimilate)) return;
			rooms.Remove(roomToAssimilate);
			//Destroy(roomToAssimilate);
		}
	}

	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
		wall.Initialize(cell, otherCell, direction);
		if (otherCell != null) {
			wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}

	private MazeRoom CreateRoom (int indexToExclude) {
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.name = rmct.ToString();
        rmct++;
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
		if (newRoom.settingsIndex == indexToExclude) {
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);
		return newRoom;
	}
}