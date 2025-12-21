# Guía de Configuración Manual - Asgard Foundry

## Índice
1. [Configuración de la Escena](#1-configuración-de-la-escena)
2. [Creación del Canvas UI](#2-creación-del-canvas-ui)
3. [Configuración de Assets](#3-configuración-de-assets)
4. [Primera Prueba](#4-primera-prueba)

---

## 1. Configuración de la Escena

### Paso 1.1: Crear Escena Nueva
1. Archivo → New Scene → Basic (Built-in)
2. Guardar como `MainCity` en `Assets/_Project/Scenes/`

### Paso 1.2: Configurar Camera
1. Selecciona **Main Camera** en Hierarchy
2. En Inspector:
   - Position: `(0, 0, -10)`
   - Clear Flags: `Solid Color`
   - Background: Gris oscuro `RGB(51, 51, 51)`
   - Projection: `Orthographic`
   - Size: `10`

### Paso 1.3: Crear GameManager
1. Click derecho en Hierarchy → Create Empty
2. Nombre: `GameManager`
3. Arrastrarlo a **root** (no dentro de nada)
4. En Inspector → Add Component → busca `GameManager`
5. Debería mostrar el script `AsgardFoundry.Core.GameManager`

### Paso 1.4: Crear TimeManager
1. Click derecho en Hierarchy → Create Empty
2. Nombre: `TimeManager`
3. Arrastrarlo a **root** (al mismo nivel que GameManager)
4. Add Component → busca `TimeManager`

---

## 2. Creación del Canvas UI

### Paso 2.1: Crear Canvas Base
1. Click derecho en Hierarchy → UI → Canvas
2. Nombre: `Canvas`
3. En Inspector:
   - Canvas → Render Mode: `Screen Space - Overlay`
   - Canvas Scaler → UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1080 x 1920`

### Paso 2.2: Crear SafeArea
1. Click derecho en **Canvas** → Create Empty
2. Nombre: `SafeArea`
3. En Inspector:
   - Rect Transform:
     - Anchors: `Min (0, 0)`, `Max (1, 1)`
     - Left/Right/Top/Bottom: todos `0`
   - Add Component → `SafeAreaFitter`

### Paso 2.3: Crear HUD Superior
1. Click derecho en **SafeArea** → Create Empty
2. Nombre: `HUD_Top`
3. Rect Transform:
   - Anchors: `Min (0, 0.85)`, `Max (1, 1)`
   - Left: `10`, Right: `-10`, Top: `-10`, Bottom: `-10`
4. Add Component:
   - `Image` → Color: Negro semi-transparente `RGBA(0, 0, 0, 128)`
   - `Vertical Layout Group`:
     - Child Alignment: `Upper Center`
     - Control Child Size: Desmarcar `Width` (esto permite a los hijos definir su propio tamaño)
     - Child Force Expand: `Width` marcado
     - Spacing: `10`
     - Padding: `10` en todos los lados
   - `HUDPresenter`

#### 2.3.1: Añadir Texto de Villagers
1. Click derecho en **HUD_Top** → UI → Text - TextMeshPro
   - Si aparece ventana "TMP Importer", click **Import TMP Essentials**
2. Nombre: `VillagersText`
3. En componente TextMeshProUGUI:
   - Text: `Villagers: 0`
   - Font Size: `32`
   - Alignment: Center
   - Color: Blanco
4. **Arrastrar este Text** al campo `Total Villagers Text` en HUDPresenter.

#### 2.3.1.2: Añadir Texto de Era
1. Click derecho en **HUD_Top** → UI → Text - TextMeshPro
2. Nombre: `EraText`
3. Configurar:
   - Text: `Stone Age`
   - Font Size: `24`
   - Alignment: Center
   - Color: Dorado suave `RGB(255, 215, 128)`
4. **Arrastrar este Text** al campo `Era Text` en HUDPresenter.

#### 2.3.2: Crear Fila de Recursos
1. Click derecho en **HUD_Top** → Create Empty
2. Nombre: `ResourcesRow`
3. Add Component → `Horizontal Layout Group`:
   - Child Alignment: `Middle Center`
   - Spacing: `30`
4. Crear 3 textos dentro:
   - **StoneText**: Text: `Stone: 0`, Size: `28` → Arrastrar a campo `stoneText`
   - **WoodText**: Text: `Wood: 0`, Size: `28` → Arrastrar a campo `woodText`
   - **FoodText**: Text: `Food: 0`, Size: `28` → Arrastrar a campo `foodText`

#### 2.3.3: Crear Barra EITR
1. Click derecho en **HUD_Top** → UI → Image
2. Nombre: `EitrBarContainer`
3. Configurar:
   - Width: `600`, Height: `40`
   - Color: Gris oscuro `RGB(51, 51, 51)`
   - Add Component → `EitrBarPresenter`
   - En el Inspector, busca el componente **Eitr Bar Presenter** (debajo de Image):
     - Arrastra esta Image al campo `Background Image`

4. Click derecho en **EitrBarContainer** → UI → Image
5. Nombre: `Fill`
6. Configurar:
   - Anchors: `Stretch` (Min 0,0 Max 1,1)
   - Left/Right/Top/Bottom: todos `0`
    - **Source Image**: Haz click en el círculo pequeño a la derecha y selecciona `UISprite` (o cualquier cuadrado blanco). **Esto es necesario para ver las opciones siguientes.**
    - Color: Azul cian `RGB(51, 179, 255)`
    - **Image Type**: `Filled`
    - Fill Method: `Horizontal`
    - Arrastrar este objeto al campo `Fill Image` en el componente **Eitr Bar Presenter** (que está en el padre `EitrBarContainer`).

7. Click derecho en **EitrBarContainer** → UI → Text - TextMeshPro
8. Nombre: `EitrText`
9. Configurar:
   - Text: `100/100`
   - Font Size: `24`
   - Alignment: Center
   - Color: Blanco
   - Anchors: Stretch
   - Arrastrar al campo `Eitr Text` en EitrBarPresenter

### Paso 2.4: Crear HUD Inferior
1. Click derecho en **SafeArea** → Create Empty
2. Nombre: `HUD_Bottom`
3. Rect Transform:
   - Anchors: `Min (0, 0)`, `Max (1, 0.2)`
   - Left: `10`, Right: `-10`, Top: `10`, Bottom: `10`
4. Add Component:
   - `Image` → Color: Negro semi-transparente `RGBA(0, 0, 0, 128)`
   - `Vertical Layout Group`:
     - Child Alignment: `Middle Center`
     - Spacing: `15`
     - Padding: `10` en todos
   - `BottomHUDPresenter`

#### 2.4.1: Crear Tabs de Pre-Asignación
1. Click derecho en **HUD_Bottom** → Create Empty
2. Nombre: `PreAssignmentTabs`
3. Add Component:
   - `Layout Element` → Preferred Height: `60`
   - `Horizontal Layout Group`:
     - Spacing: `10`
     - Child Force Expand: Width y Height marcados
   - `Toggle Group`
   - Arrastrar el Toggle Group al campo `Tab Group` en BottomHUDPresenter

4. **Crear 5 Tabs** (repite 5 veces):
   - Click derecho en **PreAssignmentTabs** → UI → Toggle
   - Nombres: `Tab_Mine`, `Tab_Wood`, `Tab_Farm`, `Tab_Smith`, `Tab_Market`
   - Para cada uno:
     - **En el Inspector**: En el componente **Toggle** → Group: Arrastra el objeto **PreAssignmentTabs** (que tiene el Toggle Group).
     - **En la Hierarchy**: Haz clic en la flechita ▶ a la izquierda de `Tab_Mine` (y los otros) para desplegar sus hijos.
     - **Objeto Background**: Selecciónalo y cambia su Image → Color a Gris `RGB(77, 77, 77)`.
     - **Objeto Label**: Selecciónalo y cambia su Text a `MINE`, `WOOD`, etc.
     - **Label**: Font Size: `18`
   - Arrastra cada Toggle al campo correspondiente en BottomHUDPresenter:
     - Tab_Mine → `Mining Tab`
     - Tab_Wood → `Woodcutting Tab`
     - Tab_Farm → `Farming Tab`
     - Tab_Smith → `Smithing Tab`
     - Tab_Market → `Market Tab`

#### 2.4.2: Crear Botón de Generación (Flotante)
1. **Mover el botón**: En la Hierarchy, arrastra `GenerateButton` fuera de `HUD_Bottom` y suéltalo dentro de **SafeArea**. Ahora el botón es independiente del Layout Group.
2. **Posicionamiento Manual** (en Rect Transform):
   - **Anchors**: Min `(0.5, 0)`, Max `(0.5, 0)` (Bottom Center)
   - **Pivot**: `(0.5, 0)`
   - **Pos X**: `0`
   - **Pos Y**: `250` (Ajusta este número para que el botón "flote" a la altura que más te guste sobre la barra negra).
   - **Width**: `180`, **Height**: `180`
5. Si no ves un componente **Image** en el Inspector de `GenerateButton`, añádelo: `Add Component` → `Image`.
6. Configura la Image:
   - Color: Rojo `RGB(204, 51, 51)`
7. Add Component → `HoldButton`.
8. Selecciona el objeto **HUD_Bottom** en la Hierarchy para ver su `BottomHUDPresenter`.
9. **Conecta las referencias** (hay dos formas, usa la que te sea más cómoda):
   - **Forma A (Arrastrar)**: Con `HUD_Bottom` seleccionado, haz clic-y-mantén en `GenerateButton` en la Hierarchy y arrástralo sin soltar hasta los campos del Inspector. (Cuidado: si haces un clic rápido en el botón, el Inspector cambiará y perderás de vista el HUD_Bottom).
   - **Forma B (Selector de círculo)**: Haz clic en el **pequeño icono de círculo con un punto** que hay a la derecha del campo `Generate Button Image`. Se abrirá una ventana; asegúrate de elegir la pestaña **Scene** y selecciona ahí el `GenerateButton`.

---

## 3. Configuración de Assets

### Paso 3.1: Crear GameConfig
1. En Project → `Assets/_Project/ScriptableObjects/Configs`
2. Click derecho → Create → Asgard Foundry → Game Configuration
3. Nombre: `GameConfig`
4. Dejar valores por defecto (ya están bien configurados)

### Paso 3.2: Crear Production System Defs
1. En Project → `Assets/_Project/ScriptableObjects/Definitions`
2. Click derecho → Create → Asgard Foundry → Production System Definition
3. Crear **5 archivos**:
   - `Mining_Def`: Type: `Mining`, Output: `Stone`
   - `Woodcutting_Def`: Type: `Woodcutting`, Output: `Wood`
   - `Farming_Def`: Type: `Farming`, Output: `Food`
   - `Smithing_Def`: Type: `Smithing`, Output: `Metal`
   - `Market_Def`: Type: `Market`, Output: `Gold`

---

## 4. Primera Prueba

### Paso 4.1: Verificar Conexiones
1. Selecciona `HUD_Top` en Hierarchy
2. En HUDPresenter, verifica que todos los campos tengan referencias (no deben decir "None")
3. Repite para `HUD_Bottom` y `EitrBarContainer`

### Paso 4.2: Probar el Juego
1. Click **Play** ▶️
2. **Mantén presionado** el botón rojo "HOLD"
3. Deberías ver:
   - ✅ "Villagers: X" aumentando
   - ✅ Barra azul (EITR) regenerándose
   - ✅ Recursos (Stone, Wood, Food) acumulándose
   - ✅ Console mostrando mensajes `[GameManager]`

### Paso 4.3: Solución de Problemas

**Si el botón no hace nada:**
- Verifica que el HoldButton esté conectado en BottomHUDPresenter
- Asegúrate que el EventSystem existe en la escena (debería crearse automáticamente con el Canvas)

**Si los números no cambian:**
- Verifica que HUDPresenter tenga todas las referencias correctas
- Revisa Console por errores

**Si aparece "DontDestroyOnLoad warning":**
- Es normal, ignóralo (es un warning, no un error)

---

## Siguiente Paso: Añadir Visuals

Una vez que el juego funcione, puedes:
1. Crear sprites de edificios
2. Añadir animaciones
3. Implementar el System Panel (para ver detalles de cada sistema)
4. Añadir el Guidance Panel ("What You Can Do Now")

Consulta `status_visual.md` para ver qué features faltan y cómo priorizarlas.
