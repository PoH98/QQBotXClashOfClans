# QQBotXClashOfClans
QQ群管理连接部落冲突API进行各种群里管理操作
## 此插件适用于酷Q机器人，基于[Native.Csharp](https://github.com/Jie2GG/Native.Csharp.Frame)所编写合并了[CocNET](https://github.com/smietanka/CocNET)
* 此插件全靠config.ini操作，每次修改了config.ini请重载插件！
* 由于本人专门写有'features'的软件，所以发现了新的'features'还请汇报
* 欢迎所有人尽情修改
* config.ini里的[部落冲突] token 请到[部落冲突developers](https://developer.clashofclans.com/#/)里申请账号获取
* config.ini里的[部落冲突] Clan_ID 是你部落的标签
* config.ini里的[自动回复] 格式是 关键词 = 回复
* config.ini里的[禁止词句] 格式是 禁止词句 = 0，机器人会自动撤回包含禁止的词句的发言（管理员&群主除外）
* Townhall.ini是用于/审核的功能，发送`/审核 #玩家标签`后，就会自动呼叫部落冲突api进行与Townhall.ini里相应的大本营兵种等级进行比对后，显示还缺多少级才通过审核