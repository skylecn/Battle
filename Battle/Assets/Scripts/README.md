# Assets/Scripts 代码结构说明

本文档描述 `Assets/Scripts` 下各子目录的代码功能与职责。

---

## Collision2d

**功能**：2D 物理与碰撞系统（AI生成，不依赖 Unity Physics，但接口类似Physics）。

- **PhysicsCore / PhysicsData / PhysicsEngine**：物理引擎核心，包含Circle、AABB、OBB碰撞检测和射线检测。用Layer和SpatialGrid优化查询效率
- **Example**：示例与测试脚本。

---

## Game

游戏主逻辑与框架代码，按模块划分子目录。

---

### Game/Base

**功能**：游戏基础框架与通用工具。

| 子目录/文件 | 功能 |
|------------|------|
| **Audio** | 音频管理。 |
| **ClientData** | 客户端本地数据工具。 |
| **Log** | 日志级别管理和统一输出。 |
| **Notification** | 自定义消息转发。 |
| **ObjectPool** | 对象池和实例内存池。 |
| **Schedule** | 游戏流程与步骤：GameMain（入口）、ResUpdateStep、LoginStep、HomeStep，启动→资源更新→登录→主城等流程。 |
| **Timer** | 定时器。 |
| **Utils** | 工具类。 |
| **VFX** | 特效管理和内存池。 |

---

### Game/CityBattle

**功能**：战斗逻辑。

- **根目录脚本**
  - **BattleController**：战斗层事件中转，用于逻辑与表现解耦。
  - **CityBattleManager**：战斗总管理器，负责初始化、Update、相机，并协调 EntityManager、BattleWorld。
  - **CityBattleCameraController**：相机控制。

- **Entity**（表现实体）
  - **EntityManager**：管理角色与子弹。由BattleController事件驱动。
  - **CharacterEntity**：角色。
  - **BulletEntity**：子弹。
  - **MapEntity**：实体基类。
  - **CharacterEntityDebuger**：角色调试工具（AI，物理碰撞等）。

- **Logic**（战斗逻辑）
  - **BattleWorld**：战斗管理，负责初始化、Tick、状态、物理世界（PhysicsWorld）、输入。
  - **AI**：行为树与 AI 行为
    - CharacterBehaviourTree、EnemyBehaviourTree、BuildingBehaviourTree：角色/敌人/建筑行为树。
    - CharBlackboard：角色黑板数据。
    - AttackAction、MoveToTargetAction：攻击、朝目标移动等行为节点。
  - **Attr**：属性与修改
    - Attr、AttrModifier、CharacterAttr：基础属性与修改器、角色属性封装。
  - **Battle**：技能、Buff、子弹等战斗核心
    - BattleDefine、BattleEngine：常用枚举定义。
    - SkillInstance、PassiveSkillInstance：技能与被动技能。
    - BuffInstance、BuffEffect*：Buff 及各类效果（伤害、属性修改、眩晕、控制免疫等）。
    - Bullet：子弹。
  - **Obj**：战斗对象
    - Character、CharacterAI、CharacterAttr、CharacterSkill、CharacterPassiveSkill、CharacterBuff、CharacterState、CharacterTag、CharacterShow：角色在逻辑层的完整抽象（AI，属性、技能、Buff、状态、标签、表现等）。

- **UI**（场景 UI）
  - **CharacterHpBar**：角色血条。
  - **MapEntityUIComponent**：地图实体上的 UI 组件基类。

---

### Game/Data

**功能**：配置与数据加载。

- **DataManager.cs**：访问配表数据。
- **cfg/**：自动导出的配表脚本文件。

---

### Game/UI

**功能**：全局 UI 框架与各功能界面。

- **Base**（UI 框架）
  - **UIManager**：窗口与资源管理，使用了FairyGUI。

---

## 模块依赖关系概览

- **GameMain** 启动 → **GameStepManager** 驱动流程（ResUpdate → Login → Home）。
- **Home** 等步骤中进入战斗 → **CityBattleManager** 初始化地图与 **BattleWorld**，**EntityManager** 绑定 **BattleController** 事件。

如需了解某个具体类或流程，可先在本目录下按上述模块定位到对应子目录再查找类名。
