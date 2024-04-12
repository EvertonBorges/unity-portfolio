using System.Collections.Generic;
using UnityEngine;

public class Manager_Memory : Minigame
{

    [Header("References")]
    [SerializeField] private List<Memory_Piece> _pieces;

    protected override void Awake()
    {
        base.Awake();

        Setup();
    }

    protected override void Setup()
    {
        _pieces.ForEach(x => x.Setup());
    }

    protected override void Release()
    {
        throw new System.NotImplementedException();
    }

    private void OnSelect(Memory_Piece piece)
    {

    }

    void OnEnable()
    {
        Manager_Events.Minigames.Memory.OnSelect += OnSelect;
    }

    void OnDisable()
    {
        Manager_Events.Minigames.Memory.OnSelect -= OnSelect;
    }

    
}
