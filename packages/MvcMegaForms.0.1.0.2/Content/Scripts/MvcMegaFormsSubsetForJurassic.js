/*

Ideally Jurassic would just reference the same script as the client side, but this was not quite possible, so this version will need to be kept in sync.

To copy over from the client version:
1) Replace "MvcMegaForms." with ""
2) Replace "$.parseJSON" with "JSON.parse"

*/

function ConditionMetForChangeVisually(ifOperator, expectedValue, actualValue, conditionPassesIfNull, valueTypeToCompare, valueFormat) {
    "use strict";
    if (IsNullOrUndefined(ifOperator)) {
        throw "MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: ifOperator was not supplied";
    }
    var conditionMet = false, actualValueIsArray, i, iMet, iNotMet, currContainsItem, currNotContainsItem;
    conditionPassesIfNull = !(IsNullOrUndefined(conditionPassesIfNull)) && conditionPassesIfNull.toString().toLowerCase() === 'true';
    if (actualValue === '' || IsNullOrUndefined(actualValue)) {
        actualValue = null;
    }
    if (expectedValue === '' || IsNullOrUndefined(expectedValue)) {
        expectedValue = null;
    }
    actualValueIsArray = IsArray(actualValue);
    if (actualValueIsArray) {
        if (actualValue.length <= 0) {
            actualValue = null;
        }
    }
    if (expectedValue != null && expectedValue.toString().lastIndexOf('[', 0) === 0) {
        expectedValue = JSON.parse(expectedValue.toString());
    }
    if (IsArray(expectedValue)) {
        switch (valueTypeToCompare) {
            case "number": {
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
                    case "equals": {
                        for (iMet = 0; iMet < expectedValue.length; iMet += 1) {
                            currContainsItem = expectedValue[iMet];
                            if (currContainsItem === actualValue) {
                                conditionMet = true;
                                break;
                            }
                        }
                        break;

                    }
                    case "notequals": {
                        conditionMet = true;
                        for (iNotMet = 0; iNotMet < expectedValue.length; iNotMet += 1) {
                            currNotContainsItem = expectedValue[iNotMet];
                            if (currNotContainsItem === actualValue) {
                                conditionMet = false;
                                break;
                            }
                        }
                        break;

                    }
                    default: {
                        throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied when expectedValue was an array: ' + ifOperator;

                    }
                }

            }
            case "datetime": {
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
                    case "equals": {
                        for (iMet = 0; iMet < expectedValue.length; iMet += 1) {
                            currContainsItem = expectedValue[iMet];
                            if (new DateJs.Helper(currContainsItem).equals(actualValue)) {
                                conditionMet = true;
                                break;
                            }
                        }
                        break;

                    }
                    case "notequals": {
                        conditionMet = true;
                        for (iNotMet = 0; iNotMet < expectedValue.length; iNotMet += 1) {
                            currNotContainsItem = expectedValue[iNotMet];
                            if (new DateJs.Helper(currNotContainsItem).equals(actualValue)) {
                                conditionMet = false;
                                break;
                            }
                        }
                        break;

                    }
                    default: {
                        throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied when expectedValue was an array: ' + ifOperator;

                    }
                }

            }
            default: {
                if (!actualValueIsArray) {
                    actualValue = actualValue.toString().toLowerCase();
                }
                expectedValue = expectedValue.toString().toLowerCase();
                switch (ifOperator) {
                    case "equals": {
                        for (iMet = 0; iMet < expectedValue.length; iMet += 1) {
                            currContainsItem = expectedValue[iMet].toString().toLowerCase();
                            if (currContainsItem === actualValue) {
                                conditionMet = true;
                                break;
                            }
                        }
                        break;

                    }
                    case "notequals": {
                        conditionMet = true;
                        for (iNotMet = 0; iNotMet < expectedValue.length; iNotMet += 1) {
                            currNotContainsItem = expectedValue[iNotMet].toString().toLowerCase();
                            if (currNotContainsItem === actualValue) {
                                conditionMet = false;
                                break;
                            }
                        }
                        break;

                    }
                    default: {
                        throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied when expectedValue was an array: ' + ifOperator;

                    }
                }

            }
        }
    } else {
        if (actualValue === null && expectedValue !== null) {
            conditionMet = conditionPassesIfNull;
        } else {
            if (actualValue !== null && expectedValue === null) {
                switch (ifOperator) {
                    case "equals": {
                        conditionMet = false;
                        break;

                    }
                    case "notequals": {
                        conditionMet = true;
                        break;

                    }
                    default: {
                        throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: When checking for a null expectedValue, DisplayChangeIf must be Equals or NotEquals, supplied ifOperator was ' + ifOperator;

                    }
                }
            } else {
                if (actualValue === null && expectedValue === null) {
                    conditionMet = conditionPassesIfNull;
                } else {
                    if (actualValueIsArray) {
                        if (ifOperator !== "contains" && ifOperator !== "notcontains") {
                            throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: When actualValue is an array (E.g. value of a multi-select), DisplayChangeIf must be Contains or NotContains, supplied ifOperator was ' + ifOperator;
                        }
                    }
                    expectedValue = expectedValue.toString().toLowerCase();
                    switch (valueTypeToCompare) {
                        case "number": {
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
                                case "equals": {
                                    conditionMet = actualValue === expectedValue;
                                    break;

                                }
                                case "notequals": {
                                    conditionMet = actualValue !== expectedValue;
                                    break;

                                }
                                case "greaterthan": {
                                    conditionMet = actualValue > expectedValue;
                                    break;

                                }
                                case "greaterthanorequals": {
                                    conditionMet = actualValue >= expectedValue;
                                    break;

                                }
                                case "lessthan": {
                                    conditionMet = actualValue < expectedValue;
                                    break;

                                }
                                case "lessthanorequals": {
                                    conditionMet = actualValue <= expectedValue;
                                    break;

                                }
                                case "contains": {
                                    if (!actualValueIsArray) {
                                        conditionMet = actualValue === expectedValue;
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

                                }
                                case "notcontains": {
                                    if (!actualValueIsArray) {
                                        conditionMet = actualValue !== expectedValue;
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

                                }
                                default: {
                                    throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied ' + ifOperator;

                                }
                            }
                            break;

                        }
                        case "datetime": {
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
                                case "equals": {
                                    conditionMet = new DateJs.Helper(actualValue).equals(expectedValue);
                                    break;

                                }
                                case "notequals": {
                                    conditionMet = !new DateJs.Helper(actualValue).equals(expectedValue);
                                    break;

                                }
                                case "greaterthan": {
                                    conditionMet = new DateJs.Helper(actualValue).isAfter(expectedValue);
                                    break;

                                }
                                case "greaterthanorequals": {
                                    conditionMet = new DateJs.Helper(actualValue).equals(expectedValue) || new DateJs.Helper(actualValue).isAfter(expectedValue);
                                    break;

                                }
                                case "lessthan": {
                                    conditionMet = new DateJs.Helper(actualValue).isBefore(expectedValue);
                                    break;

                                }
                                case "lessthanorequals": {
                                    conditionMet = new DateJs.Helper(actualValue).equals(expectedValue) || new DateJs.Helper(actualValue).isBefore(expectedValue);
                                    break;

                                }
                                case "contains": {
                                    if (!actualValueIsArray) {
                                        conditionMet = new DateJs.Helper(actualValue).equals(expectedValue);
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

                                }
                                case "notcontains": {
                                    if (!actualValueIsArray) {
                                        conditionMet = !new DateJs.Helper(actualValue).equals(expectedValue);
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

                                }
                                default: {
                                    throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied ' + ifOperator;

                                }
                            }
                            break;

                        }
                        default: {
                            if (!actualValueIsArray) {
                                actualValue = actualValue.toString().toLowerCase();
                            }
                            expectedValue = expectedValue.toString().toLowerCase();
                            switch (ifOperator) {
                                case "equals": {
                                    conditionMet = actualValue === expectedValue;
                                    break;

                                }
                                case "notequals": {
                                    conditionMet = actualValue !== expectedValue;
                                    break;

                                }
                                case "greaterthan": {
                                    conditionMet = actualValue > expectedValue;
                                    break;

                                }
                                case "greaterthanorequals": {
                                    conditionMet = actualValue >= expectedValue;
                                    break;

                                }
                                case "lessthan": {
                                    conditionMet = actualValue < expectedValue;
                                    break;

                                }
                                case "lessthanorequals": {
                                    conditionMet = actualValue <= expectedValue;
                                    break;

                                }
                                case "contains": {
                                    for (iMet = 0; iMet < actualValue.length; iMet += 1) {
                                        currContainsItem = actualValue[iMet].toString().toLowerCase();
                                        if (currContainsItem === expectedValue) {
                                            conditionMet = true;
                                            break;
                                        }
                                    }
                                    break;

                                }
                                case "notcontains": {
                                    conditionMet = true;
                                    for (iNotMet = 0; iNotMet < actualValue.length; iNotMet += 1) {
                                        currNotContainsItem = actualValue[iNotMet].toString().toLowerCase();
                                        if (currNotContainsItem === expectedValue) {
                                            conditionMet = false;
                                            break;
                                        }
                                    }
                                    break;

                                }
                                default: {
                                    throw 'MvcMegaForms-ChangeVisually Critical Error in ConditionMetForChangeVisually: Unhandled ifOperator was supplied ' + ifOperator;

                                }
                            }
                            break;

                        }
                    }
                }
            }
        }
    }
    return conditionMet;
}

function IsArray(item) {
    "use strict";
    return (Object.prototype.toString.call(item) === '[object Array]');
}

function IsNullOrUndefined(item) {
    "use strict";
    return (item == null || item === undefined || typeof item == 'undefined');
}

var DateJs;
(function (DateJs) {
    var Helper = (function () {
        function Helper(providedDate, preferAmericanFormatForDefault) {
            if (typeof preferAmericanFormatForDefault === "undefined") { preferAmericanFormatForDefault = true; }
            this.providedDate = providedDate;
            this.preferAmericanFormat = preferAmericanFormatForDefault;
        }
        Helper.$VERSION = 1.02;
        Helper.LZ = function LZ(x) {
            return (x < 0 || x > 9 ? "" : "0") + x;
        }
        Helper.monthNames = new Array('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December');
        Helper.monthAbbreviations = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
        Helper.dayNames = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
        Helper.dayAbbreviations = new Array('Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');
        Helper.isInteger = function isInteger(val) {
            for (var i = 0; i < val.length; i++) {
                if ("1234567890".indexOf(val.charAt(i)) == -1) {
                    return false;
                }
            }
            return true;
        }
        Helper.getInt = function getInt(str, i, minlength, maxlength) {
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
        Helper.parseString = function parseString(val, format, preferAmericanFormatForDefault) {
            if (typeof preferAmericanFormatForDefault === "undefined") { preferAmericanFormatForDefault = true; }
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
            }
            ;;
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
                c = format.charAt(i_format);
                token = "";
                while ((format.charAt(i_format) == c) && (i_format < format.length)) {
                    token += format.charAt(i_format++);
                }
                if (token == "yyyy" || token == "yy" || token == "y") {
                    if (token == "yyyy") {
                        x = 4;
                        y = 4;
                    }
                    if (token == "yy") {
                        x = 2;
                        y = 2;
                    }
                    if (token == "y") {
                        x = 2;
                        y = 4;
                    }
                    year = this.getInt(val, i_val, x, y);
                    if (year == null) {
                        return null;
                    }
                    i_val += year.toString().length;
                    if (year.toString().length == 2) {
                        if (year > 70) {
                            year = 1900 + (year - 0);
                        } else {
                            year = 2000 + (year - 0);
                        }
                    }
                } else {
                    if (token == "MMM" || token == "NNN") {
                        month = 0;
                        var names = (token == "MMM" ? (Helper.monthNames.concat(Helper.monthAbbreviations)) : Helper.monthAbbreviations);
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
                    } else {
                        if (token == "EE" || token == "E") {
                            var names = (token == "EE" ? Helper.dayNames : Helper.dayAbbreviations);
                            for (var i = 0; i < names.length; i++) {
                                var day_name = names[i];
                                if (val.substring(i_val, i_val + day_name.length).toLowerCase() == day_name.toLowerCase()) {
                                    i_val += day_name.length;
                                    break;
                                }
                            }
                        } else {
                            if (token == "MM" || token == "M") {
                                month = this.getInt(val, i_val, token.length, 2);
                                if (month == null || (month < 1) || (month > 12)) {
                                    return null;
                                }
                                i_val += month.toString().length;
                            } else {
                                if (token == "dd" || token == "d") {
                                    date = this.getInt(val, i_val, token.length, 2);
                                    if (date == null || (date < 1) || (date > 31)) {
                                        return null;
                                    }
                                    i_val += date.toString().length;
                                } else {
                                    if (token == "hh" || token == "h") {
                                        hh = this.getInt(val, i_val, token.length, 2);
                                        if (hh == null || (hh < 1) || (hh > 12)) {
                                            return null;
                                        }
                                        i_val += hh.toString().length;
                                    } else {
                                        if (token == "HH" || token == "H") {
                                            hh = this.getInt(val, i_val, token.length, 2);
                                            if (hh == null || (hh < 0) || (hh > 23)) {
                                                return null;
                                            }
                                            i_val += hh.toString().length;
                                        } else {
                                            if (token == "KK" || token == "K") {
                                                hh = this.getInt(val, i_val, token.length, 2);
                                                if (hh == null || (hh < 0) || (hh > 11)) {
                                                    return null;
                                                }
                                                i_val += hh.toString().length;
                                                hh++;
                                            } else {
                                                if (token == "kk" || token == "k") {
                                                    hh = this.getInt(val, i_val, token.length, 2);
                                                    if (hh == null || (hh < 1) || (hh > 24)) {
                                                        return null;
                                                    }
                                                    i_val += hh.toString().length;
                                                    hh--;
                                                } else {
                                                    if (token == "mm" || token == "m") {
                                                        mm = this.getInt(val, i_val, token.length, 2);
                                                        if (mm == null || (mm < 0) || (mm > 59)) {
                                                            return null;
                                                        }
                                                        i_val += mm.toString().length;
                                                    } else {
                                                        if (token == "ss" || token == "s") {
                                                            ss = this.getInt(val, i_val, token.length, 2);
                                                            if (ss == null || (ss < 0) || (ss > 59)) {
                                                                return null;
                                                            }
                                                            i_val += ss.toString().length;
                                                        } else {
                                                            if (token == "a") {
                                                                if (val.substring(i_val, i_val + 2).toLowerCase() == "am") {
                                                                    ampm = "AM";
                                                                } else {
                                                                    if (val.substring(i_val, i_val + 2).toLowerCase() == "pm") {
                                                                        ampm = "PM";
                                                                    } else {
                                                                        return null;
                                                                    }
                                                                }
                                                                i_val += 2;
                                                            } else {
                                                                if (val.substring(i_val, i_val + token.length) != token) {
                                                                    return null;
                                                                } else {
                                                                    i_val += token.length;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (i_val != val.length) {
                return null;
            }
            if (month == 2) {
                if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) {
                    if (date > 29) {
                        return null;
                    }
                } else {
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
            if (hh < 12 && ampm == "PM") {
                hh = hh - 0 + 12;
            } else {
                if (hh > 11 && ampm == "AM") {
                    hh -= 12;
                }
            }
            return new Date(year, month - 1, date, hh, mm, ss);
        }
        Helper.isValid = function isValid(val, format, preferAmericanFormatForDefault) {
            if (typeof preferAmericanFormatForDefault === "undefined") { preferAmericanFormatForDefault = true; }
            return (Helper.parseString(val, format, preferAmericanFormatForDefault) != null);
        }
        Helper.prototype.isBefore = function (date2) {
            if (date2 == null) {
                return false;
            }
            return (this.providedDate.getTime() < date2.getTime());
        };
        Helper.prototype.isAfter = function (date2) {
            if (date2 == null) {
                return false;
            }
            return (this.providedDate.getTime() > date2.getTime());
        };
        Helper.prototype.equals = function (date2) {
            if (date2 == null) {
                return false;
            }
            return (this.providedDate.getTime() == date2.getTime());
        };
        return Helper;
    })();
    DateJs.Helper = Helper;
})(DateJs || (DateJs = {}));
