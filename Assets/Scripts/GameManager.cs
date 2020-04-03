using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;

	public Player playerPrefab;

	private Maze mazeInstance;

	private Player playerInstance;

    public MainCamera mainCamera;

    private Camera mc;

    public Light top;

	private void Start () {
		StartCoroutine(BeginGame());
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
		}
        if (Input.GetKeyDown(KeyCode.P))
        {
            mc.enabled = !mc.enabled;
        }
	}

	private IEnumerator BeginGame () {
        top.intensity = 0;
        mc = Camera.main;
		Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
		mazeInstance = Instantiate(mazePrefab) as Maze;
		yield return StartCoroutine(mazeInstance.Generate());
		playerInstance = Instantiate(playerPrefab) as Player;
        //playerInstance.SetStartLocation(mazeInstance.GetCell(mazeInstance.RandomCoordinates));
        playerInstance.SetStartLocation(mazeInstance.GetCell(mazeInstance.ZeroCoordinates));
        playerInstance.maze = mazeInstance;
        Camera.main.clearFlags = CameraClearFlags.Depth;
		Camera.main.rect = new Rect(0f, 0f, 0.5f, 0.5f);
        mc = Camera.main;
        mazeInstance.loadDoors();
        mainCamera.findSpot();
        playerInstance.init();
        top.intensity = 1f;
        fixListeners();
	}

    private void fixListeners()
    {
        AudioListener[] aL = FindObjectsOfType<AudioListener>();
        for (int i = 0; i < aL.Length; i++)
        {
            //Ignore the first AudioListener in the array 
            if (i == 0)
                continue;

            //Destroy 
            DestroyImmediate(aL[i]);
        }
    }

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		if (playerInstance != null) {
			Destroy(playerInstance.gameObject);
		}
		StartCoroutine(BeginGame());
	}
}