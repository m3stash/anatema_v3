using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinitParallax : MonoBehaviour {

    [SerializeField] GameObject top;
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;
    [SerializeField] GameObject bottom;
    [SerializeField] GameObject middle;
    [SerializeField] GameObject leftTop;
    [SerializeField] GameObject rightTop;
    [SerializeField] GameObject leftBottom;
    [SerializeField] GameObject rightBottom;
    private GameObject[] list;
    private GameObject player;
    private int playerOldposX;
    private int playerOldposY;
    private float boundY;
    private float boundX;
    private float halfBoundX;
    private float halfBoundY;

    // Start is called before the first frame update
    void Start() {
        list = new[] { top, right, bottom, left, middle, leftTop, rightTop, leftBottom, rightBottom };
        Renderer render = list[0].GetComponent<Renderer>();
        boundX = render.bounds.size.x;
        boundY = render.bounds.size.y;
        halfBoundX = boundX / 2;
        halfBoundY = boundY / 2;
        // player = GameManager.GetPlayer();
        transform.position = player.transform.position;
        middle.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, middle.transform.position.z);
        left.transform.position = new Vector3(middle.transform.position.x - boundX, middle.transform.position.y, middle.transform.position.z);
        leftTop.transform.position = new Vector3(middle.transform.position.x - boundX, middle.transform.position.y + boundY, middle.transform.position.z);
        rightTop.transform.position = new Vector3(middle.transform.position.x + boundX, middle.transform.position.y + boundY, middle.transform.position.z);
        right.transform.position = new Vector3(middle.transform.position.x + boundX, middle.transform.position.y, middle.transform.position.z);
        bottom.transform.position = new Vector3(middle.transform.position.x, middle.transform.position.y - boundY, middle.transform.position.z);
        leftBottom.transform.position = new Vector3(middle.transform.position.x - boundX, middle.transform.position.y - boundY, middle.transform.position.z);
        rightBottom.transform.position = new Vector3(middle.transform.position.x + boundX, middle.transform.position.y - boundY, middle.transform.position.z);
        top.transform.position = new Vector3(middle.transform.position.x, middle.transform.position.y + boundY, middle.transform.position.z);
        playerOldposX = (int)player.transform.position.x;
        StartCoroutine(CalcPosition());
    }

    IEnumerator CalcPosition() {
        while (true) {
            var playerX = (int)player.transform.position.x;
            var playerY = (int)player.transform.position.y;
            if (playerOldposX != playerX && playerOldposY != playerY) {
                if (player.transform.position.x > playerOldposX) {
                    foreach (GameObject item in list) {
                        Vector3 itempPos = item.transform.position;
                        var middleGo = (int)(itempPos.x + halfBoundX);
                        if (playerX > middleGo && Mathf.Abs(playerX - middleGo) > boundX) {
                            item.transform.position = new Vector3(itempPos.x + (boundX * 3), itempPos.y, itempPos.z);
                        }
                    }
                }
                if (player.transform.position.x < playerOldposX) {
                    foreach (GameObject item in list) {
                        Vector3 itempPos = item.transform.position;
                        var middleGo = (int)(itempPos.x - halfBoundX);
                        if (playerX < middleGo && Mathf.Abs(playerX - middleGo) > boundX) {
                            item.transform.position = new Vector3(itempPos.x - (boundX * 3), itempPos.y, itempPos.z);
                        }
                    }
                }
                if (player.transform.position.y > playerOldposY) {
                    foreach (GameObject item in list) {
                        Vector3 itempPos = item.transform.position;
                        var middleGo = (int)(itempPos.y + halfBoundY);
                        if (playerY > middleGo && Mathf.Abs(playerY - middleGo) > boundY) {
                            item.transform.position = new Vector3(itempPos.x, itempPos.y + (boundY * 3), itempPos.z);
                        }
                    }
                }
                if (player.transform.position.y < playerOldposY) {
                    foreach (GameObject item in list) {
                        Vector3 itempPos = item.transform.position;
                        var middleGo = (int)(itempPos.y - halfBoundY);
                        if (playerY < middleGo && Mathf.Abs(playerY - middleGo) > boundY) {
                            item.transform.position = new Vector3(itempPos.x, itempPos.y - (boundY * 3), itempPos.z);
                        }
                    }
                }
                playerOldposX = playerX;
                playerOldposY = playerY;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

}
