using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FlexibleGridLayout : LayoutGroup {
    public enum FitType {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public FitType fitType;

    public bool centreLastRow;
    public int rows;    
    public int columns; 
    public Vector2 cellSize;
    public Vector2 spacing;

    public bool fitX;
    public bool fitY;
    public int  activateChildren;

    public override void CalculateLayoutInputVertical() {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform) {
            fitX = fitY = true;
            activateChildren = 0;

            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild(i).gameObject.activeInHierarchy) activateChildren++;
            }

            if ( activateChildren == 0 ) return; //=> break out HERE if there are no Children on this GameObject to align

            float acRT = Mathf.Sqrt(activateChildren);
            rows = columns = Mathf.CeilToInt(acRT);
        }

        if ( fitType == FitType.Width || fitType == FitType.FixedColumns )
            rows = Mathf.CeilToInt( activateChildren / (float) columns );
        
        if ( fitType == FitType.Height || fitType == FitType.FixedRows )
            columns = Mathf.CeilToInt( activateChildren / (float) rows );

        float parentWidth  = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth  = ( parentWidth - spacing.x * ( columns - 1 ) - padding.left - padding.right ) / (float) columns;
        float cellHeight = ( parentHeight- spacing.y * ( rows    - 1 ) - padding.top  - padding.bottom) / (float) rows;

        cellSize.x = fitX ? cellWidth  : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount, rowCount;
        columnCount = rowCount = 0;

        //* horizontal Adjustment Factors. see for better understanding: https://www.desmos.com/calculator/yue2ylvf4a
        int  lastRowAmount = activateChildren % columns;
        int? lastRowTrim   = null;

        if ( lastRowAmount != 0 && centreLastRow )
            lastRowTrim = columns - lastRowAmount;

        for ( int i = 0; i < activateChildren; i++ ) {
            rowCount    = i / columns;
            columnCount = i % columns;
            
            RectTransform item = rectChildren[i];
            
            float xPos = (cellSize.x + spacing.x) * columnCount + padding.left;
            float yPos = (cellSize.y + spacing.y) * rowCount    + padding.top;

            //* adjust horizntal aligment depeding on the spaces left
            if ( !(lastRowTrim is null) ) {
                if ( activateChildren - i <= lastRowAmount )
                    xPos += (float) (.5f * (cellSize.x + spacing.x) * lastRowTrim);
            }

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }

    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }
}