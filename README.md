# Bounce（物理彈射 Roguelike 練習專案）

一個自主學習的 Unity 2D Roguelike 練習專案，核心玩法是「拖曳瞄準、發射彈跳」的物理戰鬥（類似 Peglin 的彈射戰鬥 + Slay the Spire 的分支路線與遺物養成）。獨立開發，使用 C#、Unity 2D Physics 與 ScriptableObject 資料驅動架構。

> 這個專案的重點是把 Roguelike deckbuilder 常見的核心迴圈——戰鬥、遺物（Relic）、狀態效果（Effect）、商店、事件、地圖路線——完整跑一輪，並且把彈射物理的軌跡預測、地形交互、反彈計算這種比較硬核的數值/物理邏輯也做進去，而不是只做表面的回合制 UI。

## 專案亮點

- **拖曳彈射 + 反射軌跡預測系統**：玩家拖曳瞄準時，即時模擬球體多次反彈（`CircleCast` + `Vector2.Reflect`）、地形對速度的持續影響、以及碰到敵人/牆壁時的軌跡中斷，並把預測結果畫成一條會彎曲的預測線（分段取樣避免軌跡點過密），同時處理「反彈次數用完」「命中敵人提前結束」「地形讓預測失效改用箭頭指示」等多種分支情境。
- **地形（Obstacle）交互系統**：用 `ITerrainZone` 介面統一處理「互斥型」（Ice 減速、Slime 黏滯——同時只有 priority 最高的生效）與「疊加型」（BlackHole 引力——多個同時生效）兩種地形邏輯，並由 `TerrainResolver` 在每個物理步驟即時解算目前作用中的互斥地形，同時支援 `IBounceModifier`（Rubber 障礙物加成反彈力道）疊加到碰撞反應上。
- **事件驅動的 Relic（遺物）系統**：`RelicObject` 用一組 virtual hook（`OnHit` / `OnDealDamage` / `OnTakeDamage` / `OnHeal` / `OnSummon` / `OnTurnStart` / `OnTakeTurn` / `OnTurnEnd`）掛勾進整個戰鬥流程，`RelicHolder` 統一廣播、`RelicManager` 追蹤跨遺物共用的計數器（Hit / Bounce / Round），讓像 Catalyst（每 7 次擊中引爆持續傷害）這種「數值累積型」遺物可以純用計數器 + 取餘數實作，UI 也會即時訂閱同一份計數更新顯示與觸發時的發光特效。
- **狀態效果（Buff/Debuff/Dot）疊層系統**：`Stats.ApplyEffect` 依 `EffectObject.stackState` 分成三種疊層策略（`Only` 只刷新持續時間、`Separate` 同持續時間才疊加、`Merge` 疊層數直接等於持續時間），並用介面（`IOnHitEffect` / `IOnTakeTurn` / `IOnHeal` 等）讓效果自己決定要在什麼時機觸發，不用在 Stats 裡寫死每種效果的邏輯。
- **敵人技能 + 預覽系統**：`EnemySkill` 抽象類別統一管理「計算技能資料 → 顯示預覽 → 更新預覽位置/旋轉 → 觸發動畫 → 執行技能」的流程，Slime（衝刺 / 跳躍砸地）、PlagueDoctor（揮砍 / 尖叫）各自覆寫技能資料計算與預覽更新邏輯，讓玩家在敵人出手前就能看到攻擊範圍預判。
- **程序化分支路線地圖**：`MapGenerator` 依權重隨機生成節點類型（Battle / Elite / Event / Shop / Rest / Boss），並用「候選連線 + 寬度差調整連線機率」的演算法讓相鄰層節點盡量都能連到下一層、同時保留隨機分支感，搭配像素對齊定位（避免 UI 破圖）與自動捲動到最新解鎖層。
- **多語言 + 連結式術語系統**：物品/遺物/效果描述用 JSON 管理中英文，描述文字裡可以用 `<link=Glossary/Hit>` 這種標籤互相連結到其他術語，`DescriptionsListController` 會遞迴展開所有被連結到的術語（並用 `HashSet` 防止同一個 ID 重複展開），滑鼠懸停在遺物/效果圖示上就能看到完整的關聯說明堆疊顯示。
- **ScriptableObject 驅動的事件框架**：`EventObject` 抽象類別統一「顯示選項 → 等待玩家選擇 → 依結果分支」的流程，商店（`ShopEvent` → 生成 `ShopController` 進商店互動）、休息（`RestEvent` 三選一：回血/加血量上限/加攻擊）、記分板小遊戲（`ScoreBoardEvent` 限定 3 次射擊、累加/相乘分數、依目標分數決定獎懲）都是同一套框架的不同實作。

## 完成的系統清單

| # | 系統 |
|---|------|
| 1 | 回合制戰鬥框架（`BattleManager` 控制流程、`IBattleEntity`/`ITurnBase` 統一實體介面）
| 2 | 物理彈射戰鬥（拖曳瞄準、反射軌跡預測、多次反彈上限） 
| 3 | 地形/障礙物系統（互斥型 vs 疊加型 zone、優先度解算、反彈力道加成） 
| 4 | 敵人技能系統（技能預覽、動畫觸發、各敵人自訂技能邏輯） 
| 5 | Relic 遺物系統（事件掛勾、計數器型數值遺物、觸發特效 UI） 
| 6 | Effect 狀態效果系統（Buff/Debuff/Dot、三種疊層模式） 
| 7 | 程序化分支路線地圖生成 
| 8 | ScriptableObject 事件框架（休息/商店/記分板小遊戲） 
| 9 | 商店系統（購買道具/遺物、金錢驗證、隨機商品池）
| 10 | 藥水（Potion）系統 
| 11 | 召喚物系統（`Summonable` 生命週期註冊/自動清除） 
| 12 | 多語言在地化系統（JSON + 連結式術語互相跳轉）
| 13 | 描述/提示 UI 系統（滑鼠懸停顯示、遞迴連結展開防重複） 
| 14 | 記分板小遊戲事件（射擊命中累加/相乘分數） 

## 架構總覽

```
Assets/Scripts/
├── BattleManager.cs, ITurnBase.cs, IBattleEntity.cs
│                        # 回合制戰鬥主迴圈與實體/物理註冊
├── GameManager.cs, BattleLevelSO.cs
│                        # 銜接地圖節點選擇 → 戰鬥/商店/休息/事件產生
├── Enemy/
│   ├── Enemy.cs         # EnemySkill 抽象類別 + 技能預覽系統
│   ├── Slime.cs, PlagueDoctor.cs
│   ├── EnemyUI.cs, EnemyInfoObject.cs, ColliderHits.cs
├── Player/
│   ├── Player.cs        # 拖曳瞄準、反射預測線、地形交互、鏡頭跟隨
│   └── HealthUI.cs, MoneyUI.cs
├── Level/
│   ├── Obstacle.cs      # ITerrainZone / IBounceModifier 介面
│   ├── IceObstacle.cs, SlimeObstacle.cs, RubberObstacle.cs, BlackHoleObstacle.cs
│   ├── TerrainResolver.cs, LevelObstacleSO.cs
├── Map/
│   ├── MapGenerator.cs  # 權重隨機節點 + 分支連線演算法
│   └── MapNodeButton.cs
├── Event/
│   ├── EventObject.cs, EventManager.cs, EventUI.cs, ChoiceText.cs
│   ├── RestEvent.cs, ScoreBoardEvent.cs, ScoreBoard.cs
│   └── Shop/
│       ├── ShopEvent.cs, ShopController.cs, ShopItem.cs, Merchant.cs
├── Stats/
│   ├── Stats.cs         # 傷害/治療/金錢/效果通知中樞
│   ├── Effect.cs, EffectObject.cs, DamageType.cs
│   ├── RelicObject.cs, RelicManager.cs, RelicHolder.cs
├── Relic/
│   ├── PoisonBlade.cs, Catalyst.cs, StrengthDice.cs, MagicBook.cs, ProteinPowder.cs
├── Effect/
│   ├── Poison.cs, Strength.cs
├── Potion/
│   ├── PotionObject.cs, HealthPotion.cs, MaxHealthPotion.cs, StrengthPotion.cs
├── Summon/
│   ├── Summonable.cs, FireBall.cs
├── Localization/
│   ├── LocalizationManager.cs, English/En.json, Chinese/Ch.json
├── UI/
│   ├── UIManager.cs, ScreenBackdropClick.cs, PixelPerfectScrollRect.cs
│   ├── TurnStartEndUI.cs, EventTextUI.cs
│   ├── RelicUI.cs, RelicUIPrefab.cs, RelicChoose.cs, RelicChoosePanel.cs
│   ├── EnemyDescriptionBox.cs, EnemyDescriptionEffect.cs
│   └── DescriptionsListController.cs, DescriptionLinkHandler.cs, DescriptionBoxPrefab.cs
```

**貫穿全專案的設計原則：**
- **資料優先於程式碼（Data over code）**：敵人、遺物、效果、藥水、事件全部是 ScriptableObject，新增內容不需要改核心邏輯，只要在編輯器裡建立新的 asset 並掛上對應的 override 邏輯即可。
- **介面驅動的可擴充性（Interface over inheritance）**：`ITerrainZone`、`IBounceModifier`、`IOnHitEffect` 等一系列小介面讓地形/效果可以自由組合行為，不需要為每種組合寫子類別。
- **廣播式事件通知（Broadcast over polling）**：戰鬥中每個關鍵時機（命中、造成傷害、受到傷害、治療、回合開始/結束）都會廣播給 Relic 與 Effect 系統，UI 也是純訂閱端，不主動輪詢戰鬥狀態。

## 技術棧

- Unity（2D）
- C#
- Unity 2D Physics（`Rigidbody2D`、`CircleCast`、`OverlapCircleAll`）
- TextMeshPro
- ScriptableObject 資料驅動架構
- JSON 在地化（`JsonUtility`）

## 已知限制 / 之後可以做的

- 記分板小遊戲（`ScoreBoardEvent`）目前是獨立事件，尚未與主戰鬥系統的遺物觸發做更深的整合（例如命中記分板不會觸發 `OnHit` 系遺物）。
- 敵人技能目前是固定順序輪替（`currentSkillIndex` 循環），還沒有依血量/回合數做動態難度或 AI 決策。
- 目前沒有自動化測試，這個專案的重點是把物理彈射 + Roguelike 養成迴圈的系統廣度與互動細節做扎實，屬於個人系統練習專案。

---

*這個專案是我為了求職前深入理解「物理彈射戰鬥 + Roguelike 遺物養成」這類玩法的系統架構所做的個人練習專案。*
