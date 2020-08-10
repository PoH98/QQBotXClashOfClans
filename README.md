# QQBotXClashOfClans
QQ群管理连接部落冲突API进行各种群里管理操作
## 此插件适用于酷Q机器人，基于[Native.Csharp](https://github.com/Jie2GG/Native.Csharp.Frame)所编写合并了[CocNET](https://github.com/smietanka/CocNET)
* 此插件全靠config.ini操作，每次修改了config.ini请重载插件！
* 由于本人专门写有'features'的软件，所以发现了新的'features'还请汇报
* 欢迎所有人尽情修改
* config.ini里的[部落冲突] token 请到[部落冲突developers](https://developer.clashofclans.com/#/)里申请账号获取或者让它自动生成
* config.ini里的[部落冲突] Api邮箱 是登入[部落冲突developers](https://developer.clashofclans.com/#/)的邮箱，用于自动生成token
* config.ini里的[部落冲突] Api密码 是登入[部落冲突developers](https://developer.clashofclans.com/#/)的密码，用于自动生成token
* config.ini里的[部落冲突] 群号=#部落标签 将会绑定群为该部落以便进行其他操作例如查看部落战，或者群管理员和群主使用`/绑定群 #部落标签`即可
* config.ini里的[自动回复] 格式是 关键词 = 回复
* config.ini里的[禁止词句] 格式是 禁止词句 = 0，机器人会自动撤回包含禁言，0可以改变为任何数值，这个为禁言秒数（管理员&群主除外，以及加群超过3个月者除外）
* Townhall.ini是用于/审核的功能，发送`/审核 #玩家标签`后，就会自动呼叫部落冲突api进行与Townhall.ini里相应的大本营兵种等级进行比对后，显示还缺多少级才通过审核
* 由于本人很懒，所以设置的UI并不友好，还请见谅

# 由于酷Q的跑路，目前本插件将公开dll和json下载，以便可使用[mirai.native](https://github.com/iTXTech/mirai-native)进行继续运行
## 使用方式: 
* 下载[MiraiOK](http://t.imlxy.net:64724/mirai/MiraiOK/miraiOK_windows_386.exe)
* 打开MiraiOK后等待下载相关资料完成
* 关闭MiraiOK
* 下载[Mirai.Native](https://github.com/iTXTech/mirai-native/releases/download/v1.8.2/mirai-native-1.8.2.jar)
* 把Mirai.Native丢到Plugin文件夹内
* 打开Mirai.OK，让Native创建好所需文件夹和文件
* 到本插件Release页面下载dll和json
* 把东西丢到Plugins\MiraiNative\Plugins里面
* 再关闭Mirai.OK后重新打开，登入账号即可