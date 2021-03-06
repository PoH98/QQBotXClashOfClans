using System;

namespace Native.Csharp.App.Bot.GameData
{
    [Serializable]
    public class 西瓜 : Element
    {
        public override int ID => 0;

        public override decimal DropRate => 0.5M;

        public override string Name => "西瓜";

        public override ElementType ElementType => ElementType.Attack;
    }
    [Serializable]
    public class 灯笼 : Element
    {
        public override int ID => 1;

        public override decimal DropRate => 0.3M;

        public override string Name => "灯笼";

        public override ElementType ElementType => ElementType.Attack;
    }
    [Serializable]
    public class 人脸石头 : Element
    {
        public override int ID => 2;

        public override decimal DropRate => 0.15M;

        public override string Name => "人脸石头";

        public override ElementType ElementType => ElementType.Attack;
    }
    [Serializable]
    public class 陨石碎片 : Element
    {
        public override int ID => 3;

        public override decimal DropRate => 0.08M;

        public override string Name => "陨石碎片";

        public override ElementType ElementType => ElementType.Attack;
    }


    [Serializable]
    public class 红巨星 : Element
    {
        public override int ID => 4;

        public override decimal DropRate => 0.02M;

        public override string Name => "红巨星";

        public override ElementType ElementType => ElementType.Attack;
    }
}
