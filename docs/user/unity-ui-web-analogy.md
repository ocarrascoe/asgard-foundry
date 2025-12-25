# Guía de Unity UI para Desarrolladores Web 🌐

Si vienes de HTML/CSS (Flexbox, Grid), Unity UI puede parecer extraño al principio, pero los conceptos tienen equivalencias directas.

## 1. El Concepto Clave: RectTransform 📦

En web usas el "Box Model" (margin, border, padding, content). En Unity, todo es un **RectTransform**.

| Concepto Web | Concepto Unity | Explicación |
|--------------|----------------|-------------|
| `position: absolute` | **RectTransform** | Por defecto, todo en Unity UI es absoluto relativo a su padre. |
| `top, left, right, bottom` | **Rect Transform Fields** | Controlan la posición. **OJO:** Cambian de significado según los Anchors. |
| `width / height` | **Width / Height** | Tamaño explícito (como `width: 250px`). |
| `transform-origin` | **Pivot** | El punto desde donde rota y escala el objeto (0,0 es abajo-izq, 1,1 es arriba-der). |
| `z-index` | **Hierarchy Order** | El orden en la lista Hierarchy define qué se dibuja encima (lo de abajo tapa a lo de arriba). |

---

## 2. Anchors (El "Responsive" de Unity) ⚓

Los **Anchors** son los triangulitos blancos que ves en la escena. Definen **a qué parte del padre se "pega"** el elemento.

Imagina que el Anchor son **chinchetas** que clavas en el padre.

### Caso A: Chinchetas juntas en el centro (Fixed Size)
- **Web:** `position: absolute; width: 200px; height: 100px; left: 50%; top: 50%; transform: translate(-50%, -50%);`
- **Unity:** Anchors Min(0.5, 0.5) Max(0.5, 0.5).
- **Resultado:** El objeto mantiene su tamaño en píxeles y se mueve con el centro.

### Caso B: Chinchetas en las esquinas de arriba (Top Stretch)
- **Web:** `position: absolute; top: 0; left: 0; right: 0; height: 100px;` (Width automático al 100%)
- **Unity:** Anchors Min(0, 1) Max(1, 1).
- **Resultado:** 
  - El campo `Width` desaparece y se convierte en `Left` y `Right` (márgenes).
  - El campo `Height` se mantiene fijo.
  - Si estiras la pantalla, el elemento se estira horizontalmente.

### Caso C: Chinchetas en las 4 esquinas (Full Stretch)
- **Web:** `position: absolute; top: 0; bottom: 0; left: 0; right: 0;`
- **Unity:** Anchors Min(0, 0) Max(1, 1).
- **Resultado:**
  - Los campos `Width` y `Height` desaparecen.
  - Se convierten en `Left`, `Right`, `Top`, `Bottom` (márgenes).
  - El objeto se estira con el padre totalmente.

---

## 3. Auto Layout (Flexbox) 🧱

Si no quieres colocar cosas a mano (absolute), usas **Layout Groups**.

| Concepto Web | Concepto Unity |
|--------------|----------------|
| `display: flex; flex-direction: row;` | **Horizontal Layout Group** |
| `display: flex; flex-direction: column;` | **Vertical Layout Group** |
| `display: grid;` | **Grid Layout Group** |
| `flex-grow: 1` | **Layout Element (Flexible Width/Height)** |
| `min-width / min-height` | **Layout Element (Min Width/Height)** |
| `padding` | **Padding** (dentro del Layout Group) |
| `gap` | **Spacing** |

**Nota Importante:** Cuando añades un Layout Group a un objeto, **bloquea el RectTransform de sus hijos**. Ya no puedes mover los hijos manualmente; el Layout Group los controla.

---

## 4. Ejemplo Práctico: Tu `HUD_Top`

Tienes esto configurado:
- **Anchors:** Min(0, 1), Max(1, 1) → "Top Stretch"
- **Pivot:** (0.5, 1) → "Top Center"

**Cómo interpretar los campos del Inspector:**

- **Left / Right:** Es el `margin-left` y `margin-right`. Si pones 0, ocupa todo el ancho.
- **Pos Y:** Es el offset vertical desde el ancla (arriba). Como el Pivot Y es 1 (arriba), `Pos Y = 0` significa "pegado al borde superior". Si pones -10, baja 10 píxeles.
- **Height:** Es `height: 250px`. Fijo.

**¿Cómo cambiar su tamaño?**
Simplemente cambia el valor **Height** en el Inspector. Como los Anchors están en modo "Stretch Horizontal" (0 a 1 en X), el ancho es automático, pero la altura es manual.

---

## Resumen para Web Devs

1. **Canvas Scaler** = Viewport meta tag (define la resolución base).
2. **Anchors** = Define comportamiento responsive (si es `%` o `px`).
3. **RectTransform** = `position: absolute` + `box-model`.
4. **Layout Groups** = `flexbox`.
5. **Content Size Fitter** = `width: auto` o `height: auto` (se ajusta al contenido).
