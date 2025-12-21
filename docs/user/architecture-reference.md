# Asgard Foundry - Arquitectura del Proyecto

## Estructura de Carpetas

```
Assets/_Project/
├── Scripts/
│   ├── Core/              → GameManager, TimeManager, EventBus
│   ├── Data/              → GameState, EitrState, ProductionSystemState
│   ├── Persistence/       → SaveManager (JSON)
│   ├── Systems/           → ProductionSystemDef (ScriptableObject)
│   ├── Automation/        → AutomationDef, AutomationManager
│   ├── ScriptableObjects/ → GameConfig
│   └── UI/
│       ├── Components/    → SafeAreaFitter, HoldButton
│       └── Presenters/    → HUDPresenter, EitrBarPresenter, etc.
├── ScriptableObjects/
│   ├── Configs/           → GameConfig.asset
│   └── Definitions/       → ProductionSystemDef assets
├── Scenes/
├── Prefabs/
├── Art/
└── Audio/
```

## Componentes Principales

### GameManager (Singleton)
**Ubicación**: Root de la escena  
**Función**: Orquestador central del juego
- Inicializa el GameState
- Calcula progreso offline
- Actualiza sistemas de producción cada frame
- Maneja generación de villagers
- Auto-guarda cada 30s

### TimeManager (Singleton)
**Ubicación**: Root de la escena  
**Función**: Gestión del tiempo del juego
- Provee delta time
- Maneja estados de pausa
- Utilidades de timestamps

### EventBus (Static)
**Función**: Comunicación desacoplada entre sistemas
- `OnVillagerGenerated`
- `OnResourceChanged`
- `OnEitrChanged`
- `OnGameStateLoaded`

## UI Presenters - Qué Hacen

### HUDPresenter
**Conectado a**: HUD_Top  
**Referencias necesarias**:
- `stoneText` → TextMeshProUGUI
- `woodText` → TextMeshProUGUI
- `foodText` → TextMeshProUGUI
- `totalVillagersText` → TextMeshProUGUI
- `eraText` (opcional) → TextMeshProUGUI

**Función**: Actualiza los números del HUD cada frame leyendo de `GameManager.Instance.State`

### EitrBarPresenter
**Conectado a**: EitrBarContainer  
**Referencias necesarias**:
- `backgroundImage` → Image (barra gris)
- `fillImage` → Image (barra azul, tipo Filled)
- `eitrText` → TextMeshProUGUI
- `overheatOverlay` (opcional) → Image

**Función**: 
- Actualiza fillAmount de la barra según EITR actual
- Cambia color a rojo cuando está en overheat
- Muestra countdown de cooldown

### BottomHUDPresenter
**Conectado a**: HUD_Bottom  
**Referencias necesarias**:
- `tabGroup` → ToggleGroup
- `miningTab` → Toggle
- `woodcuttingTab` → Toggle
- `farmingTab` → Toggle
- `smithingTab` → Toggle
- `marketTab` → Toggle
- `generateButton` → HoldButton
- `generateButtonImage` → Image

**Función**:
- Cambia el sistema de asignación cuando cambias de tab
- Llama a `GameManager.TryGenerateVillager()` cuando presionas el botón
- Cambia color del botón según estado de overheat

## Ciclo de Gameplay

```
1. Usuario mantiene presionado botón rojo
   ↓
2. HoldButton.OnTrigger se dispara repetidamente
   ↓
3. BottomHUDPresenter.OnGenerateTriggered()
   ↓
4. GameManager.TryGenerateVillager()
   ↓
5. ¿Hay suficiente EITR?
   SÍ → Genera villager, resta EITR, dispara evento
   NO → Entra en overheat si EITR está al máximo
   ↓
6. EventBus.OnVillagerGenerated()
   ↓
7. Villager se asigna al sistema seleccionado (tab activo)
   ↓
8. Sistema de producción genera recursos automáticamente
   ↓
9. HUDPresenter actualiza números en pantalla
```

## Data Flow

```
GameState (datos persistentes)
    ↓
GameManager (lógica central)
    ↓
EventBus (notificaciones)
    ↓
UI Presenters (visualización)
```

## Shortcuts de Unity Útiles

- **F**: Focus en objeto seleccionado en Scene
- **Ctrl+D**: Duplicar
- **Ctrl+Shift+N**: Crear GameObject vacío
- **Scene/Game toggle**: Espacio
- **Play/Pause**: Ctrl+P / Ctrl+Shift+P

## Debugging

**Console Logs Importantes**:
- `[GameManager] Game initialized` → Startup correcto
- `[GameManager] Processing Xs of offline progress` → Cálculo offline
- `[SaveManager] Game saved to: path` → Guardado exitoso

**Errores Comunes**:
- `NullReferenceException` en Presenter → Falta arrastrar referencia en Inspector
- `The variable X of Y has not been assigned` → Campo vacío en Inspector
- EITR no regenera → GameManager no está en la escena activa
