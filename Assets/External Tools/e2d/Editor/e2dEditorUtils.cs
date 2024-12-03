/// @file
/// @autor Ondrej Mocny http://www.hardwire.cz
/// Veja LICENSE.txt para informações sobre a licença.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// Classe que gerencia todos os estilos usados no editor.
public class e2dEditorUtils
{

#region Inspector

    /// Índice do controle deslizante atualmente arrastado no elemento GUI de controle deslizante múltiplo vertical.
    private static int SliderDraggedIndex = -1;

    /// Deseleciona a ferramenta de cena atual.
    public static void DeselectSceneTools()
    {
        Tools.current = UnityEditor.Tool.None;
    }

    /// Retorna verdadeiro se alguma das ferramentas de cena padrão estiver selecionada.
    public static bool IsAnySceneToolSelected()
    {
        return Tools.current > UnityEditor.Tool.None;
    }

    /// Desenha campos para editar Vector2 na janela do editor atual.
    public static Vector2 Vector2Field(string label, Vector2 vector)
    {
        Vector2 v = vector;

        EditorGUILayout.BeginHorizontal();

        // rótulo
        if (label.Length > 0) EditorGUILayout.PrefixLabel(label);

        // área dos campos
        Rect drawArea = EditorGUILayout.BeginHorizontal(e2dStyles.RectArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        Rect area = drawArea;

        float width = (drawArea.width - 1 * e2dConstants.VECTOR_FIELD_PADDING) / 2;
        float labelWidth = 0;

        // garante que há altura suficiente na área horizontal
        GUILayoutUtility.GetRect(1, GUI.skin.label.CalcHeight(new GUIContent("A"), 1));

        // desenha campos
        area.width = width;
        GUI.Label(area, e2dStrings.LABEL_VECTOR2_X);
        labelWidth = e2dConstants.VECTOR_FIELD_LABEL_MARGIN + GUI.skin.label.CalcSize(new GUIContent(e2dStrings.LABEL_VECTOR2_X)).x;
        area.xMin += labelWidth;
        v.x = EditorGUI.FloatField(area, v.x, e2dStyles.RectField);
        area.xMin += e2dConstants.VECTOR_FIELD_PADDING + width - labelWidth;

        area.width = width;
        GUI.Label(area, e2dStrings.LABEL_VECTOR2_Y);
        labelWidth = e2dConstants.VECTOR_FIELD_LABEL_MARGIN + GUI.skin.label.CalcSize(new GUIContent(e2dStrings.LABEL_VECTOR2_Y)).x;
        area.xMin += labelWidth;
        v.y = EditorGUI.FloatField(area, v.y, e2dStyles.RectField);
        area.xMin += e2dConstants.VECTOR_FIELD_PADDING + width - labelWidth;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        return v;
    }

    /// Desenha campos para editar um retângulo na janela do editor atual.
    public static Rect RectField(string label, Rect rectangle)
    {
        Rect rect = rectangle;

        EditorGUILayout.BeginHorizontal();

        // rótulo
        if (label.Length > 0) EditorGUILayout.PrefixLabel(label);

        // área dos campos
        Rect drawArea = EditorGUILayout.BeginHorizontal(e2dStyles.RectArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        Rect area = drawArea;

        float width = (drawArea.width - 3 * e2dConstants.RECT_FIELD_PADDING) / 4;
        float labelWidth = 0;

        // garante que há altura suficiente na área horizontal
        GUILayoutUtility.GetRect(1, GUI.skin.label.CalcHeight(new GUIContent("A"), 1));

        // desenha campos
        area.width = width;
        GUI.Label(area, e2dStrings.LABEL_RECT_XMIN);
        labelWidth = e2dConstants.RECT_FIELD_LABEL_MARGIN + GUI.skin.label.CalcSize(new GUIContent(e2dStrings.LABEL_RECT_XMIN)).x;
        area.xMin += labelWidth;
        rect.xMin = EditorGUI.FloatField(area, rect.xMin, e2dStyles.RectField);
        area.xMin += e2dConstants.RECT_FIELD_PADDING + width - labelWidth;

        area.width = width;
        GUI.Label(area, e2dStrings.LABEL_RECT_XMAX);
        labelWidth = e2dConstants.RECT_FIELD_LABEL_MARGIN + GUI.skin.label.CalcSize(new GUIContent(e2dStrings.LABEL_RECT_XMAX)).x;
        area.xMin += labelWidth;
        rect.xMax = EditorGUI.FloatField(area, rect.xMax, e2dStyles.RectField);
        area.xMin += e2dConstants.RECT_FIELD_PADDING + width - labelWidth;

        area.width = width;
        GUI.Label(area, e2dStrings.LABEL_RECT_YMIN);
        labelWidth = e2dConstants.RECT_FIELD_LABEL_MARGIN + GUI.skin.label.CalcSize(new GUIContent(e2dStrings.LABEL_RECT_YMIN)).x;
        area.xMin += labelWidth;
        rect.yMin = EditorGUI.FloatField(area, rect.yMin, e2dStyles.RectField);
        area.xMin += e2dConstants.RECT_FIELD_PADDING + width - labelWidth;

        area.width = width;
        GUI.Label(area, e2dStrings.LABEL_RECT_YMAX);
        labelWidth = e2dConstants.RECT_FIELD_LABEL_MARGIN + GUI.skin.label.CalcSize(new GUIContent(e2dStrings.LABEL_RECT_YMAX)).x;
        area.xMin += labelWidth;
        rect.yMax = EditorGUI.FloatField(area, rect.yMax, e2dStyles.RectField);
        area.xMin += e2dConstants.RECT_FIELD_PADDING + width - labelWidth;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        return rect;
    }

    /// Desenha um controle deslizante múltiplo vertical na janela do editor atual. Pré-visualizações de imagem são usadas para descrever os controles.
    public static void VerticalMultiSlider(string label, ref List<float> values, List<Texture> images, float minValue, float maxValue, float threshold, string thresholdLabel, float size, bool upsideDown)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);

        if (images != null && images.Count != values.Count)
        {
            e2dUtils.Error("VerticalMultiSlider: values and images have different number of elements");
        }

        Event currentEvent = new Event(Event.current);

        // inicializa a área em que vamos desenhar
        Rect area = GUILayoutUtility.GetRect(0, float.MaxValue, size, size, GUI.skin.button);
        const float THUMB_Y_OFFSET = 5;
        float imageSize = size / values.Count / 2;
        area.yMin += 0.5f * imageSize;
        area.yMax -= 0.5f * imageSize;

        // inicializa os estilos que usaremos
        GUIStyle sliderStyle = new GUIStyle("MiniMinMaxSliderVertical");
        sliderStyle.fixedWidth *= 1.5f;
        GUIStyle thumbStyle = new GUIStyle("MinMaxHorizontalSliderThumb");

        // desenha o retângulo de fundo do controle deslizante
        Rect sliderRect = area;
        sliderRect.xMin += 0.5f * area.width - 0.5f * sliderStyle.fixedWidth;
        sliderRect.xMax -= 0.5f * area.width - 0.5f * sliderStyle.fixedWidth;
        GUI.Box(sliderRect, GUIContent.none, sliderStyle);

        // nível de limiar
        float thresholdValue = Mathf.Clamp01((threshold - minValue) / (maxValue - minValue));
        if (upsideDown) thresholdValue = 1 - thresholdValue;
        float thresholdY = area.y + THUMB_Y_OFFSET + thresholdValue * (area.height - 2 * THUMB_Y_OFFSET);
        GUI.Box(new Rect(area.x, thresholdY, area.width, 1), GUIContent.none);

        // rótulo de limiar
        Vector2 thresholdLabelSize = e2dStyles.MiniLabel.CalcSize(new GUIContent(thresholdLabel));
        Rect thresholdLabelRect = new Rect(area.x - thresholdLabelSize.x - 2, thresholdY - 0.5f * thresholdLabelSize.y - 2, thresholdLabelSize.x, thresholdLabelSize.y);
        GUI.Label(thresholdLabelRect, thresholdLabel, e2dStyles.MiniLabel);

        // desenha os controles
        for (int i = 0; i < values.Count; i++)
        {
            if (float.IsNaN(values[i])) values[i] = 0;

            // o controle
            float xPos = area.x + 0.5f * area.width;
            float value = Mathf.Clamp01((values[i] - minValue) / (maxValue - minValue));
            if (upsideDown) value = 1 - value;
            float yPos = area.y + THUMB_Y_OFFSET + value * (area.height - 2 * THUMB_Y_OFFSET);
            Rect thumbRect = new Rect(xPos - 0.5f * sliderStyle.fixedWidth, yPos - 0.5f * thumbStyle.fixedHeight - 3, sliderStyle.fixedWidth, thumbStyle.fixedHeight);
            GUI.Button(thumbRect, GUIContent.none, thumbStyle);

            // pré-visualização da imagem
            if (images != null && images[i] != null)
            {
                float imageX = xPos - 0.5f * thumbRect.width - imageSize;
                float imageY = yPos - 0.5f * imageSize;
                Rect imageRect = new Rect(imageX, imageY, imageSize, imageSize);
                GUI.DrawTexture(imageRect, images[i]);
            }

            // valores numéricos
            string numberString = "" + values[i];
            Vector2 numberSize = e2dStyles.MiniLabel.CalcSize(new GUIContent(numberString));
            float numberX = xPos + 0.5f * thumbRect.width;
            float numberY = yPos - 0.5f * numberSize.y - 1;
            GUI.Label(new Rect(numberX, numberY, numberSize.x, numberSize.y), numberString, e2dStyles.MiniLabel);

            // controle do mouse
            if (currentEvent.type == EventType.MouseDown && thumbRect.Contains(currentEvent.mousePosition))
            {
                SliderDraggedIndex = i;
            }
        }

        // arrastando um controle com o mouse
        if (SliderDraggedIndex != -1 && currentEvent.type == EventType.MouseDrag)
        {
            float value = (currentEvent.mousePosition.y - area.y - THUMB_Y_OFFSET) / (area.height - 2 * THUMB_Y_OFFSET);
            value = Mathf.Clamp01(value);
            if (upsideDown) value = 1 - value;
            values[SliderDraggedIndex] = value * (maxValue - minValue) + minValue;
        }

        // solta o controle ao soltar o mouse
        if (currentEvent.type == EventType.MouseUp)
        {
            SliderDraggedIndex = -1;
        }

        EditorGUILayout.EndHorizontal();
    }

#endregion


#region Scene

    /// Transform usado para todas as funções de desenho de cena e controles.
    public static Transform transform;

    /// Retorna o tamanho de um controle em um ponto dado no espaço da transformação atual. A câmera atual é considerada.
    public static float GetHandleSize(Vector2 position)
    {
        return HandleUtility.GetHandleSize(transform.TransformPoint(position));
    }

    /// Desenha uma linha 2D.
    public static void DrawLine(Vector2 a, Vector2 b)
    {
        Vector3 a3d = transform.TransformPoint(a);
        Vector3 b3d = transform.TransformPoint(b);
        Handles.DrawLine(a3d, b3d);
    }

    /// Desenha uma linha 2D.
    public static void DrawLine(Vector2 a, Vector2 b, float width)
    {
        Vector2 direction = (a - b);
        Vector2 normal = new Vector2(-direction.y, direction.x);
        normal.Normalize();
        Vector3 a3d = transform.TransformPoint(a);
        Vector3 b3d = transform.TransformPoint(b);
        Vector3 normal3d = transform.TransformDirection(normal);

        // Nota: DrawAAPolyLine está com bug na versão 3.3.0f4
        // A parte Z do segundo ponto é ignorada.
        //Handles.DrawAAPolyLine(b3d, a3d);

        Vector3[] poly = new Vector3[] { a3d - width * normal3d, a3d + width * normal3d, b3d + width * normal3d, b3d - width * normal3d };
        Handles.DrawSolidRectangleWithOutline(poly, Handles.color, Handles.color);
    }

    /// Desenha uma esfera 2D (círculo sombreado).
    public static void DrawSphere(Vector2 center, float size)
    {
        Handles.SphereHandleCap(0, center, Quaternion.identity, size, EventType.Repaint);
    }

    /// Desenha uma seta 2D.
    public static void DrawArrow(Vector2 origin, float angle, float size)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Handles.ArrowHandleCap(0, origin, rot, size, EventType.Repaint);
    }

    /// Desenha um rótulo de texto.
    public static void DrawLabel(Vector2 position, string label, GUIStyle style)
    {
        Handles.Label(position, label, style);
    }

    /// Desenha um ponto 2D.
    public static void DrawDot(Vector2 position, float size)
    {
        Handles.DotHandleCap(0, position, Quaternion.identity, size, EventType.Repaint);
    }

    /// Desenha um controle 2D para manipular vetores 2D.
    public static Vector2 PositionHandle2d(Vector2 position)
    {
        return PositionHandle2dScaled(position, GetHandleSize(position), true);
    }

    /// Desenha um controle 2D para manipular vetores 2D.
    public static Vector2 PositionHandle2d(Vector2 position, bool showSliders)
    {
        return PositionHandle2dScaled(position, GetHandleSize(position), showSliders);
    }

    /// Desenha um controle 2D para manipular vetores 2D. A escala deve levar em conta a distância da câmera ao objeto.
    /// Para torná-lo independente da distância da câmera, chame PositionHandle2d() ou use HandleUtility.GetHandleSize().
    public static Vector2 PositionHandle2dScaled(Vector2 position, float scale, bool showSliders)
    {
        Vector3 position3d = transform.TransformPoint(position);

        Handles.color = e2dConstants.COLOR_HANDLE_CENTER;
        position3d = Handles.FreeMoveHandle(position3d, e2dConstants.SCALE_HANDLE_CENTER * scale, Vector3.zero, Handles.RectangleHandleCap);
        Handles.DotHandleCap(0, position3d, Quaternion.identity, e2dConstants.SCALE_HANDLE_CENTER_DOT * scale, EventType.Repaint);
        if (showSliders)
        {
            Handles.color = e2dConstants.COLOR_HANDLE_X_SLIDER;
            position3d = Handles.Slider(position3d, transform.TransformDirection(Vector3.right), e2dConstants.SCALE_HANDLE_SLIDER * scale, Handles.ArrowHandleCap, 0);
            Handles.color = e2dConstants.COLOR_HANDLE_Y_SLIDER;
            position3d = Handles.Slider(position3d, transform.TransformDirection(Vector3.up), e2dConstants.SCALE_HANDLE_SLIDER * scale, Handles.ArrowHandleCap, 0);
        }

        return transform.InverseTransformPoint(position3d);
    }

    /// Desenha um controle 2D em forma de cubo para manipular vetores 2D.
    public static Vector2 PositionHandle2dCube(Vector2 position, Color color, float scale)
    {
        Vector3 position3d = transform.TransformPoint(position);

        Handles.color = color;
        var fmh_326_57_638680413358957196 = Quaternion.identity; position3d = Handles.FreeMoveHandle(position3d, scale * GetHandleSize(position), Vector3.zero, Handles.CubeHandleCap);

        return transform.InverseTransformPoint(position3d);
    }

    /// Desenha um controle 2D para manipular um ângulo.
    public static float RotationHandle2d(float angle, Vector2 position)
    {
        Handles.color = e2dConstants.COLOR_HANDLE_CENTER;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 position3d = transform.TransformPoint(position);
        Vector3 direction3d = transform.TransformDirection(Vector3.forward);
        rotation = Handles.Disc(rotation, position3d, direction3d, e2dConstants.SCALE_HANDLE_ROTATION * GetHandleSize(position), false, 0);
        return rotation.eulerAngles.z;
    }

#endregion


#region Scene GUI

    /// Desenha um rótulo de erro no meio da tela.
    public static void DrawErrorLabel(string message)
    {
        Handles.BeginGUI();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(message, e2dStyles.SceneError);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        Handles.EndGUI();
    }

#endregion

}
