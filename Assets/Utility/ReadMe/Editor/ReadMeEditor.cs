using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReadMe))]
[InitializeOnLoad]
public class ReadMeEditor : Editor
{
    // Private members
    [SerializeField] private GUIStyle m_linkStyle;

    [SerializeField] private GUIStyle m_titleStyle;

    [SerializeField] private GUIStyle m_headingStyle;

    [SerializeField] private GUIStyle m_bodyStyle;

    private bool m_initialized;

    // Internal (static-access) variables
    static string sessionState = "ReadmeEditor.showedReadme";

    static float kerning = 16f;

    // Publicly-accessible properties
    GUIStyle LinkStyle { get { return m_linkStyle; } }

    GUIStyle TitleStyle { get { return m_titleStyle; } }

    GUIStyle HeadingStyle { get { return m_headingStyle; } }

    GUIStyle BodyStyle { get { return m_bodyStyle; } }

    [MenuItem("Assets/Create/ReadMe")]
    internal static void CreateReadMe()
    {
        ReadMe readMe = CreateInstance<ReadMe>();

        AssetDatabase.CreateAsset(readMe, "Assets/ReadMe.asset");

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = readMe;
    }

    // Private methods
    private void init()
    {
        // Guard case
        if (m_initialized)
            return;

        m_bodyStyle = new GUIStyle(EditorStyles.label);
        m_bodyStyle.wordWrap = true;
        m_bodyStyle.fontSize = 14;

        m_titleStyle = new GUIStyle(m_bodyStyle);
        m_titleStyle.fontSize = 26;

        m_headingStyle = new GUIStyle(m_bodyStyle);
        m_headingStyle.fontSize = 18;
        m_headingStyle.fontStyle = FontStyle.Bold;

        m_linkStyle = new GUIStyle(m_bodyStyle);

        // Match selection color - works nicely in both light and dark mode
        m_linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
        m_linkStyle.stretchWidth = false;

        m_initialized = true;
    }

    private bool linkLabel(GUIContent label, params GUILayoutOption[] options)
    {
        var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

        Handles.BeginGUI();
        Handles.color = LinkStyle.normal.textColor;
        Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
        Handles.color = Color.white;
        Handles.EndGUI();

        EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

        return GUI.Button(position, label, LinkStyle);
    }

    // Editor namespace override(s)
    protected override void OnHeaderGUI()
    {
        var readMe = (ReadMe)target;

        init();

        var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, readMe.IconMaxWidth);

        GUILayout.BeginHorizontal("Title");
        {
            GUILayout.Label(readMe.Icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            GUILayout.Label(readMe.Title, TitleStyle);
        }

        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        var readMe = (ReadMe)target;

        init();

        foreach (var section in readMe.Sections)
        {
            if (!string.IsNullOrEmpty(section.Heading))
                GUILayout.Label(section.Heading, HeadingStyle);

            if (!string.IsNullOrEmpty(section.Text))
                GUILayout.Label(section.Text, BodyStyle);

            if (!string.IsNullOrEmpty(section.LinkText))
            {
                GUILayout.Space(kerning / 2);

                if (linkLabel(new GUIContent(section.LinkText)))
                    Application.OpenURL(section.URL);
            }

            GUILayout.Space(kerning);
        }
    }
}