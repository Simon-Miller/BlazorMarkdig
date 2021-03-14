
class markdigEditor {

    private static textAreaElement: HTMLTextAreaElement;
    private static dotNetObj: any;
    private static id: string

    // maybe this shouold be more like a factory?  So everything can be relative?
    public static register(dotNetObj: any, id:string): void {
        debugger; 
        const _this = markdigEditor;
        _this.dotNetObj = dotNetObj;
        _this.id = id;

        _this.textAreaElement = <HTMLTextAreaElement>document.getElementById(id);
        _this.textAreaElement.addEventListener('input', _this.onInputHandler);
    }


    private static onInputHandler(e: KeyboardEvent): void {
        //debugger;
        const _this = markdigEditor;

        let pos: number = _this.textAreaElement.selectionStart;

        _this.dotNetObj.invokeMethodAsync('updateCursorPos', pos);
    }
}

interface Window{
    DotNet: DotNet
}
interface DotNet {
    invokeMethodAsync(methodName: string, ...args): void;
}