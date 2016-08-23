///#source 1 1 /dateFormat.js
(function (window, undefined) {
    "use strict";

    // get a list of all tokens on the source that exist on the target,
    // ordered by reverse token length
    var getSortedTokens = function (sourceDefinition, targetDefinition) {
        var tokens = [],
            id = 0;

        for (var key in sourceDefinition) {
            if (!sourceDefinition.hasOwnProperty(key)) {
                continue;
            }

            //skip things that we can't replace
            if (!targetDefinition[key]) {
                continue;
            }

            tokens.push({
                value: sourceDefinition[key],
                key: key,
                id: id++
            });
        }

        tokens.sort(function (lhs, rhs) {
            return rhs.value.length - lhs.value.length;
        });

        return tokens;
    },

    // main dateFormatConverter function
    convert = function (format, from, to) {
        var sourceTokens = getSortedTokens(from, to);

        for (var i = 0; i < sourceTokens.length; i++) {
            format = format.replace(
                new RegExp(sourceTokens[i].value, "g"),
                "{" + sourceTokens[i].id + "}");
        }

        for (var j = 0; j < sourceTokens.length; j++) {
            format = format.replace(
                new RegExp("\\{" + sourceTokens[j].id + "\\}", "g"),
                to[sourceTokens[j].key]);
        }

        return format;
    };

    window.dateFormat = {
        convert: convert
    };
}(window));
///#source 1 1 /dateFormat.dotnet.js
(function (dateFormat, undefined) {
    "use strict";

    dateFormat.dotnet = {
        "day-of-month-1": "d",
        "day-of-month-2": "dd",
        "day-of-week-abbr": "ddd",
        "day-of-week": "dddd",
        "month-1": "M",
        "month-2": "MM",
        "month-name-abbr": "MMM",
        "month-name": "MMMM",
        "year-2": "yy",
        "year-3": "yyy",
        "year-4": "yyyy",
        "am-pm-2": "tt",
        "am-pm-1": "t",
        "time-24h-1": "H",
        "time-24h-2": "HH",
        "time-12h-1": "h",
        "time-12h-2": "hh",
        "minutes-1": "m",
        "minutes-2": "mm",
        "seconds-1": "s",
        "seconds-2": "ss",
        "deciseconds-optional": "F",
        "centiseconds-optional": "FF",
        "milliseconds-optional": "FFF",
        "microseconds-optional": "FFFF",
        "nanoseconds-optional": "FFFFF",
        "picoseconds-optional": "FFFFFF",
        "femtoseconds-optional": "FFFFFFF",
        "deciseconds": "f",
        "centiseconds": "ff",
        "milliseconds": "fff",
        "microseconds": "ffff",
        "nanoseconds": "fffff",
        "picoseconds": "ffffff",
        "femtoseconds": "fffffff",
        "timezone-1": "z",
        "timezone-2": "zz",
        "timezone-3": "zzz"
    };

}(window.dateFormat));
///#source 1 1 /dateFormat.moment.js
(function (dateFormat, undefined) {
    "use strict";

    dateFormat.moment = {
        "day-of-month-1": "D",
        "day-of-month-2": "DD",
        "day-of-week-abbr": "ddd",
        "day-of-week": "dddd",
        "month-1": "M",
        "month-2": "MM",
        "month-name-abbr": "MMM",
        "month-name": "MMMM",
        "year-2": "YY",
        "year-3": "YYY",
        "year-4": "YYYY",
        "am-pm-2": "A",
        "am-pm-1": "a",
        "time-24h-1": "H",
        "time-24h-2": "HH",
        "time-12h-1": "h",
        "time-12h-2": "hh",
        "minutes-1": "m",
        "minutes-2": "mm",
        "seconds-1": "s",
        "seconds-2": "ss",
        "deciseconds-optional": "S",
        "centiseconds-optional": "SS",
        "milliseconds-optional": "SSS",
        "microseconds-optional": "SSSS",
        "nanoseconds-optional": "SSSSS",
        "picoseconds-optional": "SSSSSS",
        "femtoseconds-optional": "SSSSSSS",
        "deciseconds": "S",
        "centiseconds": "SS",
        "milliseconds": "SSS",
        "microseconds": "SSSS",
        "nanoseconds": "SSSSS",
        "picoseconds": "SSSSSS",
        "femtoseconds": "SSSSSSS",
        "timezone-1": "Z",
        "timezone-2": "ZZ",
        "timezone-3": "ZZZ"
    };

}(window.dateFormat));
///#source 1 1 /dateFormat.jqplot.js
(function (dateFormat, undefined) {
    "use strict";

    dateFormat.jqplot = {
        "day-of-month-1": "%#d",
        "day-of-month-2": "%d",
        "day-of-week-abbr": "%a",
        "day-of-week": "%A",
        "month-1": "%#m",
        "month-2": "%m",
        "month-name-abbr": "%b",
        "month-name": "%B",
        "year-2": "%y",
        "year-3": "%Y",
        "year-4": "%Y",
        "am-pm-2": "%p",
        "am-pm-1": "%p",
        "time-24h-1": "%#h",
        "time-24h-2": "%H",
        "time-12h-1": "%#I",
        "time-12h-2": "%I",
        "minutes-1": "%#M",
        "minutes-2": "%M",
        "seconds-1": "%#S",
        "seconds-2": "%S",
        "deciseconds-optional": "%#N",
        "centiseconds-optional": "%N",
        "milliseconds-optional": "%N",
        "microseconds-optional": "%N",
        "nanoseconds-optional": "%N",
        "picoseconds-optional": "%N",
        "femtoseconds-optional": "%N",
        "deciseconds": "%#N",
        "centiseconds": "%N",
        "milliseconds": "%N",
        "microseconds": "%N",
        "nanoseconds": "%N",
        "picoseconds": "%N",
        "femtoseconds": "%N",
        "timezone-1": "%G",
        "timezone-2": "%G",
        "timezone-3": "%G"
    };

}(window.dateFormat));
///#source 1 1 /dateFormat.jqueryui.js
(function (dateFormat, undefined) {
    "use strict";

    dateFormat.jqueryui = {
        "day-of-month-1": "d",
        "day-of-month-2": "dd",
        "day-of-week-abbr": "D",
        "day-of-week": "DD",
        "month-1": "m",
        "month-2": "mm",
        "month-name-abbr": "M",
        "month-name": "MM",
        "year-2": "y",
        "year-4": "yy"
    };

}(window.dateFormat));