using System;

namespace Native.Csharp.App.Bot.GameData
{
    [Serializable]
    public class 蝴蝶 : Element
    {
        public override int ID => 5;

        public override decimal DropRate => 0.5M;

        public override string Name => "蝴蝶";

        public override ElementType ElementType => ElementType.Defence;
    }

    [Serializable]
    public class 人皮 : Element
    {
        public override int ID => 6;

        public override decimal DropRate => 0.3M;

        public override string Name => "人皮";

        public override ElementType ElementType => ElementType.Defence;
    }

    [Serializable]
    public class 伽马射线 : Element
    {
        public override int ID => 7;

        public override decimal DropRate => 0.15M;

        public override string Name => "伽马射线";

        public override ElementType ElementType => ElementType.Defence;
    }
    [Serializable]
    public class 十万羊驼 : Element
    {
        public override int ID => 8;

        public override decimal DropRate => 0.08M;

        public override string Name => "十万羊驼";

        public override ElementType ElementType => ElementType.Defence;
    }
    [Serializable]
    public class 蓝巨星 : Element
    {
        public override int ID => 9;

        public override decimal DropRate => 0.02M;

        public override string Name => "蓝巨星";

        public override ElementType ElementType => ElementType.Defence;
    }
}
