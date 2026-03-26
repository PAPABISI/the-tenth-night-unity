# 《第十夜》Unity 客户端

这是《第十夜》项目的 Unity 前端原型，用于联调后端 API 并演示基础可玩流程（创建房间、开始游戏、抽卡、出牌、夜晚意图提交、阶段推进）。

---

## 一、环境要求

- Unity
- TextMeshPro（Unity 内置）
- 可访问后端服务（本地或远程）

---

## 二、项目目标（当前版本）

当前客户端目标是打通最小可演示闭环，而非完整美术/交互版本：

- 创建房间（CreateRoom）
- 加入房间（JoinRoom）
- 开始游戏（StartGame）
- 阶段推进（NextPhase）
- 白天抽卡（F）
- 手牌展示与点击出牌（Use Card）
- 夜晚意图提醒（Q -> Steal / Not Steal）
- 2D 本地移动（WASD）与近距离互动（E）

---

## 三、后端连接配置

请在网络/API 脚本中确认后端地址（Base URL），例如：

- `http://localhost:5000`
- 或你的服务器地址

如无法联调，请优先检查：
1. 后端是否已启动  
2. Unity 与后端端口是否一致  
3. 防火墙/跨域/代理设置是否拦截请求

---

## 四、运行步骤

1. 打开 Unity 项目并加载主场景
2. 点击 Play  
3. 按顺序操作：
   - `CreateRoom`
  - `JoinRoom`
   - `StartGame`
  - 使用 `WASD` 移动
   - （到 `DayExploration`）按 `F` 抽卡
  - 靠近玩家按 `E` 触发互动提示
   - 点击手牌按钮出牌
  - 推进到 `NightPhase` 后按 `Q` 打开夜晚面板并提交意图
   - `NextPhase` 进入下一轮继续测试

---

## 五、按键与交互说明

- `W / A / S / D`：2D 移动
- `F`：近距离开宝箱抽卡（DayExploration）
- `E`：近距离玩家互动
- `Q`：打开夜晚意图面板（NightPhase）
- UI 按钮：
  - `CreateRoom`：创建房间
  - `StartGame`：开始对局
  - `NextPhase`：推进阶段
  - 手牌按钮：使用对应卡牌

---

## 六、当前已实现内容

- 基础 UI 面板联动
- 房间创建/加入/开局流程联调
- 状态拉取与核心字段显示（Phase / HP / Round 等）
- 手牌动态渲染（显示牌名，不再显示 UUID）
- 出牌请求与夜晚意图请求打通
- 2D 角色移动脚本（LocalPlayer2DMovement）
- 2D 相机跟随脚本（CameraFollow2D）
- 最近目标互动检测（InteractionDetector）
- 远端玩家实体同步可视化（RemotePlayers2DPresenter）
- 本地玩家位置上报（NetworkPositionSync2D -> /action/move）

---

## 七、已知限制 / 后续计划

- 交互检测与场景碰撞（Chest/Player Layer）仍可继续完善
- 场景内其他玩家实体目前依赖手工放置或后续网络同步
- 错误提示已有基础 UI Toast，后续可升级统一提示系统

---

## 九、玩家实体同步可视化（2D）

新增脚本：

- `Assets/Scripts/Game/RemotePlayers2DPresenter.cs`
- `Assets/Scripts/Game/NetworkPositionSync2D.cs`

功能：

- 根据 `store.LatestState.publics` 自动创建/更新/移除远端玩家 2D 实体
- 自动跳过本地玩家（`LocalPlayerId`）
- 使用环形布局展示远端玩家
- 存活玩家为绿色，死亡玩家为灰色
- 自动设置到 `Player` Layer，可直接被 `InteractionDetector` 的 E 互动检测命中
- 本地玩家位置按间隔上报到后端，远端实体优先按网络坐标渲染

使用步骤：

1. 在场景中新建空对象并挂载 `RemotePlayers2DPresenter`
2. 绑定 `GameStateStore`
3. 可选绑定 `spawnRoot`（不绑则挂在当前对象下）
4. 可选绑定 `remotePlayerPrefab`（不绑将运行时生成默认圆形标记+TMP文字）
5. 确保 Layer 中存在 `Player`（或在脚本里改 `playerLayerName`）
6. 给本地玩家或管理对象挂 `NetworkPositionSync2D`，并绑定 `localPlayer` + `GameController`
- 美术表现、动画反馈、音效与完整引导尚未完成
- 目标选择与复杂规则提示可进一步优化

---

## 八、相关仓库

- 后端仓库：`https://github.com/PAPABISI/The-tenth-night.git`
