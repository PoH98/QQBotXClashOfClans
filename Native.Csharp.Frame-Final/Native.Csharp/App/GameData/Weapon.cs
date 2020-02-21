using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.GameData
{
    public interface Weapon
    {
        int minDamage { get; }
        int maxDamage { get; }
        int maxHP { get; }
        string Name { get; }
        int Price { get; }
    }
    [Serializable]
    public class None : Weapon
    {
        public int minDamage => 1;

        public int maxDamage => 10;

        public string Name => "小拳拳";

        public int maxHP => 100;

        public int Price => 0;
    }

    [Serializable]
    public class Lighting : Weapon
    {
        public int minDamage => 150;

        public int maxDamage => 200;

        public string Name => "闪电";

        public int maxHP => 1100;

        public int Price => 10000;
    }

    [Serializable]
    public class Magic : Weapon
    {
        public int minDamage => 120;

        public int maxDamage => 200;

        public string Name => "火球";

        public int maxHP => 1500;

        public int Price => 10000;
    }

    [Serializable]
    public class Pekka : Weapon
    {
        public int minDamage => 180;

        public int maxDamage => 250;

        public string Name => "皮卡盔甲";

        public int maxHP => 3500;

        public int Price => 20000;
    }

    [Serializable]
    public class Knive : Weapon
    {
        public int minDamage => 200;

        public int maxDamage => 330;

        public string Name => "蛮王刀子";

        public int maxHP => 3600;

        public int Price => 25000;
    }
    [Serializable]
    public class XBow : Weapon
    {
        public int minDamage => 350;

        public int maxDamage => 500;

        public string Name => "女王X弩";

        public int maxHP => 2500;

        public int Price => 30000;
    }

    [Serializable]
    public class Inferno : Weapon
    {
        public int minDamage => 300;
        public int maxDamage => 600;
        public string Name => "巨型地狱塔";
        public int maxHP => 2600;
        public int Price => 35000;
    }

    [Serializable]
    public class UltraWeapon1 : Weapon
    {
        public int minDamage => 2000;

        public int maxDamage => 2600;

        public int maxHP => 30000;

        public string Name => "粒子高射炮";

        public int Price => -1;
    }

    [Serializable]
    public class UltraWeapon2 : Weapon
    {
        public int minDamage => 2000;

        public int maxDamage => 2600;

        public int maxHP => 30000;

        public string Name => "浩劫反物质火炮";

        public int Price => -1;
    }
}
