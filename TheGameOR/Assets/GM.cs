using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour
{

    public GameObject redWin, blueWin;
    public List<GameObject> players;
    public List<Vector3> positions;
    public List<float> rotations;
    public Quaternion rot0, rot1;
    public Text text0, text1;
    public static GM instance;

    static int hp0, hp1;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        resetGame(-1);
    }

    // Update is called once per frame
    void Update()
    {
        text0.text = hp0.ToString();
        text1.text = hp1.ToString();
        if (hp0 < 1 ) resetGame(1);
        else if (hp1 < 1) resetGame(0);
    }

    public void SetPlayer(GameObject player) {
        players.Add(player);
        positions.Add(player.transform.position);
        rotations.Add(player.transform.rotation.eulerAngles.z);
    }

    public void damage(GameObject player) {
        if (player == players[0]) hp0--;
        else hp1--;
    }

    private void resetGame(int indexWin) {
        if (indexWin >= 0) Instantiate((indexWin == 0) ? redWin : blueWin);
        Bullet.destroyAllBullets();
        hp0 = 15;
        hp1 = hp0;

        for (int i = 0; i < players.Count; i++) {
            players[i].transform.SetPositionAndRotation(positions[i], Quaternion.Euler(0, 0, rotations[i]));
            players[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

}

