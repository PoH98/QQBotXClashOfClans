﻿using System;
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
    [XmlInclude(typeof(MDSmartStick))]
    [XmlInclude(typeof(UltraWeapon1))]
    [XmlInclude(typeof(UltraWeapon2))]
    [XmlInclude(typeof(Killer))]
    [XmlInclude(typeof(BDrag))]
    [Serializable]
    public abstract class Weapon
    {
        public abstract int minDamage { get; }
        public abstract int maxDamage { get; }
        public abstract int maxHP { get; }
        public abstract string Name { get; }
        public abstract int Price { get; }
        public abstract TimeSpan GetAwaitTime { get; }
    }

    [Serializable]
    public class None : Weapon
    {
        public override int minDamage => 54;

        public override int maxDamage => 88;

        public override string Name => "小拳拳";

        public override int maxHP => 440;

        public override int Price => 0;

        public override TimeSpan GetAwaitTime => new TimeSpan(0, 9, 0);
    }

    [Serializable]
    public class Lighting : Weapon
    {
        public override int minDamage => 150;

        public override int maxDamage => 225;

        public override string Name => "闪电";

        public override int maxHP => 1100;

        public override int Price => 10000;

        public override TimeSpan GetAwaitTime => new TimeSpan(0, 35, 0);
    }

    [Serializable]
    public class Magic : Weapon
    {
        public override int minDamage => 120;

        public override int maxDamage => 200;

        public override string Name => "火球";

        public override int maxHP => 1500;

        public override int Price => 10000;

        public override TimeSpan GetAwaitTime => new TimeSpan(0, 35, 0);
    }
    [Serializable]
    public class BDrag : Weapon
    {
        public override int minDamage => 200;

        public override int maxDamage => 280;

        public override int maxHP => 3000;

        public override string Name => "飞龙宝宝";

        public override int Price => 20000;

        public override TimeSpan GetAwaitTime => new TimeSpan(0, 55, 0);
    }
    [Serializable]
    public class Pekka : Weapon
    {
        public override int minDamage => 180;

        public override int maxDamage => 250;

        public override string Name => "皮卡盔甲";

        public override int maxHP => 3500;

        public override int Price => 20000;

        public override TimeSpan GetAwaitTime => new TimeSpan(0, 55, 0);
    }

    [Serializable]
    public class Killer : Weapon
    {
        public override int minDamage => 950;

        public override int maxDamage => 1200;

        public override int maxHP => 800;

        public override string Name => "超级杀手";

        public override int Price => 35000;

        public override TimeSpan GetAwaitTime => new TimeSpan(1, 35, 0);
    }

    [Serializable]
    public class Knive : Weapon
    {
        public override int minDamage => 200;

        public override int maxDamage => 330;

        public override string Name => "蛮王刀子";

        public override int maxHP => 3600;

        public override int Price => 25000;

        public override TimeSpan GetAwaitTime => new TimeSpan(1, 35, 0);
    }

    [Serializable]
    public class XBow : Weapon
    {
        public override int minDamage => 360;

        public override int maxDamage => 510;

        public override string Name => "女王X弩";

        public override int maxHP => 2500;

        public override int Price => 35000;

        public override TimeSpan GetAwaitTime => new TimeSpan(2, 0, 0);
    }

    [Serializable]
    public class Inferno : Weapon
    {
        public override int minDamage => 300;
        public override int maxDamage => 600;
        public override string Name => "巨型地狱塔";
        public override int maxHP => 2600;
        public override int Price => 40000;

        public override TimeSpan GetAwaitTime => new TimeSpan(2, 50, 0);
    }

    [Serializable]
    public class MDSmartStick : Weapon
    {
        public override int minDamage => 400;
        public override int maxDamage => 580;
        public override string Name => "马德牌智能拐杖";
        public override int maxHP => 3000;
        public override int Price => 55000;

        public override TimeSpan GetAwaitTime => new TimeSpan(4, 0, 0);
    }

    [Serializable]
    public class UltraWeapon1 : Weapon
    {
        public override int minDamage => 2000;

        public override int maxDamage => 2600;

        public override int maxHP => 30000;

        public override string Name => "粒子高射炮";

        public override int Price => -1;

        public override TimeSpan GetAwaitTime => new TimeSpan(365,0, 0, 0);
    }

    [Serializable]
    public class UltraWeapon2 : Weapon
    {
        public override int minDamage => 2000;

        public override int maxDamage => 2600;

        public override int maxHP => 30000;

        public override string Name => "浩劫反物质火炮";

        public override int Price => -1;

        public override TimeSpan GetAwaitTime => new TimeSpan(365, 0, 0, 0);
    }
}
