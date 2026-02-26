using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using cfg;
using Game.CityBattle.Logic;
using SimpleJSON;
using YooAsset;

public class CfgData
{
    static string DATA_PATH = "Assets/GameRes/Data/";

    public static cfg.Tables Tables;

    public static void Load()
    {
        Tables = new cfg.Tables(LoadJson);
    }

    static string LoadFile(string fileName)
    {
        var assetHandle = YooAssets.LoadAssetSync<TextAsset>($"{DATA_PATH}{fileName}");
        var textAsset = assetHandle.AssetObject as TextAsset;
        var text = textAsset.text;
        assetHandle.Release();

        return text;
    }

    private static JSONNode LoadJson(string file)
    {
        return JSON.Parse(File.ReadAllText($"{DATA_PATH}/{file}.json", System.Text.Encoding.UTF8));
    }

    public static BattleInstance GetBattleInstance(int id)
    {
        return Tables.TbBattleInstance.Get(id);
    }

    public static CharacterData GetCharacter(int id)
    {
        return Tables.TbCharacterData.Get(id);
    }

    public static SkillData GetSkill(int id)
    {
        return Tables.TbSkillData.Get(id);
    }

    public static BulletData GetBullet(int id)
    {
        return Tables.TbBulletData.Get(id);
    }

    public static Effect GetEffect(int id)
    {
        return Tables.TbEffect.Get(id);
    }

    public static SkillAction GetSkillAction(int id)
    {
        return Tables.TbSkillAction.Get(id);
    }

    public static BuffData GetBuff(int id)
    {
        return Tables.TbBuffData.Get(id);
    }

    public static BuffEffectData GetBuffEffect(int id)
    {
        return Tables.TbBuffEffectData.Get(id);
    }

    public static PassiveSkillData GetPassiveSkill(int id)
    {
        return Tables.TbPassiveSkillData.Get(id);
    }
}