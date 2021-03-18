var markdigEditor = /** @class */ (function () {
    function markdigEditor() {
    }
    markdigEditor.register = function (dotNetObj, id) {
        var _this = markdigEditor;
        _this.dotNetObj = dotNetObj;
        _this.id = id;
        _this.textAreaElement = document.getElementById(id);
        _this.textAreaElement.addEventListener('keyup', _this.onKeyUpHandler);
    };
    markdigEditor.onKeyUpHandler = function (e) {
        var _this = markdigEditor;
        var pos = _this.textAreaElement.selectionStart;
        _this.dotNetObj.invokeMethodAsync('updateCursorPos', pos);
    };
    markdigEditor.insertTemplate = function (caretPosition, template, caretOffset) {
        var _this = markdigEditor;
        var currentText = _this.textAreaElement.value;
        var beforeInsertText = currentText.slice(0, caretPosition);
        var afterInsertText = currentText.slice(caretPosition);
        _this.textAreaElement.value = "" + beforeInsertText + template + afterInsertText;
        caretPosition += caretOffset; // until we inject an offset
        _this.textAreaElement.selectionStart = caretPosition;
        _this.textAreaElement.selectionEnd = caretPosition;
        _this.textAreaElement.focus();
    };
    return markdigEditor;
}());
//# sourceMappingURL=C:/markDigEditor.js.map