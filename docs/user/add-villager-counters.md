# Añadir Contadores de Villagers por Sistema

Esta guía explica cómo añadir los textos que muestran la cantidad de villagers asignados a cada sistema (Miners: X, Cutters: X, etc.)

## Resultado Final

```
┌─────────────────────────────────────────────┐
│  [MINE]  [WOOD]  [FARM]  [SMITH]  [MARKET]  │  ← Tabs (ya los tienes)
│ Miners:0 Cutters:0 Farmers:0 Smiths:0 Traders:0 │  ← NUEVO: Contadores
└─────────────────────────────────────────────┘
```

---

## Paso 1: Crear la Fila de Contadores

1. En la **Hierarchy**, selecciona `HUD_Bottom`
2. Click derecho → **Create Empty**
3. Nombre: `VillagerCountsRow`
4. En el Inspector:
   - **Layout Element** (Add Component):
     - Preferred Height: `30`
   - **Horizontal Layout Group** (Add Component):
     - Child Alignment: `Middle Center`
     - Spacing: `15`
     - Child Control Size → Width: ✓ marcado
     - Child Force Expand → Width: ✓ marcado

---

## Paso 2: Crear los 5 Textos

Repite esto **5 veces** (uno por sistema):

1. Click derecho en `VillagerCountsRow` → **UI → Text - TextMeshPro**
2. Configura cada uno:

| Nombre del Objeto | Texto Inicial | 
|-------------------|---------------|
| `MiningCountText` | `Miners: 0` |
| `WoodcuttingCountText` | `Cutters: 0` |
| `FarmingCountText` | `Farmers: 0` |
| `SmithingCountText` | `Smiths: 0` |
| `MarketCountText` | `Traders: 0` |

3. Para **cada texto**, configura en TextMeshProUGUI:
   - Font Size: `16`
   - Alignment: Center
   - Color: Blanco (`#FFFFFF`)
   - Font Style: Normal

---

## Paso 3: Conectar en BottomHUDPresenter

1. Selecciona `HUD_Bottom` en la Hierarchy
2. En el Inspector, busca el componente **Bottom HUD Presenter**
3. Verás una nueva sección: **"Villager Counts (per System)"**
4. Arrastra cada texto al campo correspondiente:

| Campo en Inspector | Objeto a arrastrar |
|--------------------|--------------------|
| Mining Count Text | `MiningCountText` |
| Woodcutting Count Text | `WoodcuttingCountText` |
| Farming Count Text | `FarmingCountText` |
| Smithing Count Text | `SmithingCountText` |
| Market Count Text | `MarketCountText` |

---

## Verificación

1. Click **Play** ▶️
2. Mantén presionado el botón rojo para generar villagers
3. Cambia entre los tabs (MINE, WOOD, etc.)
4. Observa cómo el contador correspondiente aumenta:
   - Si tienes MINE seleccionado → `Miners: 1, 2, 3...`
   - Si cambias a WOOD → `Cutters: 1, 2, 3...`

---

## Jerarquía Final

```
HUD_Bottom
├── PreAssignmentTabs
│   ├── Tab_Mine
│   ├── Tab_Wood
│   ├── Tab_Farm
│   ├── Tab_Smith
│   └── Tab_Market
├── VillagerCountsRow          ← NUEVO
│   ├── MiningCountText        ← NUEVO
│   ├── WoodcuttingCountText   ← NUEVO
│   ├── FarmingCountText       ← NUEVO
│   ├── SmithingCountText      ← NUEVO
│   └── MarketCountText        ← NUEVO
├── GenerateLabel
└── GenerateButton (en SafeArea)
```

---

## Solución de Problemas

**Los contadores no se actualizan:**
- Verifica que los textos estén conectados en BottomHUDPresenter
- Asegúrate que GameManager existe en la escena

**Los textos se ven demasiado grandes:**
- Reduce Font Size a `14` o `12`
- Ajusta el Preferred Height de VillagerCountsRow a `25`

**Los textos no caben horizontalmente:**
- Desmarca "Child Force Expand → Width" en el HorizontalLayoutGroup
- O reduce el Spacing a `5`
