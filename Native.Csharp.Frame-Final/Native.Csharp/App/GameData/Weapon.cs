using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Native.Csharp.App.GameData
{
    [XmlInclude(typeof(None))]
    [XmlInclude(typeof(Lighting))]
    [XmlInclude(typeof(Magic))]
    [XmlInclude(typeof(Pekka))]
    [XmlInclude(typeof(Knive))]
    [XmlInclude(typeof(XBow))]
    [XmlInclude(typeof(Inferno))]
    [XmlInclude(typeof(UltraWeapon1))]
    [XmlInclude(typeof(UltraWeapon2))]
    [Serializable]
    public abstract class Weapon
    {
        public abstract int minDamage { get; }
        public abstract int maxDamage { get; }
        public abstract int maxHP { get; }
        public abstract string Name { get; }
        public abstract int Price { get; }
    }


    [Serializable]
    public class None : Weapon
    {
        public override int minDamage => 1;

        public override int maxDamage => 10;

        public override string Name => "小拳拳";

        public override int maxHP => 100;

        public override int Price => 0;
    }

    [Serializable]
    public class Lighting : Weapon
    {
        public override int minDamage => 150;

        public override int maxDamage => 200;

        public override string Name => "闪电";

        public override int maxHP => 1100;

        public override int Price => 10000;
    }

    [Serializable]
    public class Magic : Weapon
    {
        public override int minDamage => 120;

        public override int maxDamage => 200;

        public override string Name => "火球";

        public override int maxHP => 1500;

        public override int Price => 10000;
    }

    [Serializable]
    public class Pekka : Weapon
    {
        public override int minDamage => 180;

        public override int maxDamage => 250;

        public override string Name => "皮卡盔甲";

        public override int maxHP => 3500;

        public override int Price => 20000;
    }

    [Serializable]
    public class Knive : Weapon
    {
        public override int minDamage => 200;

        public override int maxDamage => 330;

        public override string Name => "蛮王刀子";

        public override int maxHP => 3600;

        public override int Price => 25000;
    }
    [Serializable]
    public class XBow : Weapon
    {
        public override int minDamage => 350;

        public override int maxDamage => 500;

        public override string Name => "女王X弩";

        public override int maxHP => 2500;

        public override int Price => 30000;
    }

    [Serializable]
    public class Inferno : Weapon
    {
        public override int minDamage => 300;
        public override int maxDamage => 600;
        public override string Name => "巨型地狱塔";
        public override int maxHP => 2600;
        public override int Price => 35000;
    }

    [Serializable]
    public class UltraWeapon1 : Weapon
    {
        public override int minDamage => 2000;

        public override int maxDamage => 2600;

        public override int maxHP => 30000;

        public override string Name => "粒子高射炮";

        public override int Price => -1;
    }

    [Serializable]
    public class UltraWeapon2 : Weapon
    {
        public override int minDamage => 2000;

        public override int maxDamage => 2600;

        public override int maxHP => 30000;

        public override string Name => "浩劫反物质火炮";

        public override int Price => -1;
    }
}
