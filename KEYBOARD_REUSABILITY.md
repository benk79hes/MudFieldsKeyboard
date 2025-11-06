# Keyboard Component Reusability - Implementation Guide

## Problem Statement

The original implementation required each page to:
1. Maintain references to all field components
2. Track the active field
3. Manage keyboard visibility state
4. Handle keyboard layout switching
5. Implement event handlers for all keyboard actions (key press, backspace, clear, close)

This led to significant code duplication across pages and made the keyboard difficult to reuse.

## Solution Overview

We've implemented a **KeyboardService** pattern that centralizes keyboard state management and allows the keyboard component to be placed once in the layout and automatically work with all pages.

### Key Components

1. **KeyboardService** - A scoped service that manages:
   - Active field tracking
   - Keyboard visibility
   - Current layout and settings
   - Event handling for key presses, backspace, and clear

2. **KeyboardManager** - A component that wraps the VirtualKeyboard and connects it to the KeyboardService

3. **Enhanced NumericField** - Updated to automatically register with KeyboardService when focused

### Architecture

```
MainLayout.razor
  └── KeyboardManager (subscribes to KeyboardService)
      └── VirtualKeyboard

Pages (Home, SimplifiedDemo, etc.)
  └── NumericField components (auto-register with KeyboardService on focus)
```

## Usage

### Step 1: Register the Service

In `Program.cs`:

```csharp
builder.Services.AddScoped<KeyboardService>();
```

### Step 2: Add KeyboardManager to Layout

In `MainLayout.razor`:

```razor
@using MudFieldsKeyboard.Components

<div class="page">
    <!-- your layout content -->
    @Body
</div>

<KeyboardManager />
```

### Step 3: Use NumericField in Pages

In any page:

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

That's it! No keyboard management code needed in the page.

## Benefits

1. **No Boilerplate** - Pages don't need keyboard management code
2. **Centralized** - One keyboard instance serves all pages
3. **Automatic** - Fields auto-register when focused
4. **Smart Switching** - Keyboard layout automatically adapts to field type
5. **Reusable** - Easy to add keyboard support to new pages

## Backward Compatibility

The old pattern still works! Set `UseKeyboardService="false"` on NumericField:

```razor
<NumericField @bind-Value="myValue"
              UseKeyboardService="false"
              OnFocus="HandleManualFocus" />
```

## Implementation Details

### KeyboardService

- **Scope**: Scoped (per user/circuit in Blazor Server)
- **State**: Tracks active field, layout, visibility
- **Events**: Raises `OnStateChanged` when state changes
- **Thread-safe**: Uses event-based notification

### Field Registration

When a NumericField receives focus:
1. Determines appropriate keyboard layout based on FieldType
2. Calls `KeyboardService.RegisterField()` with layout info
3. Service sets visibility and notifies subscribers
4. KeyboardManager re-renders with new state

### Key Press Handling

1. User clicks keyboard button
2. VirtualKeyboard fires OnKeyPressed event
3. KeyboardManager calls `KeyboardService.HandleKeyPress()`
4. Service uses reflection to call `AppendCharacter()` on active field
5. Field updates its value

## Testing

The implementation has been tested with:
- Text fields (QWERTY layout)
- Integer fields (numeric keypad, no decimal)
- Decimal/Double fields (numeric keypad with decimal point)
- Multiple pages using the same keyboard
- Navigation between pages while keyboard is visible

## Next Steps

Consider adding:
1. Custom keyboard layouts
2. Field validation integration
3. Keyboard shortcuts
4. Accessibility enhancements (ARIA labels, keyboard navigation)
5. Mobile responsiveness improvements
