/// <reference path="typings/jquery/jquery.d.ts" />
declare class MvcMegaForms {
    static ChangeVisuallyJQueryParentContainerSelector: string;
    static ChangeVisuallyJQueryHideEffect: string;
    static ChangeVisuallyJQueryShowEffect: string;
    static CascadeJQueryHideEffect: string;
    static CascadeJQueryShowEffect: string;
    static DetectAllFormChanges: boolean;
    static DetectChangesWarningMessage: string;
    static DetectChangesFormClass: string;
    static IgnoreDetectChangesClass: string;
    static DisabledOrReadonlyCssClass: string;
    static LeavingPageDueToSubmitOrIgnore: boolean;
    static ConfigureDetectChanges(): void;
    static AttachEvents(): void;
    static ApplyChangeVisually(dependentProperty, otherProperty, to, ifOperator, value, conditionPassesIfNull, valueTypeToCompare, valueFormat): boolean;
    static ConditionMetForChangeVisually(ifOperator, expectedValue, actualValue, conditionPassesIfNull, valueTypeToCompare, valueFormat): boolean;
    private static CascadeStringStatus;
    private static SetupCascadingDropDown(parentList);
    private static CascadeDropDown(parentList);
    private static RenderCascadedSelectOption(currChildId, initialVal, childList, currChildValue, selected);
    static GetObjectAsJQuery(obj: any): any;
    static GetFormValue(formControl): any;
    static SetControlEnabledAndWritable(ctrl: any, customDisabledOrReadonlyCssClass?: string): void;
    static SetControlDisabled(ctrl: any, customDisabledOrReadonlyCssClass?: string): void;
    static SetControlReadonly(ctrl: any, customDisabledOrReadonlyCssClass?: string): void;
    static SetControlDisabledAndReadonly(ctrl: any, disabledOrReadonlyCssClass?: string): void;
    static IsArray(item): boolean;
    static FormControlValueHasChanged(formControl): boolean;
    static FormFieldIdChanged($form): string;
    static AlertFormChanged($form): string;
    static IsNullOrUndefined(item): boolean;
}
declare module DateJs {
    class Helper {
        public providedDate: Date;
        static $VERSION: number;
        static LZ(x): string;
        static monthNames: string[];
        static monthAbbreviations: string[];
        static dayNames: string[];
        static dayAbbreviations: string[];
        public preferAmericanFormat: boolean;
        constructor(providedDate: Date, preferAmericanFormatForDefault?: boolean);
        static isInteger(val): boolean;
        static getInt(str, i, minlength, maxlength);
        static parseString(val, format: string, preferAmericanFormatForDefault?: boolean): Date;
        static isValid(val, format: string, preferAmericanFormatForDefault?: boolean): boolean;
        public isBefore(date2): boolean;
        public isAfter(date2): boolean;
        public equals(date2): boolean;
    }
}
