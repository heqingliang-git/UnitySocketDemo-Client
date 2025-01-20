using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoContextEditor : Editor
{
    [MenuItem("CONTEXT/MonoBehaviour/自动赋值")]
    private static void AssignVariablesByName(MenuCommand command)
    {
        MonoBehaviour targetMono = (MonoBehaviour)command.context;

        FieldInfo[] fields = targetMono.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (FieldInfo field in fields)
        {
            // 获取字段的名称和类型
            string fieldName = field.Name;
            System.Type fieldType = field.FieldType;
            object fieldValue = field.GetValue(targetMono);

            if (fieldValue == null)
            {
                // 在子物体中查找与字段名称相同的组件
                Component component = targetMono.transform.Find(FirstWordToUpper(fieldName))?.GetComponent(fieldType);

                if (component != null)
                {
                    // 将找到的组件赋值给字段
                    field.SetValue(targetMono, component);
                    Debug.Log($"{component} 赋值给 {fieldName}");
                }
                else
                {
                    Debug.LogWarning($"{fieldName} 未找到符合组件");
                }
            }

        }
    }

    [MenuItem("CONTEXT/MonoBehaviour/自动重命名")]
    private static void AutoRenameObjects(MenuCommand command)
    {
        MonoBehaviour targetMono = (MonoBehaviour)command.context;

        FieldInfo[] fields = targetMono.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(targetMono);
            if (value == null) continue;

            if (value is Component component)
            {
                component.gameObject.name = FirstWordToUpper(field.Name);
            }
            else
            {
                Debug.LogWarning($"{field.FieldType} 不支持重命名");
            }
        }
    }

    private static string FirstWordToUpper(string input)
    {
        return char.ToUpper(input.First()) + input.Substring(1);
    }
}
