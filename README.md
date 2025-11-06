# MudFieldsKeyboard

A Blazor component library providing a virtual keyboard and specialized numeric/text input fields with automatic keyboard layout switching.

## Features

- **VirtualKeyboard Component**: A customizable on-screen keyboard that sticks to the bottom of the page using flex layout
- **NumericField Component**: A generic field component supporting multiple data types:
  - Text (string)
  - Integer (int, int?)
  - Decimal (decimal, decimal?)
  - Double (double, double?)
  
- **Automatic Keyboard Layout Switching**: The keyboard layout changes based on the field type:
  - Full QWERTY layout for text fields
  - Numeric keypad for integer fields
  - Numeric keypad with decimal point for decimal/double fields

- **🎯 NEW: Centralized Keyboard Management**: Use KeyboardService to place the keyboard once in your layout and reuse it across all pages with zero boilerplate code! See [KEYBOARD_REUSABILITY.md](KEYBOARD_REUSABILITY.md) for details.

## Project Structure

- `src/MudFieldsKeyboard.Components`: The component library containing VirtualKeyboard and NumericField components
- `src/MudFieldsKeyboard.Demo`: A demo Blazor Server application showcasing the components

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later

### Building the Project

```bash
dotnet build
```

### Running the Demo

```bash
cd src/MudFieldsKeyboard.Demo
dotnet run
```

Then navigate to `http://localhost:5000` in your browser.

## Usage

### ⭐ Simplified Usage (Recommended)

The easiest way to use the keyboard is with the KeyboardService pattern:

1. Register the service in `Program.cs`:
```csharp
builder.Services.AddScoped<KeyboardService>();
```

2. Add KeyboardManager to your `MainLayout.razor`:
```razor
@using MudFieldsKeyboard.Components

<div class="page">
    @Body
</div>

<KeyboardManager />
```

3. Use NumericField in any page (no keyboard management code needed!):
```razor
@page "/mypage"
@using MudFieldsKeyboard.Components
@rendermode InteractiveServer

<NumericField @bind-Value="myValue" 
              FieldType="NumericFieldType.Integer"
              Placeholder="Enter a number" />

@code {
    private int myValue;
}
```

That's it! The keyboard automatically appears when you focus a field and adapts to the field type.

See [KEYBOARD_REUSABILITY.md](KEYBOARD_REUSABILITY.md) for complete details.

### Manual Usage (Legacy)

You can also manually manage the keyboard on each page:

```razor
@page "/example"
@rendermode InteractiveServer

<NumericField @ref="myField"
              @bind-Value="myValue"
              FieldType="NumericFieldType.Decimal"
              Placeholder="Enter a decimal value"
              OnFocus="HandleFieldFocus" />

<VirtualKeyboard IsVisible="true"
                 CurrentLayout="keyboardLayout"
                 AllowNegative="true"
                 AllowDecimal="true"
                 OnKeyPressed="HandleKeyPressed"
                 OnBackspacePressed="HandleBackspace"
                 OnClearPressed="HandleClear" />

@code {
    private decimal myValue;
    private NumericField<decimal>? myField;
    private KeyboardLayout keyboardLayout = KeyboardLayout.Numeric;
    
    private void HandleFieldFocus(NumericField<decimal> field)
    {
        keyboardLayout = KeyboardLayout.Numeric;
    }
    
    private void HandleKeyPressed(char key)
    {
        myField?.AppendCharacter(key);
    }
    
    private void HandleBackspace()
    {
        myField?.Backspace();
    }
    
    private void HandleClear()
    {
        myField?.Clear();
    }
}
```

## Components

### VirtualKeyboard

**Parameters:**
- `IsVisible` (bool): Controls keyboard visibility
- `CurrentLayout` (KeyboardLayout): The keyboard layout to display
- `AllowNegative` (bool): Shows/hides the minus button for numeric keyboards
- `AllowDecimal` (bool): Shows/hides the decimal point button for numeric keyboards
- `OnKeyPressed` (EventCallback<char>): Fired when a key is pressed
- `OnBackspacePressed` (EventCallback): Fired when backspace is pressed
- `OnClearPressed` (EventCallback): Fired when clear is pressed

### NumericField<T>

**Parameters:**
- `Value` (T?): The current field value
- `ValueChanged` (EventCallback<T?>): Fired when the value changes
- `Placeholder` (string): Placeholder text
- `Disabled` (bool): Disables the field
- `ReadOnly` (bool): Makes the field read-only
- `CssClass` (string): Custom CSS class
- `OnFocus` (EventCallback<NumericField<T>>): Fired when field receives focus
- `OnBlur` (EventCallback<NumericField<T>>): Fired when field loses focus
- `FieldType` (NumericFieldType): The type of field (Text, Integer, Decimal, Double)

**Public Methods:**
- `AppendCharacter(char c)`: Appends a character to the field
- `Backspace()`: Removes the last character
- `Clear()`: Clears the field value

## Implementation Notes

- The virtual keyboard uses CSS flexbox for sticky bottom positioning
- Fields are based on HTML text input types to properly handle numeric input with positive/negative signs and decimal separators
- The keyboard layout automatically adapts based on the FieldType parameter
- All numeric parsing uses `CultureInfo.InvariantCulture` for consistent behavior across locales

## License

This project is provided as-is for demonstration purposes.