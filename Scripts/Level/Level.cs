using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour
{
    [SerializeField] private int numberMoves;
    [SerializeField] private int bonusMoney;
    [SerializeField] private int moneyAmount;
    [SerializeField] private Lose lose;
    [SerializeField] private Storage storage;
    [SerializeField] private Money money;
    [SerializeField] private TypeObject[] typeObjects;

    public int Moves
    {
        get { return _moves; }
        private set
        {
            _moves = value;
            UIController.Instance.SetTextStep(value);
        }
    }

    private int _moves;

    private void OnEnable()
    {
        Board.Instance.Activate(storage, typeObjects);
        Board.Instance.Step += Move;
        storage.Win += Win;
        Reset();
    }

    private void OnDisable()
    {
        Board.Instance.Deactivate();
        Board.Instance.Step -= Move;
        storage.Win -= Win;
    }

    private void Move()
    {
        if (Moves > 0 && Board.Instance.IsActivate)
        {
            Moves--;

            if (Moves <= 0)
                lose.ShowLose(this);
        }
    }

    private void Win()
    {
        Board.Instance.IsActivate = false;
        Moves--;

        int bonus = bonusMoney * Moves;

        UIController.Instance.ShowWinPanel(moneyAmount, Moves, bonusMoney);
        money.AddMoney(moneyAmount + bonus);
        storage.Win -= Win;

    }
    [Button]
    public void SetStorage()
    {
        Shelf[] temp = storage.GetActiveShelves();
        typeObjects = new TypeObject[5];
        for (int i = 0; i < temp.Length; i++)
        {
            typeObjects[i] = temp[i].Type;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void AddMoves(int num) => Moves += num;

    public void Reset()
    {
        Moves = numberMoves;
        storage.Reset();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(storage);



#endif
    }
}