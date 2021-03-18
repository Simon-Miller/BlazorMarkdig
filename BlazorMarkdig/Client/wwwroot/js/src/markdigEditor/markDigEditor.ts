
class markdigEditor {

    private static textAreaElement: HTMLTextAreaElement;
    private static dotNetObj: any;
    private static id: string

    public static register(dotNetObj: any, id:string): void {
        const _this = markdigEditor;
        _this.dotNetObj = dotNetObj;
        _this.id = id;

        _this.textAreaElement = <HTMLTextAreaElement>document.getElementById(id);
        _this.textAreaElement.addEventListener('keyup', _this.onKeyUpHandler);
    }

    private static onKeyUpHandler(e: KeyboardEvent): void {
        const _this = markdigEditor;
        let pos: number = _this.textAreaElement.selectionStart;
        _this.dotNetObj.invokeMethodAsync('updateCursorPos', pos);
    }

    public static insertTemplate(caretPosition: number, template: string, caretOffset:number): void {
        const _this = markdigEditor;
        let currentText = _this.textAreaElement.value;
        let beforeInsertText = currentText.slice(0, caretPosition);
        let afterInsertText = currentText.slice(caretPosition);

        _this.textAreaElement.value = `${beforeInsertText}${template}${afterInsertText}`;

        caretPosition += caretOffset; // until we inject an offset
        _this.textAreaElement.selectionStart = caretPosition;
        _this.textAreaElement.selectionEnd = caretPosition;

        _this.textAreaElement.focus();
    }
}


