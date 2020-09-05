﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.GameData
{
    class SharedData
    {
        internal static SharedData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SharedData();
                }
                return instance;
            }
        }
        internal List<Weapon> weapon = new List<Weapon>();

        internal Dictionary<long, List<GameMember>> gameMembers = new Dictionary<long, List<GameMember>>();

        internal Dictionary<Work, int> 工资 = new Dictionary<Work, int> {
            { Work.给野蛮人刷背, 100 },
            { Work.给弓箭手洗脚, 120 },
            { Work.给哥布林偷金币, 150 },
            { Work.给巨人当沙包, 180 },
            { Work.给炸弹人重组骨架, 200 },
            { Work.给法师搞爆炸头, 220 } ,
            { Work.给天使配眼镜, 250 },
            { Work.给龙灭火, 280 },
            { Work.给皮卡擦盔甲, 300 },
            { Work.给矿工点蜡烛, 320 },
            { Work.给雷龙充电, 350 },
            { Work.给大雪怪带孩子, 380 }
        };

        internal Dictionary<Work, int> 需要经验值 = new Dictionary<Work, int> {
            { Work.给野蛮人刷背, 500 },
            { Work.给弓箭手洗脚, 1500 },
            { Work.给哥布林偷金币, 2500 },
            { Work.给巨人当沙包, 4000 },
            { Work.给炸弹人重组骨架, 6500 },
            { Work.给法师搞爆炸头, 9500 },
            { Work.给天使配眼镜, 13000 },
            { Work.给龙灭火, 18000 },
            { Work.给皮卡擦盔甲, 23000 },
            { Work.给矿工点蜡烛, 30000 },
            { Work.给雷龙充电, 36500 },
            { Work.给大雪怪带孩子, 50000 }
        };

        internal string[] TreasureFindingSuccess =
        new string[]
        {
            "你在寻宝路上看见了个蓝色狸猫，他给了你一个圆柱体后就跳到个圈里消失了，你获得了%G%金币！",
            "你在寻宝路上看见了个戴皇冠的人，他身上不停冒出绿色的票卷，他给了你一张绿色的卷后就消失了，你拿去卖掉后获得了%G%金币！",
            "你在森林里迷路了，在那里你看见了个小房子，房子里传出了'面对恐惧的最好办法...'，你没继续听下去就把房子炸了，并且在那里找到了%G%金币，并且发现其实回家的路就被这破房子拦住了！",
            "你到海上去寻宝，回去的路上搭了个小船，有个老妇人在你隔壁咳嗽了一声，你没在意，下船后你开始了莫名的咳嗽并且开始发烧。你获得了%G%金币！",
            "你在森林里找到了个宝箱，然而有个在打篮球的男人在你回家的时候阻拦了你，你轻松把他的篮球踢飞后赶紧溜回了家。你获得了%G%金币！",
            "你经过了一个遗迹，找到了个石制面具，你把他卖给了个黄金头发的男人，你获得了%G%金币！",
            "你刚出门，邻居的老婆婆就把你叫住，她给了你个红苹果，你看过这个故事，便把老婆婆杀死，并且拿她的苹果卖给了路边戴红帽的小女孩，你获得了%G%金币！",
            "你到海上寻宝，路上看见了个举着个火炬的女性雕像，下船后你在路上捡到个黑色的小东西，里面塞满了大量印着某个人的纸张，上面全写了100$，你把这些破纸卖给了个捡破烂的，获得了%G%金币！",
            "你出门后发现今天在下雪，你门口站着个火柴在卖女孩，你跟她买了个女孩后把女孩拿去点燃了取暖，并且在融化的雪地里找到了个小宝箱，获得了%G%金币！",
            "你在寻宝路上发现了地上充满了坑，你生气的破骂了一顿，这肯定又是某主播挖的坑！幸运的是，你在其中一个坑里挖到了些宝藏，你获得了%G%金币！",
            "在雪花飘飘，你在北风萧萧里找到了一大箱的费玉(青)，你拿去卖掉后获得了%G%金币！",
            "你到了祖安去寻宝，见到个白发男人在卖鱼，你跟他买了个内裤后转卖，获得了%G%金币！",
            "在路上你遇见了贞子，你绑着贞子剪了头发后发现变成了美女就拿去卖给了奴隶商人，你获得了%G%金币！",
            "你到了隔壁的游戏村庄，这里的村民都是用游戏进行的决斗，你决定在那边跟他们Van游戏，成功赚取了%G%的金币回家！",
            "你:\n金币：突然多出了%G%金币！"
        };

        internal string[] TreasureFindingFailed =
        new string[]
        {
            "你到森林里发现了个巨龙，你使用了大量精力用煮菜刀把龙打死了，当你打开宝箱时箱子突然跳出了大量的杂草，你突然惊醒发现你满口的杂草，只好自讨无趣回家了！",
            "你在寻宝路上找到了个老旧的城堡，正当你想要进去的时候你看见了好几个天使在拉着个野蛮人往这里走来，你躲起来后看见天使把野蛮人拉进去城堡里，没多久就发出了惨叫，你吓得赶紧逃离了这个地方！",
            "你在路上看见了口井，你往下面丢了个金币希望可以在今天内寻到一些好的宝藏，井底回应了你的愿望，发出了呱呱呱的叫声！",
            "你在路上看见了个老巫师在点头，你跟他一起点头，到了傍晚后你觉得颈部酸痛就回家了",
            "你找到了传说中的龙王之穴，里面的龙王告诉你他知道宝藏的地点，然而他一直滔滔不绝的一直在说他以前的辉煌事迹说了9天9夜，你听不下去只好回家了！",
            "你在路上看见个帅气的男人在不停的抖动肩膀，他似乎放出了魔法控制了你的身体，你不由自主的跟他一起抖动肩膀，直到隔天你好不容易挣脱了魔法的控制后赶紧逃回了家！",
            "你在个美好的早晨突然看见个包着头巾的男人喊着去东北玩泥巴，还说什么他大连没有家，你觉得他是疯子就只好呆在家里的被窝里瑟瑟发抖！",
            "你躺在床上玩了一天的手机，并且在一个群聊里的机器人寻找到了大量宝藏！",
            "你走到了个名叫滑尾的奇怪村庄，发现这里正在与一名奇怪金发男人发生战争，金发男人疯狂的往村子里丢各种奇怪会爆炸的纸，而滑尾村庄则疯狂的在向金发男人使用会发光的小长方形玩意反击。你为了避免被战火烧及赶紧跑回了家！",
            "你到了个小村庄，发现这里已经被强盗洗劫一空，而强盗似乎还在不远处在搬运窃取到的厕纸，你吓得赶紧跑回了家！",
            "在寻宝途中你看见了一群黑人穿着西装对着你笑，你不知道他们在干嘛走了过去，结果被他们一把抓进了棺材里，等你回过神来你已经被安葬在了老家！",
            "你到了个古代遗迹，找到了一本古代的魔法卷抽，似乎是可以把全世界的男人变成女性，你学习后感觉奇怪的知识增加了！",
            "在出门之前，你到了个餐馆吃了顿生鱼片当早餐，突然想到个问题，生鱼片为什么是生的？结果就这样你在餐馆里想了三天！",
            "到了森林里，你发现了个淡黄的长裙，蓬松的头发的男人，他发现你后大声的喊I potato you，吓得你连滚带爬的跑回了家！",
            "今天你觉得一点干劲都没有，还是明天再努力吧！"
        };

        internal static SharedData instance;
    }
}