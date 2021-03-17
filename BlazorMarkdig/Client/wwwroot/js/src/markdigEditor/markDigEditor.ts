
class markdigEditor {

    private static textAreaElement: HTMLTextAreaElement;
    private static dotNetObj: any;
    private static id: string

    public static register(dotNetObj: any, id:string): void {
        const _this = markdigEditor;
        _this.dotNetObj = dotNetObj;
        _this.id = id;

        _this.textAreaElement = <HTMLTextAreaElement>document.getElementById(id);
        _this.textAreaElement.addEventListener('input', _this.onInputHandler);
    }

    private static onInputHandler(e: KeyboardEvent): void {
        const _this = markdigEditor;
        let pos: number = _this.textAreaElement.selectionStart;
        _this.dotNetObj.invokeMethodAsync('updateCursorPos', pos);
    }

    public static insertTemplate(caretPosition: number, template: string): void {
        const _this = markdigEditor;
        let currentText = _this.textAreaElement.value;
        let beforeInsertText = currentText.slice(0, caretPosition);
        let afterInsertText = currentText.slice(caretPosition);

        _this.textAreaElement.value = `${beforeInsertText}${template}${afterInsertText}`;

        caretPosition += template.length; // until we inject an offset
        _this.textAreaElement.selectionStart = caretPosition;
        _this.textAreaElement.selectionEnd = caretPosition;

        _this.textAreaElement.focus();
    }
}


