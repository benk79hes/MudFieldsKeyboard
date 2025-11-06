using Microsoft.JSInterop;

namespace MudFieldsKeyboard.Components;

public class KeyboardService
{
    private static readonly KeyboardService _instance = new();
    
    public static KeyboardService Instance => _instance;
    
    private object? _activeField;
    private KeyboardLayout _currentLayout = KeyboardLayout.Text;
    private bool _allowDecimal = false;
    private bool _isVisible = false;
    private IJSRuntime? _jsRuntime;

    public event Action? OnStateChanged;

    public bool IsVisible => _isVisible;
    public KeyboardLayout CurrentLayout => _currentLayout;
    public bool AllowDecimal => _allowDecimal;

    public void Initialize(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void RegisterField(object field, KeyboardLayout layout, bool allowDecimal)
    {
        _activeField = field;
        _currentLayout = layout;
        _allowDecimal = allowDecimal;
        _isVisible = true;
        NotifyStateChanged();
    }

    public void HideKeyboard()
    {
        _isVisible = false;
        _activeField = null;
        NotifyStateChanged();
    }

    public void HandleKeyPress(char key)
    {
        if (_activeField == null) return;

        // Use reflection to call AppendCharacter on the active field
        var method = _activeField.GetType().GetMethod("AppendCharacter");
        method?.Invoke(_activeField, new object[] { key });
    }

    public void HandleBackspace()
    {
        if (_activeField == null) return;

        var method = _activeField.GetType().GetMethod("Backspace");
        method?.Invoke(_activeField, null);
    }

    public void HandleClear()
    {
        if (_activeField == null) return;

        var method = _activeField.GetType().GetMethod("Clear");
        method?.Invoke(_activeField, null);
    }

    private async void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
        
        // Also notify via JavaScript interop using the global keyboardState object
        if (_jsRuntime != null)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "keyboardState.notifyStateChanged",
                    _isVisible,
                    (int)_currentLayout,
                    _allowDecimal
                );
            }
            catch
            {
                // Ignore JS interop errors
            }
        }
    }
}
