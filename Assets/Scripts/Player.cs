using UnityEngine;

using UnityEngine.UI;

public class Player : MonoBehaviour {

	private MazeCell currentCell;

	private MazeDirection currentDirection;

    public Maze maze;

    private float starttime;

    public Text text;
    public Text doors;

    private int cd = 0;

    bool done = false;

    public void SetStartLocation(MazeCell cell)
    {
        if (currentCell != null)
        {
            currentCell.OnPlayerExited();
        }
        currentCell = cell;
        transform.localPosition = cell.transform.localPosition;
        currentCell.OnPlayerStart();
    }

    public void init()
    {
        maze.makePath(currentCell.room);
        starttime = Time.fixedTime;

    }

    private void LateUpdate()
    {
        int diff = (int)(Time.fixedTime - starttime);
        text.text = "Seconds elapsed: " + diff.ToString();
        if (done) { 
            doors.text = "You reached the final room!";
            return;
        }
        doors.text = "Correct doors: " + cd.ToString();
    }

    public void SetLocation (MazeCell cell) {
		if (currentCell != null) {
			currentCell.OnPlayerExited();
		}
		currentCell = cell;
		transform.localPosition = cell.transform.localPosition;
		currentCell.OnPlayerEntered();
	}



    //public void SetLocation(MazeDoor cell, bool sound)
    //{
    //    if (sound)
    //    {
    //        SetLocation(cell);
    //        return;
    //    }
    //    if (currentCell != null)
    //    {
    //        currentCell.OnPlayerExited();
    //    }
    //    currentCell = cell;
    //    transform.localPosition = cell.transform.localPosition;
    //    currentCell.OnPlayerEnteredNoSound();
    //}

    private void Move (MazeDirection direction) {
		MazeCellEdge edge = currentCell.GetEdge(direction);

        if (edge is MazePassage) {
			SetLocation(edge.otherCell);
		}
        if (edge is MazeDoor)
        {
            bool correct = maze.doorSignal((MazeDoor)edge);
            if (correct) cd++;
            //SetLocation(edge.otherCell, correct);
            done = maze.makePath(currentCell.room);
        }
    }

	private void Look (MazeDirection direction) {
		transform.localRotation = direction.ToRotation();
		currentDirection = direction;
    }

	private void Update () {
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
			Move(currentDirection);
		}
		else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
			Move(currentDirection.GetNextClockwise());
		}
		else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
			Move(currentDirection.GetOpposite());
		}
		else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			Move(currentDirection.GetNextCounterclockwise());
		}
		else if (Input.GetKeyDown(KeyCode.Q)) {
			Look(currentDirection.GetNextCounterclockwise());
		}
		else if (Input.GetKeyDown(KeyCode.E)) {
			Look(currentDirection.GetNextClockwise());
		}
	}
}