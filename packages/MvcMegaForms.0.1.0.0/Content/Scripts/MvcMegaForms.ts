/// <reference path="jquery.d.ts" />

/*
Copyright (c) 2012 Andrew Newton (http://about.me/nootn)
 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


//document ready
$(() => {
    "use strict";
    MvcMegaForms.AttachEvents();
    MvcMegaForms.ConfigureDetectChanges();
});

//window before unload
$(window).bind("beforeunload", function (): string {
    "use strict";
    if (!MvcMegaForms.LeavingPageDueToSubmitOrIgnore) {
        var formSearch = "form", hasPossibleFormsDetecting = true, doNoLeaveMessage = '';
        if (MvcMegaForms.IsNullOrUndefined(MvcMegaForms.DetectAllFormChanges) || MvcMegaForms.DetectAllFormChanges === false) {
            if (MvcMegaForms.IsNullOrUndefined(MvcMegaForms.DetectChangesFormClass) || MvcMegaForms.DetectChangesFormClass === '') {
                //there is no detect changes option available
                hasPossibleFormsDetecting = false;
            }
            formSearch += "." + MvcMegaForms.DetectChangesFormClass;
        }

        if (hasPossibleFormsDetecting) {
            $(formSearch).each(function (): any {
                doNoLeaveMessage = MvcMegaForms.AlertFormChanged($(this));
                if (doNoLeaveMessage !== '') {
                    return;
                }
            });
            if (doNoLeaveMessage !== '') {
                return doNoLeaveMessage;
            }
        }
    }
});

//TODO: get client side required if validation working
//if ($.validator !== undefined) {
//    $.validator.addMethod('requiredifcontains', function (val, element, dependentproperty, dependentvalue) {
//        "use strict";
//        if (!IsNullOrUndefined(val)) {
//            return false;
//        }
//        var modelPrefix = element.name.substr(0, element.name.lastIndexOf(".") + 1), otherProperty = $("[name='" + modelPrefix + dependentproperty + "']"), otherVal = GetFormValue(otherProperty).toLowerCase(), i, currValue;
//        for (i = 0; i < otherVal.length; i += 1) {
//            currValue = otherVal[i].toLowerCase();
//            if (currValue === dependentvalue) {
//                return false;
//            }
//        }
//        return true;
//    });
//    $.validator.unobtrusive.adapters.addSingleVal('requiredifcontains', 'dependentproperty', 'dependentvalue', 'requiredifcontains');

//    $.validator.addMethod('requiredifnotcontains', function (val, element, dependentproperty, dependentvalue) {
//        "use strict";
//        if (!IsNullOrUndefined(val)) {
//            return false;
//        }
//        var modelPrefix = element.name.substr(0, element.name.lastIndexOf(".") + 1), otherProperty = $("[name='" + modelPrefix + dependentproperty + "']"), otherVal = GetFormValue(otherProperty).toLowerCase(), i, currValue;
//        for (i = 0; i < otherVal.length; i += 1) {
//            currValue = otherVal[i].toLowerCase();
//            if (currValue === dependentvalue) {
//                return true;
//            }
//        }
//        return false;
//    });
//    $.validator.unobtrusive.adapters.addSingleVal('requiredifnotcontains', 'dependentproperty', 'dependentvalue', 'requiredifnotcontains');
//}

class MvcMegaForms {

    public static ChangeVisuallyJQueryParentContainerSelector: string = '.control-group';
    public static ChangeVisuallyJQueryHideEffect: string = 'fast';
    public static ChangeVisuallyJQueryShowEffect: string = 'fast';
    public static CascadeJQueryHideEffect: string = 'fast';
    public static CascadeJQueryShowEffect: string = 'fast';
    public static DetectAllFormChanges: bool = false; //set value whether to detect changes on all forms or not
    public static DetectChangesWarningMessage: string = '';  //The message to show if a form value has changed and page is being left.  If blank uses default which includes ID of first element found
    public static DetectChangesFormClass: string = 'detect-changes'; //The class to give forms that you want changes detected on if 'MvcMegaForms.DetectAllFormChanges' is false
    public static IgnoreDetectChangesClass: string = 'ignore-detect-changes'; //Add this class to any elements you want to allow to be clicked that leave the page but don't show the message (E.g. clear buttons that reset the form)
    public static DisabledOrReadonlyCssClass: string = 'ui-state-disabled';  //The class to give controls that are disabled or readonly

    private static LeavingPageDueToSubmitOrIgnore: bool = false;

    public static ConfigureDetectChanges(): void {
        "use strict";
        var formSearch = "form";
        if (IsNullOrUndefined(MvcMegaForms.DetectAllFormChanges) || MvcMegaForms.DetectAllFormChanges === false) {
            if (IsNullOrUndefined(MvcMegaForms.DetectChangesFormClass) || MvcMegaForms.DetectChangesFormClass === '') {
                //there is no detect changes option available
                return;
            }
            formSearch += "." + MvcMegaForms.DetectChangesFormClass;
        }

        $(formSearch).each(function (): any {
            var $form = $(this);

            //wire up submit buttons
            $form.find("input:submit").each(function (): any {
                var $me = $(this);
                $me.click(function () {
                    MvcMegaForms.LeavingPageDueToSubmitOrIgnore = true;
                });
            });

            //ensure all selects that have options have a selected option (otherwise it will always say they changed)
            $form.find("select").each(function (): any {
                var $me = $(this), foundDefaultSelected, $firstOption;
                if (MvcMegaForms.IsNullOrUndefined($me.attr('multiple')) && $me.find('option').length > 0) {
                    foundDefaultSelected = false;
                    $me.find('option').each(function (): any {
                        if (this.defaultSelected) {
                            foundDefaultSelected = true;
                            return;
                        }
                    });
                    if (!foundDefaultSelected) {
                        $firstOption = $me.find("option:first-child");
                        $firstOption.attr("selected", true);
                        $firstOption.attr("defaultSelected", true);
                    }
                }
            });
        });

        if (!MvcMegaForms.IsNullOrUndefined(MvcMegaForms.IgnoreDetectChangesClass)) {
            //wire up any other items to ignore which could be anywhere on the screen, not within a form
            $("." + MvcMegaForms.IgnoreDetectChangesClass).each(function (): any {
                var $me = $(this);
                $me.click(function (): void {
                    MvcMegaForms.LeavingPageDueToSubmitOrIgnore = true;
                });
            });
        }
    }

    public static AttachEvents(): void {
        "use strict";
        $(":input").each(function (): any {
            var tos,
                toValues,
                otherPropertyNames,
                ifOperators,
                values,
                conditionPassesIfNulls,
                valueTypeToCompares,
                valueFormats,
                dependentProperty,
                uniqueOtherPropertyNames,
                iOuter,
                fullName,
                otherProperty,
                iInner,
                currentOtherProperty,
                parentId,
                parentList;

            tos = $(this).attr('data-val-changevisually-to');
            if (!MvcMegaForms.IsNullOrUndefined(tos)) {
                toValues = tos.split("~");
                otherPropertyNames = $(this).attr('data-val-changevisually-otherpropertyname').split("~");
                ifOperators = $(this).attr('data-val-changevisually-ifoperator').split("~");
                values = $(this).attr('data-val-changevisually-value').split("~");
                conditionPassesIfNulls = $(this).attr('data-val-changevisually-conditionpassesifnull').split("~");
                valueTypeToCompares = $(this).attr('data-val-changevisually-valuetypetocompare').split("~");
                valueFormats = $(this).attr('data-val-changevisually-valueformat').split("~");
                dependentProperty = $(this);
                uniqueOtherPropertyNames = $.unique(otherPropertyNames.slice()); //Get each unique other property name
                //go through each 'other' field and hook up the change event
                for (iOuter = 0; iOuter < uniqueOtherPropertyNames.length; iOuter += 1) {
                    fullName = dependentProperty.attr('name').substr(0, dependentProperty.attr("name").lastIndexOf(".") + 1) + uniqueOtherPropertyNames[iOuter];
                    otherProperty = $("[name='" + fullName + "']");
                    otherProperty.change({ otherPropertyOuterInitialName: uniqueOtherPropertyNames[iOuter], otherPropertyFullName: fullName }, function (event) {
                        for (var iInner = 0; iInner < otherPropertyNames.length; iInner++) {
                            if (otherPropertyNames[iInner] === event.data.otherPropertyOuterInitialName) {
                                var currentOtherProperty = $("[name='" + event.data.otherPropertyFullName + "']");
                                if (ApplyChangeVisually(dependentProperty, currentOtherProperty, toValues[iInner], ifOperators[iInner], values[iInner], conditionPassesIfNulls[iInner], valueTypeToCompares[iInner], valueFormats[iInner])) {
                                    break; //a condition has passed, don't process the rest
                                }
                            }
                        }
                    });
                    otherProperty.change();
                }
            }

            parentId = $(this).attr("parentListId");
            if (!IsNullOrUndefined(parentId)) {
                parentList = $("[name='" + $(this).attr("name").substr(0, $(this).attr("name").lastIndexOf(".") + 1) + parentId + "']");
                parentList.attr("childid", $(this).attr('id'));
                parentList.change(function () {
                    SetupCascadingDropDown($(this));
                });
                parentList.change();
            }
        });
    }

    public static ApplyChangeVisually(dependentProperty, otherProperty, to, ifOperator, value, conditionPassesIfNull, valueTypeToCompare, valueFormat): bool {
        "use strict";
        var parentSelector = IsNullOrUndefined(MvcMegaForms.ChangeVisuallyJQueryParentContainerSelector) ? '.editor-field' : MvcMegaForms.ChangeVisuallyJQueryParentContainerSelector,
            container = dependentProperty.parents(parentSelector),
            showEffect,
            hideEffect,
            disabledOrReadonlyCssClass,
            otherPropValue,
            conditionMet;
        if (IsNullOrUndefined(container)) {
            throw 'MvcMegaForms-ChangeVisually Critical Error: Unable to find parent container with selector: ' + parentSelector + ' for property ' + dependentProperty;
        } else {
            showEffect = IsNullOrUndefined(MvcMegaForms.ChangeVisuallyJQueryShowEffect) ? 'fast' : MvcMegaForms.ChangeVisuallyJQueryShowEffect;
            hideEffect = IsNullOrUndefined(MvcMegaForms.ChangeVisuallyJQueryHideEffect) ? 'fast' : MvcMegaForms.ChangeVisuallyJQueryHideEffect;
            disabledOrReadonlyCssClass = IsNullOrUndefined(MvcMegaForms.DisabledOrReadonlyCssClass) ? 'ui-state-disabled' : MvcMegaForms.DisabledOrReadonlyCssClass;

            otherPropValue = GetFormValue(otherProperty);

            conditionMet = ConditionMetForChangeVisually(ifOperator, value, otherPropValue, conditionPassesIfNull, valueTypeToCompare, valueFormat);
            if (conditionMet) {
                if (to === 'hidden') {
                    //hide
                    container.hide(hideEffect);

                    //enable
                    SetControlEnabledAndWritable(dependentProperty);
                } else if (to === 'disabled') {
                    //show  
                    container.show(showEffect);
                    dependentProperty.removeAttr('readonly');

                    //disable
                    SetControlDisabled(dependentProperty);
                } else if (to === 'readonly') {
                    //show  
                    container.show(showEffect);
                    dependentProperty.removeAttr('disabled');

                    //disable
                    SetControlReadonly(dependentProperty);
                }
            } else {
                //show
                container.show(showEffect);

                //enable
                SetControlEnabledAndWritable(dependentProperty);
            }
            return conditionMet;
        }
    }

    public static ConditionMetForChangeVisually(ifOperator, expectedValue, actualValue, conditionPassesIfNull, valueTypeToCompare, valueFormat): bool {
        "use strict";
        //ensure it's not null or undefined before we begin
        if (IsNullOrUndefined(ifOperator)) {
            throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: ifOperator was not supplied";
        }

        var conditionMet = false,
            actualValueIsArray,
            i,
            iMet,
            iNotMet,
            currContainsItem,
            currNotContainsItem;
        conditionPassesIfNull = !(IsNullOrUndefined(conditionPassesIfNull)) && conditionPassesIfNull.toString().toLowerCase() === 'true'; //it was a string, make it a bool

        //treat empty string or undefined as null
        if (actualValue === '' || IsNullOrUndefined(actualValue)) {
            actualValue = null;
        }
        if (expectedValue === '' || IsNullOrUndefined(expectedValue)) {
            expectedValue = null;
        }

        //if the actual value is an empty array, treat it as null
        actualValueIsArray = IsArray(actualValue);
        if (actualValueIsArray) {
            if (actualValue.length <= 0) {
                actualValue = null;
            }
        }

        if (expectedValue != null && expectedValue.toString().lastIndexOf('[', 0) === 0) {
            //appears to be an array, turn it into one
            expectedValue = $.parseJSON(expectedValue.toString());
        }
        if (IsArray(expectedValue)) {
            //throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Array data type for expectedValue is not supported at this time.  expectedValue supplied was: " + expectedValue;

            //verify value types (if not known, assume string therefore no checks to do)
            switch (valueTypeToCompare) {
                case "number":
                    if (isNaN(expectedValue)) {
                        throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'number', but expectedValue was not a number: " + expectedValue;
                    }

                    if (!actualValueIsArray) {
                        if (isNaN(actualValue)) {
                            throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'number', but actualValue was not a number: " + actualValue;
                        }
                        actualValue = parseFloat(actualValue.toString());
                    } else {
                        for (i = 0; i < actualValue.length; i += 1) {
                            if (isNaN(actualValue[i])) {
                                throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'number', but an item in the actualValue array was not a number: " + actualValue[i];
                            }
                            actualValue[i] = parseFloat(actualValue[i].toString());
                        }
                    }
                    expectedValue = parseFloat(expectedValue.toString());

                    switch (ifOperator) {
                        case "equals":
                            for (iMet = 0; iMet < expectedValue.length; iMet += 1) {
                                currContainsItem = expectedValue[iMet];
                                if (currContainsItem === actualValue) {
                                    conditionMet = true;
                                    break;
                                }
                            }
                            break;
                        case "notequals":
                            conditionMet = true;
                            for (iNotMet = 0; iNotMet < expectedValue.length; iNotMet += 1) {
                                currNotContainsItem = expectedValue[iNotMet];
                                if (currNotContainsItem === actualValue) {
                                    conditionMet = false;
                                    break;
                                }
                            }
                            break;
                        default:
                            throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied when expectedValue was an array: ' + ifOperator;
                    }
                case "datetime":
                    if (!DateJs.Helper.isValid(expectedValue, valueFormat)) {
                        throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'datetime', but expectedValue does not fit the specified date format: " + expectedValue + " was not a valid DateTime in the specified format: " + valueFormat;
                    }

                    if (!actualValueIsArray) {
                        if (!DateJs.Helper.isValid(actualValue, valueFormat)) {
                            throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'datetime', but actualValue: " + actualValue + " was not a valid DateTime in the specified format: " + valueFormat;
                        }
                        actualValue = DateJs.Helper.parseString(actualValue.toString(), valueFormat);
                    } else {
                        for (i = 0; i < actualValue.length; i += 1) {
                            if (!DateJs.Helper.isValid(actualValue[i], valueFormat)) {
                                throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'datetime', but an item in the actualValue array '" + actualValue[i] + "' was not a valid DateTime in the specified format: " + valueFormat;
                            }
                            actualValue[i] = DateJs.Helper.parseString(actualValue[i].toString(), valueFormat);
                        }
                    }
                    expectedValue = DateJs.Helper.parseString(expectedValue.toString(), valueFormat);
                    switch (ifOperator) {
                        case "equals":
                            for (iMet = 0; iMet < expectedValue.length; iMet += 1) {
                                currContainsItem = expectedValue[iMet];
                                if (new DateJs.Helper(currContainsItem).equals(actualValue)) {
                                    conditionMet = true;
                                    break;
                                }
                            }
                            break;
                        case "notequals":
                            conditionMet = true;
                            for (iNotMet = 0; iNotMet < expectedValue.length; iNotMet += 1) {
                                currNotContainsItem = expectedValue[iNotMet];
                                if (new DateJs.Helper(currNotContainsItem).equals(actualValue)) {
                                    conditionMet = false;
                                    break;
                                }
                            }
                            break;
                        default:
                            throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied when expectedValue was an array: ' + ifOperator;
                    }
                default:
                    //must be a string
                    if (!actualValueIsArray) {
                        actualValue = actualValue.toString().toLowerCase();
                    }
                    expectedValue = expectedValue.toString().toLowerCase();

                    switch (ifOperator) {
                        case "equals":
                            for (iMet = 0; iMet < expectedValue.length; iMet += 1) {
                                currContainsItem = expectedValue[iMet].toString().toLowerCase();
                                if (currContainsItem === actualValue) {
                                    conditionMet = true;
                                    break;
                                }
                            }
                            break;
                        case "notequals":
                            conditionMet = true;
                            for (iNotMet = 0; iNotMet < expectedValue.length; iNotMet += 1) {
                                currNotContainsItem = expectedValue[iNotMet].toString().toLowerCase();
                                if (currNotContainsItem === actualValue) {
                                    conditionMet = false;
                                    break;
                                }
                            }
                            break;
                        default:
                            throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied when expectedValue was an array: ' + ifOperator;
                    }

            }

        } else {
            if (actualValue === null && expectedValue !== null) {
                //expectedValue is null, condition is met if we wanted it to be met when null
                conditionMet = conditionPassesIfNull;
            } else if (actualValue !== null && expectedValue === null) {
                //the expectedValue is not null, but we were looking for a null, determine what to do
                switch (ifOperator) {
                    case "equals":
                        conditionMet = false; //we wanted a null and it was not
                        break;
                    case "notequals":
                        conditionMet = true; //we did not want a null and it was not
                        break;
                    default:
                        throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: When checking for a null expectedValue, DisplayChangeIf must be Equals or NotEquals, supplied ifOperator was ' + ifOperator;
                }
            } else if (actualValue === null && expectedValue === null) {
                //both are null, condition is met if we wanted it to be met when null
                conditionMet = conditionPassesIfNull;
            } else { //both are not null
                if (actualValueIsArray) {
                    //verify that if the actual value is an array, we are only dealing with contains/notcontains
                    if (ifOperator !== "contains" && ifOperator !== "notcontains") {
                        throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: When actualValue is an array (E.g. value of a multi-select), DisplayChangeIf must be Contains or NotContains, supplied ifOperator was ' + ifOperator;
                    }
                }

                expectedValue = expectedValue.toString().toLowerCase();

                //verify value types (if not known, assume string therefore no checks to do)
                switch (valueTypeToCompare) {
                    case "number":
                        if (isNaN(expectedValue)) {
                            throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'number', but expectedValue was not a number: " + expectedValue;
                        }

                        if (!actualValueIsArray) {
                            if (isNaN(actualValue)) {
                                throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'number', but actualValue was not a number: " + actualValue;
                            }
                            actualValue = parseFloat(actualValue.toString());
                        } else {
                            for (i = 0; i < actualValue.length; i += 1) {
                                if (isNaN(actualValue[i])) {
                                    throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'number', but an item in the actualValue array was not a number: " + actualValue[i];
                                }
                                actualValue[i] = parseFloat(actualValue[i].toString());
                            }
                        }
                        expectedValue = parseFloat(expectedValue.toString());

                        switch (ifOperator) {
                            case "equals":
                                conditionMet = actualValue === expectedValue;
                                break;
                            case "notequals":
                                conditionMet = actualValue !== expectedValue;
                                break;
                            case "greaterthan":
                                conditionMet = actualValue > expectedValue;
                                break;
                            case "greaterthanorequals":
                                conditionMet = actualValue >= expectedValue;
                                break;
                            case "lessthan":
                                conditionMet = actualValue < expectedValue;
                                break;
                            case "lessthanorequals":
                                conditionMet = actualValue <= expectedValue;
                                break;
                            case "contains":
                                if (!actualValueIsArray) {
                                    conditionMet = actualValue === expectedValue; //same as equals
                                } else {
                                    for (iMet = 0; iMet < actualValue.length; iMet += 1) {
                                        currContainsItem = actualValue[iMet];
                                        if (currContainsItem === expectedValue) {
                                            conditionMet = true;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case "notcontains":
                                if (!actualValueIsArray) {
                                    conditionMet = actualValue !== expectedValue; //same as not equals
                                } else {
                                    conditionMet = true;
                                    for (iNotMet = 0; iNotMet < actualValue.length; iNotMet += 1) {
                                        currNotContainsItem = actualValue[iNotMet];
                                        if (currNotContainsItem === expectedValue) {
                                            conditionMet = false;
                                            break;
                                        }
                                    }
                                }
                                break;
                            default:
                                throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied ' + ifOperator;
                        }

                        break;
                    case "datetime":
                        if (!DateJs.Helper.isValid(expectedValue, valueFormat)) {
                            throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'datetime', but expectedValue does not fit the specified date format: " + expectedValue + " was not a valid DateTime in the specified format: " + valueFormat;
                        }

                        if (!actualValueIsArray) {
                            if (!DateJs.Helper.isValid(actualValue, valueFormat)) {
                                throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'datetime', but actualValue: " + actualValue + " was not a valid DateTime in the specified format: " + valueFormat;
                            }
                            actualValue = DateJs.Helper.parseString(actualValue.toString(), valueFormat);
                        } else {
                            for (i = 0; i < actualValue.length; i += 1) {
                                if (!DateJs.Helper.isValid(actualValue[i], valueFormat)) {
                                    throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: valueTypeToCompare was 'datetime', but an item in the actualValue array '" + actualValue[i] + "' was not a valid DateTime in the specified format: " + valueFormat;
                                }
                                actualValue[i] = DateJs.Helper.parseString(actualValue[i].toString(), valueFormat);
                            }
                        }
                        expectedValue = DateJs.Helper.parseString(expectedValue.toString(), valueFormat);

                        switch (ifOperator) {
                            case "equals":
                                conditionMet = new DateJs.Helper(actualValue).equals(expectedValue);
                                break;
                            case "notequals":
                                conditionMet = !new DateJs.Helper(actualValue).equals(expectedValue);
                                break;
                            case "greaterthan":
                                conditionMet = new DateJs.Helper(actualValue).isAfter(expectedValue);
                                break;
                            case "greaterthanorequals":
                                conditionMet = new DateJs.Helper(actualValue).equals(expectedValue) || new DateJs.Helper(actualValue).isAfter(expectedValue);
                                break;
                            case "lessthan":
                                conditionMet = new DateJs.Helper(actualValue).isBefore(expectedValue);
                                break;
                            case "lessthanorequals":
                                conditionMet = new DateJs.Helper(actualValue).equals(expectedValue) || new DateJs.Helper(actualValue).isBefore(expectedValue);
                                break;
                            case "contains":
                                if (!actualValueIsArray) {
                                    conditionMet = new DateJs.Helper(actualValue).equals(expectedValue); //same as equals
                                } else {
                                    for (iMet = 0; iMet < actualValue.length; iMet += 1) {
                                        currContainsItem = actualValue[iMet];
                                        if (new DateJs.Helper(currContainsItem).equals(expectedValue)) {
                                            conditionMet = true;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case "notcontains":
                                if (!actualValueIsArray) {
                                    conditionMet = !new DateJs.Helper(actualValue).equals(expectedValue); //same as not equals
                                } else {
                                    conditionMet = true;
                                    for (iNotMet = 0; iNotMet < actualValue.length; iNotMet += 1) {
                                        currNotContainsItem = actualValue[iNotMet];
                                        if (new DateJs.Helper(currNotContainsItem).equals(expectedValue)) {
                                            conditionMet = false;
                                            break;
                                        }
                                    }
                                }
                                break;
                            default:
                                throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied ' + ifOperator;
                        }

                        break;
                    default:
                        //must be a string
                        if (!actualValueIsArray) {
                            actualValue = actualValue.toString().toLowerCase();
                        }
                        expectedValue = expectedValue.toString().toLowerCase();
                        switch (ifOperator) {
                            case "equals":
                                conditionMet = actualValue === expectedValue;
                                break;
                            case "notequals":
                                conditionMet = actualValue !== expectedValue;
                                break;
                            case "greaterthan":
                                conditionMet = actualValue > expectedValue;
                                break;
                            case "greaterthanorequals":
                                conditionMet = actualValue >= expectedValue;
                                break;
                            case "lessthan":
                                conditionMet = actualValue < expectedValue;
                                break;
                            case "lessthanorequals":
                                conditionMet = actualValue <= expectedValue;
                                break;
                            case "contains":
                                for (iMet = 0; iMet < actualValue.length; iMet += 1) {
                                    currContainsItem = actualValue[iMet].toString().toLowerCase();
                                    if (currContainsItem === expectedValue) {
                                        conditionMet = true;
                                        break;
                                    }
                                }
                                break;
                            case "notcontains":
                                conditionMet = true;
                                for (iNotMet = 0; iNotMet < actualValue.length; iNotMet += 1) {
                                    currNotContainsItem = actualValue[iNotMet].toString().toLowerCase();
                                    if (currNotContainsItem === expectedValue) {
                                        conditionMet = false;
                                        break;
                                    }
                                }
                                break;
                            default:
                                throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied ' + ifOperator;
                        }
                        break;
                }
            }
        }
        return conditionMet;
    }

    private static CascadeStringStatus = {
        StartParentId: 0,
        StartChildId: 1,
        EndChildId: 2,
        EndChildValueWithNext: 3,
        EndChildValue: 4
    }

    private static SetupCascadingDropDown(parentList): void {
        "use strict";

        var childId,
            childList,
            childOfChild,
            indexOfLastDot,
            childNameWithoutPrefix,
            valArray;

        CascadeDropDown(parentList);

        childId = $(parentList).attr('childid');
        childList = $('#' + childId);

        //if this child has a child one, it's change event will not fire, so we must call it manually
        childOfChild = childList.attr('childid');
        if (!IsNullOrUndefined(childOfChild)) {
            childList.change();
        } else {
            //find if there are any form elements that depend on the child one for changevisually
            indexOfLastDot = childList.attr("name").lastIndexOf(".");
            childNameWithoutPrefix = childList.attr('name').substr((indexOfLastDot < 0 ? -1 : indexOfLastDot) + 1);
            $(":input[data-val-changevisually-otherpropertyname]").each(function (): any {
                valArray = $(this).attr("data-val-changevisually-otherpropertyname").split("~");
                if ($.inArray(childNameWithoutPrefix, valArray)) {
                    childList.change();
                }
            });
        }
    }

    private static CascadeDropDown(parentList): void {
        "use strict";

        var parentVal,
            childId,
            childList,
            combos,
            isChildVisible,
            hideEffect,
            initialVal,
            state,
            currParentId,
            currChildId,
            currChildValue,
            i,
            val,
            showEffect;


        parentVal = GetFormValue($(parentList));
        childId = $(parentList).attr('childid');
        childList = $('#' + childId);
        combos = childList.attr('combos');

        isChildVisible = childList.is(":visible");
        if (isChildVisible) {
            hideEffect = IsNullOrUndefined(MvcMegaForms.CascadeJQueryHideEffect) ? 'fast' : MvcMegaForms.CascadeJQueryHideEffect;
            childList.hide(hideEffect);
        }

        initialVal = GetFormValue(childList);

        childList.val(null);
        childList.empty();

        state = CascadeStringStatus.StartParentId;
        currParentId = "";
        currChildId = "";
        currChildValue = "";
        for (i = 0; i < combos.length; i += 1) {
            val = combos.charAt(i);

            //set state
            if (val === "{") {
                state = CascadeStringStatus.StartChildId;
            } else if (val === "~") {
                state = CascadeStringStatus.EndChildId;
            } else if (val === ";") {
                state = CascadeStringStatus.EndChildValueWithNext;
            } else if (val === "}") {
                state = CascadeStringStatus.EndChildValue;
            }

            //set values
            if (state === CascadeStringStatus.StartParentId) {
                currParentId += val;
            } else if (state === CascadeStringStatus.StartChildId) {
                if (currParentId === parentVal) {
                    if (val !== "{") {
                        currChildId += val;
                    }
                } else {
                    currParentId = "";
                }
            } else if (state === CascadeStringStatus.EndChildId) {
                if (currParentId !== "" && currChildId !== "") {
                    if (val !== "~") {
                        currChildValue += val;
                    }
                } else {
                    currParentId = "";
                    currChildId = "";
                }
            } else if (state === CascadeStringStatus.EndChildValueWithNext) {
                RenderCascadedSelectOption(currChildId, initialVal, childList, currChildValue);
                state = CascadeStringStatus.StartChildId;
                currChildId = "";
                currChildValue = "";
            } else if (state === CascadeStringStatus.EndChildValue) {
                RenderCascadedSelectOption(currChildId, initialVal, childList, currChildValue);
                state = CascadeStringStatus.StartParentId;
                currParentId = "";
                currChildId = "";
                currChildValue = "";
            }
        }

        if (isChildVisible) {
            showEffect = IsNullOrUndefined(MvcMegaForms.CascadeJQueryShowEffect) ? 'fast' : MvcMegaForms.CascadeJQueryShowEffect;
            childList.show(showEffect);
        }
    }

    private static RenderCascadedSelectOption(currChildId, initialVal, childList, currChildValue): void {
        "use strict";
        var isSelected = false,
            iMet,
            currContainsItem;
        if (currChildId !== "") {
            if (IsArray(initialVal)) {
                for (iMet = 0; iMet < initialVal.length; iMet += 1) {
                    currContainsItem = initialVal[iMet];
                    if (currContainsItem.toString() === currChildId.toString()) {
                        isSelected = true;
                        break;
                    }
                }
            } else {
                isSelected = currChildId === initialVal;
            }
            if (isSelected) {
                childList.append($('<option selected="selected"></option>').val(currChildId).html(currChildValue));
            } else {
                childList.append($('<option></option>').val(currChildId).html(currChildValue));
            }
        }
    }

    public static GetObjectAsJQuery(obj: any): any {
        //ensure we have a jquery object to deal with
        return obj instanceof jQuery ? obj : $(obj);
    }

    public static GetFormValue(formControl): any {
        "use strict";
        //ensure it's not null or undefined before we begin
        if (IsNullOrUndefined(formControl)) {
            throw "Undefined form control supplied";
        }

        var $formControl,
            val;

        //ensure we have a jquery object to deal with
        $formControl = GetObjectAsJQuery(formControl);

        //get the value different ways based on the type of form control
        if ($formControl.is(':checkbox')) {
            val = $formControl.is(':checked') ? true : false;
        } else if ($formControl.is(':radio')) {
            val = $("input:radio[name='" + $formControl.attr('name') + "']:checked").val();
        } else {
            val = $formControl.val();
        }
        return val;
    }

    public static SetControlEnabledAndWritable(ctrl: any, customDisabledOrReadonlyCssClass?: string): void {
        //ensure we have a jquery object to deal with
        ctrl = GetObjectAsJQuery(ctrl);

        //enable
        ctrl.removeAttr('disabled');
        ctrl.removeAttr('readonly');

        if (!IsNullOrUndefined(customDisabledOrReadonlyCssClass) && customDisabledOrReadonlyCssClass !== '') {
            ctrl.removeClass(customDisabledOrReadonlyCssClass);
        }
        else {
            ctrl.removeClass(DisabledOrReadonlyCssClass);
        }
    }

    public static SetControlDisabled(ctrl: any, customDisabledOrReadonlyCssClass?: string): void {
        //ensure we have a jquery object to deal with
        ctrl = GetObjectAsJQuery(ctrl);

        ctrl.attr('disabled', 'disabled');
        if (!IsNullOrUndefined(customDisabledOrReadonlyCssClass) && customDisabledOrReadonlyCssClass !== '') {
            ctrl.addClass(customDisabledOrReadonlyCssClass);
        }
        else {
            ctrl.addClass(DisabledOrReadonlyCssClass);
        }
    }

    public static SetControlReadonly(ctrl: any, customDisabledOrReadonlyCssClass?: string): void {
        //ensure we have a jquery object to deal with
        ctrl = GetObjectAsJQuery(ctrl);

        ctrl.attr('readonly', 'readonly');
        if (!IsNullOrUndefined(customDisabledOrReadonlyCssClass) && customDisabledOrReadonlyCssClass !== '') {
            ctrl.addClass(customDisabledOrReadonlyCssClass);
        }
        else {
            ctrl.addClass(DisabledOrReadonlyCssClass);
        }
    }

    public static SetControlDisabledAndReadonly(ctrl: any, disabledOrReadonlyCssClass?: string): void {
        //ensure we have a jquery object to deal with
        ctrl = GetObjectAsJQuery(ctrl);

        ctrl.attr('disabled', 'disabled');
        ctrl.attr('readonly', 'readonly');
        ctrl.addClass(disabledOrReadonlyCssClass);
    }

    public static IsArray(item): bool {
        "use strict";
        return (Object.prototype.toString.call(item) === '[object Array]');
    }

    public static FormControlValueHasChanged(formControl): bool {
        "use strict";

        var $formControl,
            allCndMet,
            currOpt,
            returnVal,
            i;

        $formControl = $(formControl);

        if ($formControl.is(':checkbox')) {
            returnVal = (formControl.checked !== formControl.defaultChecked);
        } else if ($formControl.is(':radio')) {
            returnVal = (formControl.checked !== formControl.defaultChecked);
        } else if ($formControl.is('select') && !IsNullOrUndefined($formControl.attr('multiple'))) {
            if (IsNullOrUndefined(formControl.options) || formControl.options.length <= 0) {
                returnVal = false;
            } else {
                allCndMet = false;
                for (i = 0; i < formControl.options.length; i += 1) {
                    currOpt = formControl.options[i];
                    allCndMet = allCndMet || (currOpt.selected !== currOpt.defaultSelected);
                }
                returnVal = allCndMet;
            }
        } else if ($formControl.is('select')) {
            if (IsNullOrUndefined(formControl.options) || formControl.options.length <= 0) {
                returnVal = false;
            } else {
                returnVal = !(formControl.options[formControl.selectedIndex].defaultSelected);
            }
        } else {
            returnVal = (formControl.value !== formControl.defaultValue);
        }
        return returnVal;
    }

    public static FormFieldIdChanged($form): string {
        "use strict";
        var changedId = null;
        $form.find('input').each(function () {
            //specifically leave 'this' as non-jquery
            if (FormControlValueHasChanged(this)) {
                changedId = IsNullOrUndefined(this.id) ? IsNullOrUndefined(this.name) ? '[unknown]' : this.name : this.id;
                return;
            }
        });
        $form.find('textarea').each(function () {
            //specifically leave 'this' as non-jquery
            if (FormControlValueHasChanged(this)) {
                changedId = IsNullOrUndefined(this.id) ? IsNullOrUndefined(this.name) ? '[unknown]' : this.name : this.id;
                return;
            }
        });
        if (IsNullOrUndefined(changedId)) {
            $form.find('select').each(function () {
                //specifically leave 'this' as non-jquery
                if (FormControlValueHasChanged(this)) {
                    changedId = IsNullOrUndefined(this.id) ? IsNullOrUndefined(this.name) ? '[unknown]' : this.name : this.id;
                    return;
                }
            });
        }
        return changedId;
    }

    public static AlertFormChanged($form): string {
        "use strict";

        var changedId,
            confMsg;

        changedId = FormFieldIdChanged($form);
        if (!IsNullOrUndefined(changedId)) {
            confMsg = "At least one unsaved value has changed ('" + changedId + "'), are you sure you want to leave the page?";
            if (!IsNullOrUndefined(MvcMegaForms.DetectChangesWarningMessage) && MvcMegaForms.DetectChangesWarningMessage !== '') {
                confMsg = MvcMegaForms.DetectChangesWarningMessage;
            }
            return confMsg;
        }
        return '';
    }

    public static IsNullOrUndefined(item): bool {
        "use strict";
        return (item == null || item === undefined || typeof item == 'undefined');
    }

}

module DateJs {
    export class Helper {

        /*
        NOTE: TypeScript code somewhat ported from JS library:
        * Copyright (c)2005-2009 Matt Kruse (javascripttoolbox.com)
        * 
        * Dual licensed under the MIT and GPL licenses. 
        * This basically means you can use this code however you want for
        * free, but don't claim to have written it yourself!
        * Donations always accepted: http://www.JavascriptToolbox.com/donate/
        * 
        * Please do not link to the .js files on javascripttoolbox.com from
        * your site. Copy the files locally to your server instead.
        * 
        */

        static $VERSION = 1.02;
        // Utility function to append a 0 to single-digit numbers
        static LZ(x) { return (x < 0 || x > 9 ? "" : "0") + x };
        // Full month names. Change this for local month names
        static monthNames: string[] = new Array('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December');
        // Month abbreviations. Change this for local month names
        static monthAbbreviations: string[] = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
        // Full day names. Change this for local month names
        static dayNames: string[] = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
        // Day abbreviations. Change this for local month names
        static dayAbbreviations: string[] = new Array('Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');

        // Used for parsing ambiguous dates like 1/2/2000 - default to preferring 'American' format meaning Jan 2.
        // Set to false to prefer 'European' format meaning Feb 1
        preferAmericanFormat: bool;

        constructor(public providedDate: Date, preferAmericanFormatForDefault?: bool = true) {
            this.preferAmericanFormat = preferAmericanFormatForDefault;
        }

        static isInteger(val) {
            for (var i = 0; i < val.length; i++) {
                if ("1234567890".indexOf(val.charAt(i)) == -1) {
                    return false;
                }
            }
            return true;
        }

        static getInt(str, i, minlength, maxlength) {
            for (var x = maxlength; x >= minlength; x--) {
                var token = str.substring(i, i + x);
                if (token.length < minlength) {
                    return null;
                }
                if (this.isInteger(token)) {
                    return token;
                }
            }
            return null;
        }

        // Parse a string and convert it to a Date object.
        // If no format is passed, try a list of common formats.
        // If string cannot be parsed, return null.
        // Avoids regular expressions to be more portable.
        static parseString(val, format: string, preferAmericanFormatForDefault?: bool = true): Date {
            // If no format is specified, try a few common formats
            if (typeof (format) == "undefined" || format == null || format == "") {
                var generalFormats = new Array('y-M-d', 'MMM d, y', 'MMM d,y', 'y-MMM-d', 'd-MMM-y', 'MMM d', 'MMM-d', 'd-MMM');
                var monthFirst = new Array('M/d/y', 'M-d-y', 'M.d.y', 'M/d', 'M-d');
                var dateFirst = new Array('d/M/y', 'd-M-y', 'd.M.y', 'd/M', 'd-M');
                var checkList = new Array(generalFormats, preferAmericanFormatForDefault ? monthFirst : dateFirst, preferAmericanFormatForDefault ? dateFirst : monthFirst);
                for (var i = 0; i < checkList.length; i++) {
                    var l = checkList[i];
                    for (var j = 0; j < l.length; j++) {
                        var d = this.parseString(val, l[j]);
                        if (d != null) {
                            return d;
                        }
                    }
                }
                return null;
            };


            val = val + "";
            format = format + "";
            var i_val = 0;
            var i_format = 0;
            var c = "";
            var token = "";
            var token2 = "";
            var x, y;
            var year = new Date().getFullYear();
            var month = 1;
            var date = 1;
            var hh = 0;
            var mm = 0;
            var ss = 0;
            var ampm = "";
            while (i_format < format.length) {
                // Get next token from format string
                c = format.charAt(i_format);
                token = "";
                while ((format.charAt(i_format) == c) && (i_format < format.length)) {
                    token += format.charAt(i_format++);
                }
                // Extract contents of value based on format token
                if (token == "yyyy" || token == "yy" || token == "y") {
                    if (token == "yyyy") {
                        x = 4; y = 4;
                    }
                    if (token == "yy") {
                        x = 2; y = 2;
                    }
                    if (token == "y") {
                        x = 2; y = 4;
                    }
                    year = this.getInt(val, i_val, x, y);
                    if (year == null) {
                        return null;
                    }
                    i_val += year.toString().length;
                    if (year.toString().length == 2) {
                        if (year > 70) {
                            year = 1900 + (year - 0);
                        }
                        else {
                            year = 2000 + (year - 0);
                        }
                    }
                }
                else if (token == "MMM" || token == "NNN") {
                    month = 0;
                    var names = (token == "MMM" ? (monthNames.concat(monthAbbreviations)) : monthAbbreviations);
                    for (var i = 0; i < names.length; i++) {
                        var month_name = names[i];
                        if (val.substring(i_val, i_val + month_name.length).toLowerCase() == month_name.toLowerCase()) {
                            month = (i % 12) + 1;
                            i_val += month_name.length;
                            break;
                        }
                    }
                    if ((month < 1) || (month > 12)) {
                        return null;
                    }
                }
                else if (token == "EE" || token == "E") {
                    var names = (token == "EE" ? dayNames : dayAbbreviations);
                    for (var i = 0; i < names.length; i++) {
                        var day_name = names[i];
                        if (val.substring(i_val, i_val + day_name.length).toLowerCase() == day_name.toLowerCase()) {
                            i_val += day_name.length;
                            break;
                        }
                    }
                }
                else if (token == "MM" || token == "M") {
                    month = this.getInt(val, i_val, token.length, 2);
                    if (month == null || (month < 1) || (month > 12)) {
                        return null;
                    }
                    i_val += month.toString().length;
                }
                else if (token == "dd" || token == "d") {
                    date = this.getInt(val, i_val, token.length, 2);
                    if (date == null || (date < 1) || (date > 31)) {
                        return null;
                    }
                    i_val += date.toString().length;
                }
                else if (token == "hh" || token == "h") {
                    hh = this.getInt(val, i_val, token.length, 2);
                    if (hh == null || (hh < 1) || (hh > 12)) {
                        return null;
                    }
                    i_val += hh.toString().length;
                }
                else if (token == "HH" || token == "H") {
                    hh = this.getInt(val, i_val, token.length, 2);
                    if (hh == null || (hh < 0) || (hh > 23)) {
                        return null;
                    }
                    i_val += hh.toString().length;
                }
                else if (token == "KK" || token == "K") {
                    hh = this.getInt(val, i_val, token.length, 2);
                    if (hh == null || (hh < 0) || (hh > 11)) {
                        return null;
                    }
                    i_val += hh.toString().length;
                    hh++;
                }
                else if (token == "kk" || token == "k") {
                    hh = this.getInt(val, i_val, token.length, 2);
                    if (hh == null || (hh < 1) || (hh > 24)) {
                        return null;
                    }
                    i_val += hh.toString().length;
                    hh--;
                }
                else if (token == "mm" || token == "m") {
                    mm = this.getInt(val, i_val, token.length, 2);
                    if (mm == null || (mm < 0) || (mm > 59)) {
                        return null;
                    }
                    i_val += mm.toString().length;
                }
                else if (token == "ss" || token == "s") {
                    ss = this.getInt(val, i_val, token.length, 2);
                    if (ss == null || (ss < 0) || (ss > 59)) {
                        return null;
                    }
                    i_val += ss.toString().length;
                }
                else if (token == "a") {
                    if (val.substring(i_val, i_val + 2).toLowerCase() == "am") {
                        ampm = "AM";
                    }
                    else if (val.substring(i_val, i_val + 2).toLowerCase() == "pm") {
                        ampm = "PM";
                    }
                    else {
                        return null;
                    }
                    i_val += 2;
                }
                else {
                    if (val.substring(i_val, i_val + token.length) != token) {
                        return null;
                    }
                    else {
                        i_val += token.length;
                    }
                }
            }
            // If there are any trailing characters left in the value, it doesn't match
            if (i_val != val.length) {
                return null;
            }
            // Is date valid for month?
            if (month == 2) {
                // Check for leap year
                if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) { // leap year
                    if (date > 29) {
                        return null;
                    }
                }
                else {
                    if (date > 28) {
                        return null;
                    }
                }
            }
            if ((month == 4) || (month == 6) || (month == 9) || (month == 11)) {
                if (date > 30) {
                    return null;
                }
            }
            // Correct hours value
            if (hh < 12 && ampm == "PM") {
                hh = hh - 0 + 12;
            }
            else if (hh > 11 && ampm == "AM") {
                hh -= 12;
            }
            return new Date(year, month - 1, date, hh, mm, ss);
        }

        static isValid(val, format: string, preferAmericanFormatForDefault?: bool = true): bool {
            return (parseString(val, format, preferAmericanFormatForDefault) != null);
        }

        public isBefore(date2) {
            if (date2 == null) {
                return false;
            }
            return (this.providedDate.getTime() < date2.getTime());
        }

        public isAfter(date2) {
            if (date2 == null) {
                return false;
            }
            return (this.providedDate.getTime() > date2.getTime());
        }

        public equals(date2) {
            if (date2 == null) {
                return false;
            }
            return (this.providedDate.getTime() == date2.getTime());
        }
    }
}
