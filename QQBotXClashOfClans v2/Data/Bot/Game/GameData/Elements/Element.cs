using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace QQBotXClashOfClans_v2.GameData
{
    [XmlInclude(typeof(西瓜))]
    [XmlInclude(typeof(灯笼))]
    [XmlInclude(typeof(人脸石头))]
    [XmlInclude(typeof(陨石碎片))]
    [XmlInclude(typeof(红巨星))]
    [XmlInclude(typeof(蝴蝶))]
    [XmlInclude(typeof(人皮))]
    [XmlInclude(typeof(伽马射线))]
    [XmlInclude(typeof(十万羊驼))]
    [XmlInclude(typeof(蓝巨星))]
    [Serializable]
    public abstract class Element
    {
        public abstract int ID { get; }
        public abstract decimal DropRate { get; }
        public abstract string Name { get; }
        public abstract ElementType ElementType { get; }
    }

    public enum ElementType
    {
        Attack,
        Defence
    }
}
