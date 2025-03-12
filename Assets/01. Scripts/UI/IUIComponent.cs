using System.Collections.Generic;

public interface IUIComponent
{
    void Init();
    void Show();
    void Hide();
    List<IUIComponent> GetChildren();
}