
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class control : MonoBehaviour
{
    public TextMeshPro turnText;
    public TextMeshPro winText;
    public TextMeshPro blackScoreText;
    public TextMeshPro whiteScoreText;

    public GameObject disc;

    public Sprite black;
    public Sprite white;

    public float discSpace;
    public float discOffset;

    //Turn -1 is black and turn 1 is white.
    private int turn = -1;
    private int moves = 0;
    private int scoreWhite = 0;
    private int scoreBlack = 0;

    private bool isValidPlace;
    private bool canPlace;
    private bool canPlaceAny;

    private int[,] discs;

    private List<Vector2Int> markers;
    private List<Vector2Int> markersPass;

    private GameObject[,] discObjects;
    
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    void Start()
    {
        turnText.text = "BLACK TURN";
        turnText.color = Color.black;
        turnText.outlineColor = Color.white;

        winText.text = "";
        blackScoreText.text = "";
        whiteScoreText.text = "";

        discs = new int[8, 8];

        discObjects = new GameObject[8, 8];
        markers = new List<Vector2Int>();
        markersPass = new List<Vector2Int>();

        //Places the start discs.

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                discObjects[x, y] = Instantiate(disc, new Vector3((x * discSpace) + discOffset, (y * discSpace) + discOffset, -1), Quaternion.identity);
                discObjects[x, y].GetComponent<disc>().control = GetComponent<control>();
                discObjects[x, y].GetComponent<disc>().pos = new Vector2Int(x, y);

                if ((x == (1 + 3) && y == (-1 + 4)) || (x == (-1 + 4) && y == (1 + 3)))
                {
                    discs[x, y] = 1;
                    discObjects[x, y].GetComponent<SpriteRenderer>().sprite = white;
                }
                else if ((x == (-1 + 4) && y == (-1 + 4)) || (x == (1 + 3) && y == (1 + 3)))
                {
                    discs[x, y] = -1;
                    discObjects[x, y].GetComponent<SpriteRenderer>().sprite = black;
                }
                else
                {
                    discs[x, y] = 0;
                }
            }
        }

        //Adds start black markers.

        checkValid(turn);

        for (int i = 0; i < markersPass.Count; i++)
        {
            discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().sprite = black;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void click(Vector2Int pos)
    {
        canPlace = false;
        canPlaceAny = false;
        isValidPlace = false;

        if (discs[pos.x, pos.y] == 0)
        {
            if (turn == -1)
            {
                discObjects[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = black;
                discObjects[pos.x, pos.y].GetComponent<SpriteRenderer>().color = Color.white;

                discs[pos.x, pos.y] = -1;

                //Changes the disc colors, the first 2 number are directions.

                changeDiscs(1, 1, pos);
                changeDiscs(-1, -1, pos);
                changeDiscs(1, -1, pos);
                changeDiscs(-1, 1, pos);
                changeDiscs(1, 0, pos);
                changeDiscs(0, 1, pos);
                changeDiscs(-1, 0, pos);
                changeDiscs(0, -1, pos);
            }
            else if (turn == 1)
            {
                discObjects[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = white;
                discObjects[pos.x, pos.y].GetComponent<SpriteRenderer>().color = Color.white;

                discs[pos.x, pos.y] = 1;

                //Changes the disc colors, the first 2 number are directions.

                changeDiscs(1, 1, pos);
                changeDiscs(-1, -1, pos);
                changeDiscs(1, -1, pos);
                changeDiscs(-1, 1, pos);
                changeDiscs(1, 0, pos);
                changeDiscs(0, 1, pos);
                changeDiscs(-1, 0, pos);
                changeDiscs(0, -1, pos);
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (!isValidPlace)
            {
                canPlaceAny = true;

                discObjects[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = null;
                discs[pos.x, pos.y] = 0;
            }
            else
            {
                //Removes old markers.

                for (int i = 0; i < markersPass.Count; i++)
                {
                    if (discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().color.a == 0.5f)
                    {
                        discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().color = Color.white;
                        discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().sprite = null;
                    }
                }

                for (int i = 0; i < markers.Count; i++)
                {
                    if (discObjects[markers[i].x, markers[i].y].GetComponent<SpriteRenderer>().color.a == 0.5f)
                    {
                        discObjects[markers[i].x, markers[i].y].GetComponent<SpriteRenderer>().color = Color.white;
                        discObjects[markers[i].x, markers[i].y].GetComponent<SpriteRenderer>().sprite = null;
                    }
                }

                markers.Clear();
                markersPass.Clear();

                //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                checkValid(turn);
                checkValid(-turn);

                if (canPlace)
                {
                    if (turn == -1)
                    {
                        turnText.text = "WHITE TURN";
                        turnText.color = Color.white;

                        turn = 1;
                    }
                    else if (turn == 1)
                    {
                        turnText.text = "BLACK TURN";
                        turnText.color = Color.black;

                        turn = -1;
                    }

                    //Adds new markers.

                    for (int i = 0; i < markers.Count; i++)
                    {
                        discObjects[markers[i].x, markers[i].y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

                        if (turn == -1)
                        {
                            discObjects[markers[i].x, markers[i].y].GetComponent<SpriteRenderer>().sprite = black;
                        }
                        else if (turn == 1)
                        {
                            discObjects[markers[i].x, markers[i].y].GetComponent<SpriteRenderer>().sprite = white;
                        }
                    }
                }
                else
                {
                    //Adds new markers.

                    for (int i = 0; i < markersPass.Count; i++)
                    {
                        discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

                        if (turn == -1)
                        {
                            discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().sprite = black;
                        }
                        else if (turn == 1)
                        {
                            discObjects[markersPass[i].x, markersPass[i].y].GetComponent<SpriteRenderer>().sprite = white;
                        }
                    }
                }

                moves++;
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //Checks if the game is over.
            if ((moves == ((8 * 8) - 4)) || (!canPlaceAny && !canPlace))
            {
                //Callculate score.
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (discs[x, y] == -1)
                        {
                            scoreBlack++;
                        }
                        else if (discs[x, y] == 1)
                        {
                            scoreWhite++;
                        }
                    }
                }

                blackScoreText.text = "BLACK SCORE: " + scoreBlack;
                whiteScoreText.text = "WHITE SCORE: " + scoreWhite;

                turnText.text = "";

                if (scoreWhite > scoreBlack)
                {
                    winText.color = Color.white;
                    winText.text = "WHITE WINS!!";
                }
                else if (scoreWhite < scoreBlack)
                {
                    winText.color = Color.black;
                    winText.text = "BLACK WINS!!";
                }
                else if (scoreWhite == scoreBlack)
                {
                    winText.color = Color.grey;
                    winText.text = "DRAW";
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void changeDiscs(int x, int y, Vector2Int pos)
    {
        bool isDone;
        int xPos;
        int yPos;
        List<Vector2Int> vectors;

        isDone = false;
        xPos = pos.x + x;
        yPos = pos.y + y;
        vectors = new List<Vector2Int>();

        while (!isDone)
        {
            if (xPos >= 8 || xPos < 0 || yPos >= 8 || yPos < 0)
            {
                isDone = true;
                return;
            }

            if (discs[xPos, yPos] == -turn)
            {
                vectors.Add(new Vector2Int(xPos, yPos));
            }
            else if (discs[xPos, yPos] == 0)
            {
                isDone = true;
                return;
            }
            else if (discs[xPos, yPos] == turn)
            {
                for (int i = 0; i < vectors.Count; i++)
                {
                    isValidPlace = true;

                    discs[vectors[i].x, vectors[i].y] = turn;

                    if (turn == -1)
                    {
                        discObjects[vectors[i].x, vectors[i].y].GetComponent<SpriteRenderer>().sprite = black;
                        discObjects[vectors[i].x, vectors[i].y].GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    else if (turn == 1)
                    {
                        discObjects[vectors[i].x, vectors[i].y].GetComponent<SpriteRenderer>().sprite = white;
                        discObjects[vectors[i].x, vectors[i].y].GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }

                isDone = true;
                return;
            }

            xPos += x;
            yPos += y;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void check(int x, int y, Vector2Int pos, int t)
    {
        int xPos;
        int yPos;
        int disc = 0;
        bool isDone;

        xPos = pos.x + x;
        yPos = pos.y + y;
        disc = 0;
        isDone = false;

        if (discs[pos.x, pos.y] == 0)
        {
            while (!isDone)
            {
                if (xPos >= 8 || xPos < 0 || yPos >= 8 || yPos < 0)
                {
                    isDone = true;
                    return;
                }

                if (discs[xPos, yPos] == -t)
                {
                    disc++;
                }
                else if (discs[xPos, yPos] == 0)
                {
                    isDone = true;
                    return;
                }
                else if (discs[xPos, yPos] == t)
                {
                    if (disc > 0)
                    {
                        if (t == turn)
                        {
                            markersPass.Add(pos);
                            canPlaceAny = true;

                            isDone = true;

                            return;
                        }
                        else if (t == (-turn))
                        {
                            markers.Add(pos);
                            canPlace = true;

                            isDone = true;

                            return;
                        }
                    }
                    else
                    {
                        isDone = true;
                        return;
                    }
                }

                xPos += x;
                yPos += y;
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void checkValid(int t)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                check(1, 1, new Vector2Int(x, y), t);
                check(-1, -1, new Vector2Int(x, y), t);
                check(1, -1, new Vector2Int(x, y), t);
                check(-1, 1, new Vector2Int(x, y), t);
                check(1, 0, new Vector2Int(x, y), t);
                check(0, 1, new Vector2Int(x, y), t);
                check(0, -1, new Vector2Int(x, y), t);
                check(-1, 0, new Vector2Int(x, y), t);
            }
        }
    }
}
