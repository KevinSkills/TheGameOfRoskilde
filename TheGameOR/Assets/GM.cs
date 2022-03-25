using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour
{

    public GameObject redWin, blueWin;
    public GameObject player0, player1;
    public Vector3 pos0, pos1;
    public Quaternion rot0, rot1;
    public Text text0, text1;
    public static GM instance;

    static int hp0, hp1;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        pos0 = player0.transform.position;
        pos1 = player1.transform.position;
        rot0 = player0.transform.rotation;
        rot1 = player1.transform.rotation;

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

    public void damage(GameObject player) {
        if (player == player0) hp0--;
        else hp1--;
    }

    private void resetGame(int indexWin) {
        if (indexWin >= 0) Instantiate((indexWin == 0) ? redWin : blueWin);
        Bullet.destroyAllBullets();
        hp0 = 15;
        hp1 = hp0;
        player0.transform.SetPositionAndRotation(pos0, rot0);
        player1.transform.SetPositionAndRotation(pos1, rot1);
        player0.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}

