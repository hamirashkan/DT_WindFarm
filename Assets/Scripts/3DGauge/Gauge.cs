﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using TMPro;

public class Gauge : MonoBehaviour {
    /// <summary>
    /// The number of segments in the graphic that may be visible
    /// </summary>
    private const int maxItemsVisible = 18;
    /// <summary>
    /// References to the individual segments so they can be activated/deactivated
    /// </summary>
    private MeshFilter[] segments;
    /// <summary>
    /// Reference to the label in the center of the gauge
    /// </summary>
    private TextMeshPro label = null;
    /// <summary>
    /// The format string to be used for formatting the label.  Allows for decimal places
    /// </summary>
    private string formatString = string.Empty;

    /// <summary>
    /// Internal storage for the current percentage displayed
    /// </summary>
    private float percent = 0.0f;
    /// <summary>
    /// Access method to get/set the percentage to be displayed
    /// </summary>
    public float Percent
    {
        get { return (percent); }
        set { percent = value; UpdateDisplay(); }
    }
    /// <summary>
    /// The text to appear before the number in the display
    /// </summary>
    public string labelPrefix = string.Empty;
    /// <summary>
    /// The text to appear after the number in the display
    /// </summary>
    public string labelSuffix = string.Empty;
    /// <summary>
    /// Internal storage for the number of decimal places to display
    /// </summary>
    private int decimalPlaces = 2;
    /// <summary>
    /// Accessor for changing the number of decimal places displayed by the text
    /// </summary>
    public int DecimalPlaces
    {
        get { return (decimalPlaces); }
        set { decimalPlaces = value; formatString = GetFormatString(decimalPlaces); }
    }

    /// <summary>
    /// Create the text format string
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places to include</param>
    /// <returns></returns>
    private string GetFormatString(int decimalPlaces)
    {
        StringBuilder decimalFormatter = new StringBuilder(4);
        decimalFormatter.Append("0");
        if (decimalPlaces > 0) decimalFormatter.Append(".");
        for (int i = 0; i < decimalPlaces; i++) decimalFormatter.Append("0");

        return string.Format("{{0}}{{1:{0}}}{{2}}", decimalFormatter.ToString());
    }
    /// <summary>
    /// Provide a method that can be messaged from the framework
    /// </summary>
    /// <param name="percent">The new percentage</param>
    public void SetPercentage(float newPercent)
    {
        // Minimize impact if we're being called with the same value
        if (percent != newPercent) Percent = newPercent;
    }

    /// <summary>
    /// Get our children object references so we don't have to refetch
    /// </summary>
    void Start()
    {
        // Get Child Object References (so we don't have to continually refresh)
        MeshFilter[] meshSegments = gameObject.GetComponentsInChildren<MeshFilter>();

        segments = meshSegments.OrderBy(x => x.name).ToArray<MeshFilter>();

        label = gameObject.GetComponentInChildren<TextMeshPro>();

        // Get the format string for the text
        formatString = GetFormatString(decimalPlaces);

        // Do initial display update
        UpdateDisplay();
    }

    /// <summary>
    /// Update display based on the percentage
    /// </summary>
    void UpdateDisplay()
    {
        // Calculate segments
        int itemsVisible = Mathf.FloorToInt(12.5f *(TurbineSetting.WindOutputValue / 100000)/ (100 / maxItemsVisible));
        // Update Segments
        for (int visible = 0; visible < maxItemsVisible; visible++)
        {
            MeshFilter child = segments[visible];
            child.gameObject.SetActive((visible > (maxItemsVisible - itemsVisible)));
        }

        // Update Text
        //label.text = string.Format(formatString, labelPrefix, Percent * 120, labelSuffix);
        var power = TurbineSetting.WindOutputValue / 100000;
        label.text = string.Format(formatString, labelPrefix, power, labelSuffix);
        label.color = new Color(16f * power / 100, 1 - 16f * power / 100, 1f, 1f);
    }

}
