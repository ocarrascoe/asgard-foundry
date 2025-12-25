# Mejoras Visuales y Settings 🎨⚙️

Esta guía cubre tres grandes mejoras:
1. **Workers Infinitos**: Ya no hay límite de 10.
2. **Menú de Settings**: Botón de engranaje para reiniciar.
3. **Iconos de Recursos**: Usar las imágenes en vez de solo texto.

---

## 1. Workers Infinitos 👩‍🌾

✅ **¡Hecho!** He modificado el código (`ProductionSystemState.cs`) internamente.
- Ahora `CanAcceptVillager` siempre es `true`.
- `MaxVillagers` está configurado en "infinito" (`int.MaxValue`).
- No necesitas hacer nada, ya funciona.

---

## 2. Menú de Settings ⚙️

### A. Crear el Panel de Settings
1. En **Canvas** → Click derecho → **UI → Panel**.
2. Nombre: `SettingsPanel`.
3. Color: Negro casi sólido (`RGBA 0,0,0, 240`).
4. Dentro de SettingsPanel, crea:
   - **Text (TMP)**: "PAUSED" o "SETTINGS" (título grande).
   - **Button (TMP)**: Nombre `ResetButton`. Texto: "RESET GAME". Color: Rojo.
   - **Button (TMP)**: Nombre `CloseButton`. Texto: "CONTINUE". Color: Verde.
5. **¡Importante!** Desactiva el objeto `SettingsPanel` (desmarca la casilla junto al nombre en el Inspector) para que empiece oculto.

### B. Crear el Botón de Engranaje (Gear)
1. En **SafeArea** (fuera de HUDs) → Click derecho → **UI → Button (TMP)**.
2. Nombre: `SettingsButton`.
3. Posición (Top Right):
   - Anchors: Min(1, 1), Max(1, 1).
   - Pivot: (1, 1).
   - Pos X: `-20`, Pos Y: `-20`.
   - Size: `60x60`.
4. Borra el componente Text hijo (si quieres usar icono) y ponle un icono de engranaje al Image, o ponle texto "⚙️".

### C. Conectar la Lógica
1. Crea un objeto vacío en **Canvas** llamado `SettingsManager`.
2. Add Component → **`SettingsPresenter`** (el script nuevo que creé).
3. Arrastra las referencias:
   - **Settings Panel**: Arrastra tu `SettingsPanel`.
   - **Open Settings Button**: Arrastra tu `SettingsButton`.
   - **Close Button**: Arrastra el `CloseButton` (dentro del panel).
   - **Reset Game Button**: Arrastra el `ResetButton` (dentro del panel).

---

## 3. Iconos de Recursos 💎

Tienes iconos en `Assets/_Project/Art/UI/`. Vamos a usar `Icon_Mining.png` (piedra), `Icon_Woodcutting.png` (madera), etc.

### Paso a Paso para cada recurso:

Vamos a cambiar la "ResourcesRow" actual.

1. Ve a `HUD_Top/ResourcesRow`.
2. Borra (o desactiva) los textos viejos (`StoneText`, `WoodText`, etc.).
3. Para la PIEDRA (Stone):
   - Crea un **Empty** llamado `Resource_Stone`.
   - Ponle un **Horizontal Layout Group** (Child Control Size: Width/Height marcados).
   - Dentro, crea una **UI → Image**:
     - Nombre: `Icon`.
     - Source Image: Arrastra `Icon_Mining` desde Project.
     - Layout Element: Preferred Width/Height `40`.
   - Dentro, crea un **UI → Text (TMP)**:
     - Nombre: `ValueText`.
     - Texto: `0`.
     - Alignment: Left Middle.
   
   **Repite para Wood y Food**.

### Actualizar el HUDPresenter
Ahora que has cambiado la estructura, el script `HUDPresenter` habrá perdido las referencias a los textos.

1. Selecciona `HUD_Top`.
2. En `HUD Presenter`:
   - Arrastra tus NUEVOS objetos `ValueText` a los campos `Stone Text`, `Wood Text`, etc.
   - **¡OJO!** Como ahora el icono está separado, en el script `HUDPresenter.cs` deberíamos quitar el prefijo "Stone: " para que solo muestre el número, ya que el icono visualmente ya dice qué es.

   *(Si quieres que yo quite los prefijos del código, dímelo y lo hago en un segundo)*.
