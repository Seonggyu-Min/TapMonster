using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SnapshotParser
{
    public static SaveDataDTO ParseSaveDataDTO(DataSnapshot snapshot)
    {
        var dto = new SaveDataDTO();

        if (!TryGetMapFromSnapshot(snapshot, out var root))
        {
            return dto;
        }

        dto.LastSavedAtUnixMs = GetLong(root, DatabaseKeys.LastSavedAtUnixMs, 0);

        // Stage
        if (TryGetDict(root, DatabaseKeys.Stage, out var stage))
        {
            dto.StageDTO = new StageDTO
            {
                CurrentStage = (int)GetLong(stage, DatabaseKeys.CurrentStage, 1)
            };
        }

        // Monster
        dto.MonsterHpDTO = ReadMonsterHp(root);

        // Boss Timer
        dto.BossTimerDTO = new BossTimerDTO();
        if (TryGetDict(root, DatabaseKeys.BossTimer, out var bossTimer))
        {
            dto.BossTimerDTO.IsRunning = ToBool(bossTimer[DatabaseKeys.IsRunning], false);
            dto.BossTimerDTO.BossStage = (int)GetLong(bossTimer, DatabaseKeys.BossStage, 0);
            dto.BossTimerDTO.RemainingSeconds = (float)GetDouble(bossTimer, DatabaseKeys.RemainingSeconds, 0d);
        }


        // Wallet
        dto.WalletDTO = new WalletDTO();
        if (TryGetDict(root, DatabaseKeys.Wallet, out var wallet))
        {
            foreach (var kv in wallet)
            {
                string currencyKey = kv.Key;

                if (kv.Value is not Dictionary<string, object> bnDict)
                    continue;

                var bn = new BigNumberDTO
                {
                    Mantissa = GetDouble(bnDict, DatabaseKeys.Mantissa, 0),
                    Exponent = (int)GetLong(bnDict, DatabaseKeys.Exponent, 0)
                };

                dto.WalletDTO.Currencies[currencyKey] = bn;
            }
        }

        // Skill Slots
        dto.SkillSlotDTO = ReadSkillSlots(root);

        // Levels
        dto.RelicLevels = ReadLevels(root, DatabaseKeys.RelicLevels);
        dto.UpgradeLevels = ReadLevels(root, DatabaseKeys.UpgradeLevels);
        dto.SkillLevels = ReadLevels(root, DatabaseKeys.SkillLevels);

        return dto;
    }

    private static Dictionary<string, int> ReadLevels(Dictionary<string, object> root, string key)
    {
        var result = new Dictionary<string, int>();
        if (!TryGetDict(root, key, out var dict)) return result;

        foreach (var kv in dict)
        {
            int level = (int)ToLong(kv.Value, 0);
            result[kv.Key] = Mathf.Max(0, level);
        }
        return result;
    }
    private static MonsterHpDTO ReadMonsterHp(Dictionary<string, object> root)
    {
        var result = new MonsterHpDTO();

        if (!TryGetDict(root, DatabaseKeys.MonsterHp, out var mhpRoot))
            return result;

        // hasValue
        result.HasValue = ToBool(
            mhpRoot.TryGetValue(DatabaseKeys.HasValue, out var hv) ? 
            hv : null, false
            );

        // maxHp
        if (TryGetDict(mhpRoot, DatabaseKeys.MaxHp, out var maxDict))
        {
            result.MaxHp = new BigNumberDTO
            {
                Mantissa = GetDouble(maxDict, DatabaseKeys.Mantissa, 0),
                Exponent = (int)GetLong(maxDict, DatabaseKeys.Exponent, 0)
            };
        }

        // currentHp
        if (TryGetDict(mhpRoot, DatabaseKeys.CurrentHp, out var curDict))
        {
            result.CurrentHp = new BigNumberDTO
            {
                Mantissa = GetDouble(curDict, DatabaseKeys.Mantissa, 0),
                Exponent = (int)GetLong(curDict, DatabaseKeys.Exponent, 0)
            };
        }

        return result;
    }
    private static SkillSlotDTO ReadSkillSlots(Dictionary<string, object> root)
    {
        var result = new SkillSlotDTO();

        if (!TryGetDict(root, DatabaseKeys.SkillSlots, out var slotsRoot))
            return result;

        // Equipped
        if (TryGetMap(slotsRoot, DatabaseKeys.Equipped, out var eqDict))    // Dictionary<string, object> 형태
        {
            for (int i = 0; i < result.Equipped.Length; i++)
            {
                string k = i.ToString();
                if (eqDict.TryGetValue(k, out var v))
                    result.Equipped[i] = (int)ToLong(v, SkillId.None);
                else
                    result.Equipped[i] = SkillId.None;
            }
        }
        else if (TryGetList(slotsRoot, DatabaseKeys.Equipped, out var eqList))  // IList 형태
        {
            for (int i = 0; i < result.Equipped.Length; i++)
            {
                if (i < eqList.Count)
                    result.Equipped[i] = (int)ToLong(eqList[i], SkillId.None);
                else
                    result.Equipped[i] = SkillId.None;
            }
        }
        else
        {
            // 없으면 전부 None
            for (int i = 0; i < result.Equipped.Length; i++)
            {
                result.Equipped[i] = SkillId.None;
            }
        }

        // Inventory
        result.Inventory.Clear();

        if (TryGetMap(slotsRoot, DatabaseKeys.Inventory, out var invDict))
        {
            //정렬해서 리스트로
            var temp = new List<(int idx, int id)>();

            foreach (var kv in invDict)
            {
                if (!int.TryParse(kv.Key, out int idx)) continue;
                int id = (int)ToLong(kv.Value, SkillId.None);
                temp.Add((idx, id));
            }

            temp.Sort((a, b) => a.idx.CompareTo(b.idx));
            for (int i = 0; i < temp.Count; i++)
            {
                result.Inventory.Add(temp[i].id);
            }
        }
        else if (TryGetList(slotsRoot, DatabaseKeys.Inventory, out var invList))
        {
            for (int i = 0; i < invList.Count; i++)
            {
                result.Inventory.Add((int)ToLong(invList[i], SkillId.None));
            }
        }

        return result;
    }


    private static bool TryGetDict(Dictionary<string, object> root, string key, out Dictionary<string, object> dict)
    {
        dict = null;
        if (root.TryGetValue(key, out var obj) && obj is Dictionary<string, object> d)
        {
            dict = d;
            return true;
        }
        return false;
    }
    private static bool TryGetMap(Dictionary<string, object> root, string key, out Dictionary<string, object> map)
    {
        map = null;
        if (!root.TryGetValue(key, out var obj) || obj == null) return false;

        if (obj is Dictionary<string, object> d1)
        {
            map = d1;
            return true;
        }

        // Dictionary<object, object> / Hashtable 케이스
        if (obj is IDictionary dict)
        {
            var converted = new Dictionary<string, object>(dict.Count);
            foreach (DictionaryEntry e in dict)
                converted[e.Key.ToString()] = e.Value;
            map = converted;
            return true;
        }

        return false;
    }
    private static bool TryGetList(Dictionary<string, object> root, string key, out IList list)
    {
        list = null;
        if (!root.TryGetValue(key, out var obj) || obj == null) return false;

        if (obj is IList l)
        {
            list = l;
            return true;
        }
        return false;
    }
    private static bool TryGetMapFromSnapshot(DataSnapshot snapshot, out Dictionary<string, object> root)
    {
        root = null;
        object v = snapshot?.Value;
        if (v == null) return false;

        if (v is Dictionary<string, object> d)
        {
            root = d;
            return true;
        }

        if (v is IDictionary dict)
        {
            var converted = new Dictionary<string, object>(dict.Count);
            foreach (DictionaryEntry e in dict)
            {
                converted[e.Key.ToString()] = e.Value;
            }
            root = converted;
            return true;
        }

        return false;
    }
    private static long GetLong(Dictionary<string, object> dict, string key, long def)
        => dict.TryGetValue(key, out var v) ? ToLong(v, def) : def;
    private static double GetDouble(Dictionary<string, object> dict, string key, double def)
        => dict.TryGetValue(key, out var v) ? ToDouble(v, def) : def;
    private static long ToLong(object v, long def)
    {
        if (v == null) return def;
        if (v is long l) return l;
        if (v is int i) return i;
        if (v is double d) return (long)d;
        if (long.TryParse(v.ToString(), out var p)) return p;
        return def;
    }
    private static double ToDouble(object v, double def)
    {
        if (v == null) return def;
        if (v is double d) return d;
        if (v is long l) return l;
        if (v is int i) return i;
        if (double.TryParse(v.ToString(), out var p)) return p;
        return def;
    }
    private static bool ToBool(object v, bool def)
    {
        if (v == null) return def;
        if (v is bool b) return b;
        if (v is long l) return l != 0;
        if (v is int i) return i != 0;
        if (v is double d) return Math.Abs(d) > 0.000001;
        if (bool.TryParse(v.ToString(), out var pb)) return pb;
        if (long.TryParse(v.ToString(), out var pl)) return pl != 0;
        return def;
    }
}
