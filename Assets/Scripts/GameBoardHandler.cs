using UnityEngine;
using UnityEngine.UI;

public class GameBoardHandler : MonoBehaviour {
    public ArrayLayout GameBoardMatrix;
    public GameObject ReturnButton;
    bool scored;
    public Transform flyPosition;
    public int sum;
    [SerializeField]
    private Text[] TotalText;
    // Use this for initialization
    void Start () {
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
                                    numbers[i].Destroy(row, i, flyPosition);
                            }
                        }
                        else
                        {
                            scored = true;
                            for (int i = 0; i < 4; i++)
                            {
                                if (numbers[i] != number)
                                    numbers[i].Destroy(row, i, flyPosition);
                            }
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 0; i < 3; i++)
                        {
                            if (numbers[i] != number)
                                numbers[i].Destroy(row, i, flyPosition);
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
                                numbers[i].Destroy(row, i, flyPosition);
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 1; i < 4; i++)
                        {
                            if (numbers[i] != number)
                                numbers[i].Destroy(row, i, flyPosition);
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
                            numbers[i].Destroy(row, i, flyPosition);
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
                                    numbers[i].column[col].Destroy(i, col, flyPosition);
                            }
                        }
                        else
                        {
                            scored = true;
                            for (int i = 0; i < 4; i++)
                            {
                                if (numbers[i].column[col] != number)
                                    numbers[i].column[col].Destroy(i, col, flyPosition);
                            }
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 0; i < 3; i++)
                        {
                            if (numbers[i].column[col] != number)
                                numbers[i].column[col].Destroy(i, col, flyPosition);
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
                                numbers[i].column[col].Destroy(i, col, flyPosition);
                        }
                    }
                    else
                    {
                        scored = true;
                        for (int i = 1; i < 4; i++)
                        {
                            if (numbers[i].column[col] != number)
                                numbers[i].column[col].Destroy(i, col, flyPosition);
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
                            numbers[i].column[col].Destroy(i, col, flyPosition);
                    }
                }
            }
        }
    }

    public bool ControlMatrix(int column, int row, int cardNumber, Number number)
    {
        scored = false;
        ClearCellsAndScoreInRow(GameBoardMatrix.row[row].column, row, cardNumber, number);
        ClearCellsAndScoreInColumn(GameBoardMatrix.row, column, cardNumber, number);
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
        var numbers = FindObjectsOfType<Number>();
        foreach (var item in numbers)
        {
            Destroy(item.gameObject);
        }
        ReturnButton.SetActive(true);
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
        TotalText[0].text = TotalText[1].text = sum.ToString();
        return sum;
    }
}
