var markdigEditor = /** @class */ (function () {
    function markdigEditor() {
    }
    // maybe this shouold be more like a factory?  So everything can be relative?
    markdigEditor.register = function (dotNetObj, id) {
        debugger;
        var _this = markdigEditor;
        _this.dotNetObj = dotNetObj;
        _this.id = id;
        _this.textAreaElement = document.getElementById(id);
        _this.textAreaElement.addEventListener('input', _this.onInputHandler);
    };
    markdigEditor.onInputHandler = function (e) {
        //debugger;
        var _this = markdigEditor;
        var pos = _this.textAreaElement.selectionStart;
        _this.dotNetObj.invokeMethodAsync('updateCursorPos', pos);
    };
    return markdigEditor;
}());
//# sourceMappingURL=C:/markDigEditor.js.map