using UnityEngine;

public class GameBoardHandler : MonoBehaviour
{
    public ArrayLayout GameBoardMatrix;
    public static GameBoardHandler GetGameBoardHandler;
    public GameObject ReturnButton;
    bool scored;
    public Transform flyPosition;
    public int sum;
    [SerializeField]
    private GameObject PowerHolder;
    [SerializeField]
    private PowerUp[] PowerUp;
    private TurnManager _turnManager;

    private void Awake()
    {
        GetGameBoardHandler = this;
    }

    // Use this for initialization
    void Start()
    {
        _turnManager = TurnManager.GetTurnManager;
        ReturnButton.SetActive(false);
    }

    private void ClearCellsAndScoreInRow(Number[] numbers, int row, int cardNumber, Number number)
    {
        if (numbers[0] != null && numbers[0].MyNumber == cardNumber)
        {
            if (numbers[1] != null && numbers[1].MyNumber == cardNumber)
            {
                if (numbers[2] != null && numbers[2].MyNumber == cardNumber)
                {
                    if (numbers[3] != null && numbers[3].MyNumber == cardNumber)
                    {
                        if (numbers[4] != null && numbers[4].MyNumber == cardNumber)
                        {
                            scored = true;
                            for (int i = 0; i < 5; i++)
                            {
                                if (numbers[i] != number)
                                    numbers[i].Destroy(row, i, _turnManager.TurnNumber, flyPosition);
                            }
                        }
                        else
                        {
                            scored = true;
                            for (int i = 0; i < 4; i++)
                            {
                                if (numbers[i] != number)
                                    numbers[i].Destroy(row, i, _turnManager.TurnNumber, flyPosition);
                            }
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 0; i < 3; i++)
                        {
                            if (numbers[i] != number)
                                numbers[i].Destroy(row, i, _turnManager.TurnNumber, flyPosition);
                        }
                    }
                }
            }
        }
        if (numbers[1] != null && numbers[1].MyNumber == cardNumber)
        {
            if (numbers[2] != null && numbers[2].MyNumber == cardNumber)
            {
                if (numbers[3] != null && numbers[3].MyNumber == cardNumber)
                {
                    if (numbers[4] != null && numbers[4].MyNumber == cardNumber)
                    {
                        scored = true;
                        for (int i = 1; i < 5; i++)
                        {
                            if (numbers[i] != number)
                                numbers[i].Destroy(row, i, _turnManager.TurnNumber, flyPosition);
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 1; i < 4; i++)
                        {
                            if (numbers[i] != number)
                                numbers[i].Destroy(row, i, _turnManager.TurnNumber, flyPosition);
                        }
                    }
                }
            }
        }
        if (numbers[2] != null && numbers[2].MyNumber == cardNumber)
        {
            if (numbers[3] != null && numbers[3].MyNumber == cardNumber)
            {
                if (numbers[4] != null && numbers[4].MyNumber == cardNumber)
                {
                    scored = true;
                    for (int i = 2; i < 5; i++)
                    {
                        if (numbers[i] != number)
                            numbers[i].Destroy(row, i, _turnManager.TurnNumber, flyPosition);
                    }
                }
            }
        }
    }

    private void ClearCellsAndScoreInColumn(ArrayLayout.rowData[] numbers, int col, int cardNumber, Number number)
    {
        if (numbers[0].column[col] != null && numbers[0].column[col].MyNumber == cardNumber)
        {
            if (numbers[1].column[col] != null && numbers[1].column[col].MyNumber == cardNumber)
            {
                if (numbers[2].column[col] != null && numbers[2].column[col].MyNumber == cardNumber)
                {
                    if (numbers[3].column[col] != null && numbers[3].column[col].MyNumber == cardNumber)
                    {
                        if (numbers[4].column[col] != null && numbers[4].column[col].MyNumber == cardNumber)
                        {
                            scored = true;
                            for (int i = 0; i < 5; i++)
                            {
                                if (numbers[i].column[col] != number)
                                    numbers[i].column[col].Destroy(i, col, _turnManager.TurnNumber, flyPosition);
                            }
                        }
                        else
                        {
                            scored = true;
                            for (int i = 0; i < 4; i++)
                            {
                                if (numbers[i].column[col] != number)
                                    numbers[i].column[col].Destroy(i, col, _turnManager.TurnNumber, flyPosition);
                            }
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 0; i < 3; i++)
                        {
                            if (numbers[i].column[col] != number)
                                numbers[i].column[col].Destroy(i, col, _turnManager.TurnNumber, flyPosition);
                        }
                    }
                }
            }
        }
        if (numbers[1].column[col] != null && numbers[1].column[col].MyNumber == cardNumber)
        {
            if (numbers[2].column[col] != null && numbers[2].column[col].MyNumber == cardNumber)
            {
                if (numbers[3].column[col] != null && numbers[3].column[col].MyNumber == cardNumber)
                {
                    if (numbers[4].column[col] != null && numbers[4].column[col].MyNumber == cardNumber)
                    {
                        scored = true;
                        for (int i = 1; i < 5; i++)
                        {
                            if (numbers[i].column[col] != number)
                                numbers[i].column[col].Destroy(i, col, _turnManager.TurnNumber, flyPosition);
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 1; i < 4; i++)
                        {
                            if (numbers[i].column[col] != number)
                                numbers[i].column[col].Destroy(i, col, _turnManager.TurnNumber, flyPosition);
                        }
                    }
                }
            }
        }
        if (numbers[2].column[col] != null && numbers[2].column[col].MyNumber == cardNumber)
        {
            if (numbers[3].column[col] != null && numbers[3].column[col].MyNumber == cardNumber)
            {
                if (numbers[4].column[col] != null && numbers[4].column[col].MyNumber == cardNumber)
                {
                    scored = true;
                    for (int i = 2; i < 5; i++)
                    {
                        if (numbers[i].column[col] != number)
                            numbers[i].column[col].Destroy(i, col, _turnManager.TurnNumber, flyPosition);
                    }
                }
            }
        }
    }

    public void SpawnPowerUp(int row, int col, int power)
    {
        var up = Instantiate(PowerHolder, transform).GetComponent<PowerHolder>();
        up.MyPower = PowerUp[power];
        Debug.Log("Spawning power up at: " + col.ToString() + row.ToString() + " of type " + up.MyPower.ToString());
        up.ChangeSprite();
        up.transform.localPosition = new Vector2(col, row);
        GameBoardMatrix.row[2 + row].pColumn[2 + col] = up;
    }

    private void SpawnPowerUp()
    {
        float perc = Random.Range(0, 99);
        if (perc > 60 && !CheckBoardFull())
        {
            int rand_x = Random.Range(-2, 2);

            int rand_y = Random.Range(-2, 2);

            if (!GameBoardMatrix.row[rand_y + 2].column[rand_x + 2] && !GameBoardMatrix.row[rand_y + 2].pColumn[rand_x + 2])
            {
                var up = Instantiate(PowerHolder, transform).GetComponent<PowerHolder>();
                up.transform.localPosition = new Vector2(rand_x, rand_y);
                up.MyPower = PowerUp[Random.Range(0, PowerUp.Length)];
                up.ChangeSprite();
                GameBoardMatrix.row[rand_y + 2].pColumn[rand_x + 2] = up;
                string[] data = new string[]
                {
                    ProjectConstants.message_spawnPowerUp,
                    up.MyPower.powerType.ToString(),
                    (rand_y).ToString(),
                    (rand_x).ToString()
                };
                GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                    GPGController.GetOpponentId());
            }
        }
    }

    public void MagnetizeRow(int? row, int? col)
    {
        if (row != null && col != null)
        {
            int r = (int)row;
            GameManager.GetGameManager.MagnetizeRow(r);
            int c = (int)col;
            for (int i = 0; i < GameBoardMatrix.row[r].column.Length; i++)
            {
                if (i != col && GameBoardMatrix.row[r].column[i] != null)
                    GameBoardMatrix.row[r].column[i].Magnetize();
            }
        }
    }

    public void GivePower(int pos_x, int pos_y, Number card)
    {
        Debug.Log(GameBoardMatrix.row[pos_y].pColumn[pos_x]);
        if (GameBoardMatrix.row[pos_y].pColumn[pos_x] != null)
        {
            var powerUp = GameBoardMatrix.row[pos_y].pColumn[pos_x].GetComponent<PowerHolder>();
            GameBoardMatrix.row[pos_y].pColumn[pos_x] = null;
            powerUp.PowerThings(card, pos_y, pos_x);
        }
    }

    public void DestroyMagnetized(int turnNumber, Transform pos)
    {
        for (int i = 0; i < GameBoardMatrix.row.Length; i++)
        {
            for (int j = 0; j < GameBoardMatrix.row[i].column.Length; j++)
            {
                if (GameBoardMatrix.row[i].column[j] != null && GameBoardMatrix.row[i].column[j].isMagnetized)
                {
                    GameBoardMatrix.row[i].column[j].Destroy(i, j, turnNumber + 1, pos);
                }
            }
        }
    }

    public bool ControlMatrix(int column, int row, int cardNumber, Number number, bool isFromOpponent = false)
    {
        scored = false;
        ClearCellsAndScoreInRow(GameBoardMatrix.row[row].column, row, cardNumber, number);
        ClearCellsAndScoreInColumn(GameBoardMatrix.row, column, cardNumber, number);
        if (!isFromOpponent)
            SpawnPowerUp();
        return scored;
    }

    public bool CheckBoardFull()
    {
        foreach (var r in GameBoardMatrix.row)
        {
            foreach (var c in r.column)
            {
                if (c == null)
                    return false;
            }
        }
        return true;
    }

    public void ClearTheBoard()
    {
        foreach (var item in GameBoardMatrix.row)
        {
            foreach (var a in item.column)
            {
                if (a != null)
                    Destroy(a.gameObject);
            }
        }
        foreach (var item in GameBoardMatrix.row)
        {
            foreach (var a in item.pColumn)
            {
                if (a != null)
                    Destroy(a.gameObject);
            }
        }
        ReturnButton.SetActive(true);
        SumTheBoard();
    }

    public int SumTheBoard(int number = 0)
    {
        sum = 0;
        foreach (var item in GameBoardMatrix.row)
        {
            foreach (var i in item.column)
            {
                if (i != null)
                    sum += i.MyNumber;
            }
        }
        sum += number;
        return sum;
    }
}
