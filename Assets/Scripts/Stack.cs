using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stack : MonoBehaviour
{
    public Camera mainCamera;
    public Text scoreText;
    public GameObject endPanel;

    private const float BOUNDS_SIZE = 2f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.2f;
    private const int COMBO_START_GAIN = 3;

    private GameObject[] stack;
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int stackIndex;
    private int scoreCount = 0;
    private int combo = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePositon;

    private float[] rgbArray = new float[3];

    // Start is called before the first frame update
    private void Start()
    {
        UpdateRGBArray();
        SetupStack();
        SetupBG();
        SetupScoreText();
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Handheld.Vibrate();
            if (PlaceTile())
            {
                UpdateRGBArray();
                SpawnTile();
                UpdateScoreCount();
            }
            else
            {
                EndGame();
            }
            UpdateTileSpeed();
        }

        MoveTile();
        UpdateStackPosition();
    }

    private void UpdateRGBArray()
    {
        for (int i = 0; i < rgbArray.Length; i++)
        {
            rgbArray[i] = Random.Range(0f, 1f);
        }
    }

    private Color RandomColor()
    {
        if (rgbArray.Length == 3)
        {
            return new Color(rgbArray[0], rgbArray[1], rgbArray[2]);
        }
        return new Color(0, 0, 0);
    }

    private void SetupStack()
    {
        stack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            stack[i] = transform.GetChild(i).gameObject;
            stack[i].GetComponent<Renderer>().material.color = RandomColor();
        }
        stackIndex = transform.childCount - 1;
    }

    private void SetupBG()
    {
        mainCamera.backgroundColor = RandomColor();
    }

    private void SetupScoreText()
    {
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.fontSize = 100;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperCenter;
    }

    private bool PlaceTile()
    {
        Transform t = stack[stackIndex].transform;

        if (isMovingOnX)
        {
            float deltaX = lastTilePositon.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                {
                    return false;
                }

                float middle = lastTilePositon.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(new Vector3((t.position.x > 0) ? t.position.x + (t.localScale.x / 2) : t.position.x - (t.localScale.x / 2), t.position.y, t.position.z),
                        new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z));
                t.localPosition = new Vector3(middle - (lastTilePositon.x / 2), scoreCount, lastTilePositon.z);
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;

                    if (stackBounds.x > BOUNDS_SIZE)
                    {
                        stackBounds.x = BOUNDS_SIZE;
                    }

                    float middle = lastTilePositon.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTilePositon.x / 2), scoreCount, lastTilePositon.z);
                }

                combo++;
                t.localPosition = new Vector3(lastTilePositon.x, scoreCount, lastTilePositon.z);
            }
        }
        else
        {
            float deltaZ = lastTilePositon.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                {
                    return false;
                }

                float middle = lastTilePositon.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(new Vector3(t.position.x, t.position.y, (t.position.z > 0) ? t.position.z + (t.localScale.z / 2) : t.position.z - (t.localScale.z / 2)),
                       new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ)));
                t.localPosition = new Vector3(lastTilePositon.x, scoreCount, middle - (lastTilePositon.z / 2));
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUNDS_GAIN;

                    if (stackBounds.y > BOUNDS_SIZE)
                    {
                        stackBounds.y = BOUNDS_SIZE;
                    }

                    float middle = lastTilePositon.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePositon.x, scoreCount, middle - (lastTilePositon.z / 2));
                }

                combo++;
                t.localPosition = new Vector3(lastTilePositon.x, scoreCount, lastTilePositon.z);
            }
        }

        secondaryPosition = isMovingOnX ? t.localPosition.x : t.localPosition.z;

        isMovingOnX = !isMovingOnX;
        return true;
    }

    private void SpawnTile()
    {
        lastTilePositon = stack[stackIndex].transform.localPosition;

        stackIndex--;
        if (stackIndex < 0)
        {
            stackIndex = transform.childCount - 1;
        }

        desiredPosition = Vector3.down * scoreCount;
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        stack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stack[stackIndex].gameObject.GetComponent<Renderer>().material.color = RandomColor();
    }

    private void UpdateScoreCount()
    {
        scoreCount++;
        scoreText.text = scoreCount.ToString();
    }

    private void EndGame()
    {
        gameOver = true;
        stack[stackIndex].AddComponent<Rigidbody>();
        endPanel.SetActive(true);
        SaveScore();
    }

    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX)
        {
            stack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
        }
        else
        {
            stack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);
        }
    }

    public void UpdateStackPosition()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
    }

    private void CreateRubble(Vector3 position, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.gameObject.GetComponent<Renderer>().material.color = RandomColor();
    }

    private void UpdateTileSpeed()
    {
        if (scoreCount % 10 == 0 && scoreCount != 0)
        {
            tileSpeed *= 1.2f;
        }
    }

    private void SaveScore()
    {
        if (PlayerPrefs.GetInt(PlayerPrefsValues.Score.ToString()) < scoreCount)
        {
            PlayerPrefs.SetInt(PlayerPrefsValues.Score.ToString(), scoreCount);
        }
    }
}