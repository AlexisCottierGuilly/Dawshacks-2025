using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSwitcher : MonoBehaviour
{
    public string scene;

    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Debug.Log($"Switching to scene: {scene}");
        GameManager.instance.SwitchScene(scene);
    }
}
