# 游戏展示
![image](https://github.com/skylecn/Battle/blob/master/gameshow.png)

# 入口
**Battle/Assets/Scenes/GameStart**：用Unity打开Battle工程，打开GameStart场景，启动


# 主要功能
一个简单的战斗框架，示例是简单的塔防玩法，怪物轮番进攻，主要演示战斗底层逻辑。资源用的简模。

实现了基础战斗的主要模块，角色，属性，技能，BUFF，被动技能，基础状态，AI。逻辑和和显示分离，方便战斗逻辑移植到服务器。


# 工程主要目录

**Battle**：Unity工程

**FGUIProject**：UI工程，用的FairyGUI。

**Tables**：配置表，用的luban导表工具，gen\_client导出脚本和配表数据到Unity工程

**Tools**：luban导表工具

**Battle/Assets/Scripts**：战斗及基础框架，详细可查看[Battle/Assets/Scripts/README.md](Battle/Assets/Scripts/README.md), 其中Collision2d由AI生成，主要实现了Circle,AABB,OBB碰撞检测和射线检测

# 主要插件
资源管理用YooAsset，配置表用的luban，行为树用NPBehave，UI用的FairyGUI

