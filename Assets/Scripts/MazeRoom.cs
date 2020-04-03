using UnityEngine;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject {

	public int settingsIndex;

	public MazeRoomSettings settings;
	
	public List<MazeCell> cells = new List<MazeCell>();

    private bool initroom = false;

    public bool assim = true;

    new public string name;

	public void Add (MazeCell cell) {
		cell.room = this;
		cells.Add(cell);
	}

	public bool Assimilate (MazeRoom room) {
        if (!assim) return false;
		for (int i = 0; i < room.cells.Count; i++) {
			Add(room.cells[i]);
		}
        room.cells = cells;
        return true;
	}

	public void Hide () {
		for (int i = 0; i < cells.Count; i++) {
			cells[i].Hide();
		}
	}
	
	public void Show () {

        playBG(true);
        for (int i = 0; i < cells.Count; i++) {
			cells[i].Show();
		}
	}

    public void ShowNoSound(bool init)
    {
        initroom = init;
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Show();
        }
    }

    public void ShowNoSound()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Show();
        }
    }

    public void playBG(bool b)
    {
        if (initroom) return;
        AudioSource aud = GameObject.Find("BGNoise").GetComponent<AudioSource>();
        if (b && !aud.isPlaying) aud.Play();
        else if (!b && aud.isPlaying) aud.Stop();
    }

}