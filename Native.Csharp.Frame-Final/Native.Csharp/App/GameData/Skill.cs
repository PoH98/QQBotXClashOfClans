using System;
using System.Xml.Serialization;

namespace Native.Csharp.App.GameData
{
    [XmlInclude(typeof(Critical))]
    [XmlInclude(typeof(Heal))]
    [XmlInclude(typeof(DoubleHit))]
    [Serializable]
    public abstract class Skill
    {
        public abstract string Name { get; }
        public abstract double[] TriggerPercentage { get; }
        public abstract double[] Amount { get; }
        public abstract int[] Price { get; }
        public abstract SkillType SkillType { get; }
    }

    public class Critical : Skill
    {
        public override string Name => "暴击";

        public override double[] TriggerPercentage => new double[] { 0.07, 0.10, 0.13, 0.16, 0.18 };

        public override double[] Amount => new double[] { 1.35, 1.37, 1.4, 1.43, 1.45 };

        public override SkillType SkillType => SkillType.Critical;

        public override int[] Price => new int[] { 100, 10000, 20000, 30000, 50000 };
    }

    public class Heal : Skill
    {
        public override string Name => "治疗";

        public override double[] TriggerPercentage => new double[] { 0.08, 0.11, 0.14, 0.18, 0.22 };

        public override double[] Amount => new double[] { 0.4, 0.44, 0.48, 0.5, 0.6 };

        public override SkillType SkillType => SkillType.Heal;

        public override int[] Price => new int[] { 100, 10000, 20000, 30000, 50000 };
    }

    public class DoubleHit : Skill
    {
        public override string Name => "连击";

        public override double[] TriggerPercentage => new double[] { 0.03, 0.04, 0.06, 0.08, 0.08 };

        public override double[] Amount => new double[] { 2, 2, 2, 2, 3 };

        public override SkillType SkillType => SkillType.DoubleHit;

        public override int[] Price => new int[] { 100, 10000, 20000, 30000, 50000 };
    }
}
