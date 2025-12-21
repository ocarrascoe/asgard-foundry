# Asgard Foundry

[![Unity](https://img.shields.io/badge/Unity-2022.3+-black?logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-All%20Rights%20Reserved-red)](./LICENSE)

**Asgard Foundry** is a Norse mythology-themed idle/incremental game for mobile platforms. Build your Viking settlement, generate villagers, and develop your civilization through the ages.

## 🎮 Game Concept

Inspired by games like **Egg Inc.** and **Adventure Capitalist**, Asgard Foundry combines:

- **Idle mechanics** - Resources accumulate while you're away
- **Villager generation** - Tap/hold to create workers using EITR (divine energy)
- **Production systems** - Assign villagers to Mining, Woodcutting, Farming, Smithing, and Market
- **City progression** - Upgrade buildings and advance through eras (Stone Age → Bronze Age → Iron Age → Viking Age)

## 🏗️ Architecture

```
Assets/_Project/
├── Scripts/
│   ├── Core/           # GameManager, TimeManager, EventBus
│   ├── Data/           # GameState, EitrState, ProductionSystemState
│   ├── Persistence/    # SaveManager (JSON)
│   ├── Systems/        # ProductionSystemDef (ScriptableObject)
│   └── UI/
│       ├── Components/ # SafeAreaFitter, HoldButton
│       └── Presenters/ # HUDPresenter, EitrBarPresenter, BottomHUDPresenter
├── ScriptableObjects/
└── Scenes/
```

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- TextMeshPro (imported via Package Manager)

### Setup
1. Clone the repository
2. Open in Unity
3. Open the `MainCity` scene (or create following `docs/user/manual-setup-guide.md`)
4. Press Play

## 📖 Documentation

| Document | Description |
|----------|-------------|
| [Manual Setup Guide](docs/user/manual-setup-guide.md) | Step-by-step Unity UI configuration |
| [Architecture Reference](docs/user/architecture-reference.md) | Component roles and data flow |
| [Add Villager Counters](docs/user/add-villager-counters.md) | Adding per-system villager counts |

## 🎯 Core Features

- [x] EITR energy system with overheat mechanic
- [x] Villager generation with hold-to-spawn button
- [x] 5 production systems (Mining, Woodcutting, Farming, Smithing, Market)
- [x] Pre-assignment tabs for directing new villagers
- [x] Offline progress calculation
- [x] Auto-save system (every 30s + on pause/quit)
- [ ] Automation managers
- [ ] Building upgrades
- [ ] Era progression
- [ ] Visual villager representation
- [ ] Sound effects and music

## 📜 License

All Rights Reserved © 2024 Komarov

This project is proprietary software. See [LICENSE](./LICENSE) for details.
