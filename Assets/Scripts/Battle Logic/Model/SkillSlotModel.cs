using System;
using System.Collections.Generic;


public enum SkillSlotChangeKind
{
    EquippedChanged,
    InventoryChanged,
    Any
}


public class SkillSlotModel
{
    public const int EquippedSlotCount = 6;

    private readonly int[] _equipped = new int[EquippedSlotCount];  // 장착한 스킬
    private readonly List<int> _inventory = new();                  // 보유한 스킬

    public event Action<SkillSlotChangeKind> OnChanged;

    public int GetEquipped(int index) => _equipped[index];
    public int GetInventory(int index) => _inventory[index];

    public IReadOnlyList<int> Inventory => _inventory;

    public void CopyEquippedTo(int[] dst)
    {
        if (dst == null) return;
        int n = Math.Min(dst.Length, _equipped.Length);
        for (int i = 0; i < n; i++)
        {
            dst[i] = _equipped[i];
        }
    }

    public void CopyInventoryTo(List<int> dst)
    {
        if (dst == null) return;
        dst.Clear();
        dst.AddRange(_inventory);
    }

    public void ApplySnapshot(int[] equipped, List<int> inventory)
    {
        // equipped
        for (int i = 0; i < _equipped.Length; i++)
        {
            _equipped[i] = SkillId.None;
        }

        if (equipped != null)
        {
            int n = Math.Min(equipped.Length, _equipped.Length);
            for (int i = 0; i < n; i++)
            {
                _equipped[i] = equipped[i];
            }
        }

        // inventory
        _inventory.Clear();
        if (inventory != null)
        {
            _inventory.AddRange(inventory);
        }

        OnChanged?.Invoke(SkillSlotChangeKind.Any);
    }



    public void SwapEquipped(int a, int b)
    {
        (_equipped[a], _equipped[b]) = (_equipped[b], _equipped[a]);
        OnChanged?.Invoke(SkillSlotChangeKind.EquippedChanged);
    }


    public void ReplaceEquipped(int index, int skillId)
    {
        _equipped[index] = skillId;
        OnChanged?.Invoke(SkillSlotChangeKind.EquippedChanged);
    }

    public void SetInitial(List<int> inventory, int[] equipped = null)
    {
        // inventory
        _inventory.Clear();
        if (inventory != null)
        {
            _inventory.AddRange(inventory);
        }

        // equipped, 초기화
        for (int i = 0; i < _equipped.Length; i++)
        {
            _equipped[i] = SkillId.None;
        }

        // equipped 값 복사
        if (equipped != null)
        {
            int n = Math.Min(equipped.Length, _equipped.Length);
            for (int i = 0; i < n; i++)
            {
                _equipped[i] = equipped[i];
            }
        }

        OnChanged?.Invoke(SkillSlotChangeKind.Any);
    }



    #region Deprecated Methods

    //public void SetEquipped(int index, int skillId)
    //{
    //    _equipped[index] = skillId;
    //    OnChanged?.Invoke();
    //}

    //public void SetInventory(int index, int skillId)
    //{
    //    _inventory[index] = skillId;
    //    OnChanged?.Invoke();
    //}

    //public void SwapInventory(int a, int b)
    //{
    //    (_inventory[a], _inventory[b]) = (_inventory[b], _inventory[a]);
    //    OnChanged?.Invoke();
    //}

    //public void SwapEquippedInventory(int eqIndex, int invIndex)
    //{
    //    int tmp = _equipped[eqIndex];
    //    _equipped[eqIndex] = _inventory[invIndex];
    //    _inventory[invIndex] = tmp;
    //    OnChanged?.Invoke();
    //}

    #endregion
}
