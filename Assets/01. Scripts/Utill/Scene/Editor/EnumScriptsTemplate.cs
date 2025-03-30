using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class IndexRebuildController
{
    private const string SCRIPT_TEMPLATE =
        "using UnityEngine; \n" +
        "using System.Collections.Generic;\n" +
        "\n" +
        "public enum SceneId\n" +
        "{\n" +
        "        [Tooltip(\"Previous Scene in Build Index\")]\n" +
        "        PreviousScene = -2,\n" +
        "\n" +
        "        [Tooltip(\"Next Scene in Build Index\")]\n" +
        "        NextScene = -1,\n" +
        "\n" +
        "        [Tooltip(\"Invalid Scene/Unknown Scene/Null Scene\")]\n" +
        "        Unknown = 0,\n" +
        "\n" +
        "SCENE_ID_ENTRIES\n" +
        "}\n" +
        "\n" +
        "\n" +
        "public static class BI\n" +
        "{\n" +
        "    public static readonly (SceneId, string)[] BUILD_INDEX = new (SceneId, string)[]\n" +
        "    {\n" +
        "       BUILD_INDEX_ENTRIES\n" +
        "    };\n" +
        "\n" +
        "    public static readonly Dictionary<SceneId, string> ID_TO_NAME;\n" +
        "    public static readonly Dictionary<string, SceneId> NAME_TO_ID;\n" +
        "    public static readonly Dictionary<SceneId, int> ID_TO_INDEX;\n" +
        "\n" +
        "    static BI()\n" +
        "    {\n" +
        "        ID_TO_NAME = new Dictionary<SceneId, string>();\n" +
        "        ID_TO_INDEX = new Dictionary<SceneId, int>();\n" +
        "        NAME_TO_ID = new Dictionary<string, SceneId>();\n" +
        "\n" +
        "        int index = -3;\n" +
        "        foreach ((SceneId id, string name) in BUILD_INDEX)\n" +
        "        {\n" +
        "            ID_TO_NAME.Add(id, name);\n" +
        "            NAME_TO_ID.Add(name, id);\n" +
        "            ID_TO_INDEX.Add(id, index);\n" +
        "            index++;\n" +
        "        }\n" +
        "    }\n" +
        "}";
}
