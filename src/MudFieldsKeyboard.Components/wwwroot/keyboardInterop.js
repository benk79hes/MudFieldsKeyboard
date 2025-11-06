// Global keyboard state management - no module initialization needed
window.keyboardState = {
    dotNetHelper: null,
    
    register: function(dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
        console.log('Keyboard manager registered');
    },
    
    notifyStateChanged: function(isVisible, layout, allowDecimal) {
        if (this.dotNetHelper) {
            this.dotNetHelper.invokeMethodAsync('OnKeyboardStateChanged', isVisible, layout, allowDecimal);
        }
    },
    
    dispose: function() {
        this.dotNetHelper = null;
    }
};
