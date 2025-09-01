import {
  Alert,
  AuthService,
  CreateSessionCommandSchema,
  DefaultValueAccessor,
  FormBuilder,
  FormControlName,
  FormGroupDirective,
  FormInput,
  InvalidPipe,
  NgControlStatus,
  NgControlStatusGroup,
  NgSelectOption,
  NumberValueAccessor,
  ReactiveFormsModule,
  SelectControlValueAccessor,
  buildValidations,
  clientOptions,
  ɵNgNoValidate,
  ɵNgSelectMultipleOption
} from "./chunk-67T4FLOB.js";
import {
  HttpClient
} from "./chunk-IXWTYFU2.js";
import {
  AsyncPipe,
  CommonModule,
  DatePipe,
  NgStyle
} from "./chunk-ABNVDUIW.js";
import {
  animate,
  sequence,
  style,
  transition,
  trigger
} from "./chunk-HNJAQDA3.js";
import {
  ANIMATION_MODULE_TYPE,
  BehaviorSubject,
  Component,
  DOCUMENT,
  EventEmitter,
  HostListener,
  Inject,
  Injectable,
  Input,
  Output,
  RendererFactory2,
  RuntimeError,
  Subject,
  ViewChild,
  ViewEncapsulation,
  __spreadProps,
  __spreadValues,
  catchError,
  distinctUntilChanged,
  inject,
  map,
  mergeMap,
  of,
  setClassMetadata,
  startWith,
  switchMap,
  take,
  timer,
  ɵsetClassDebugInfo,
  ɵɵadvance,
  ɵɵattribute,
  ɵɵclassProp,
  ɵɵconditional,
  ɵɵconditionalCreate,
  ɵɵdefineComponent,
  ɵɵdefineInjectable,
  ɵɵelement,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵgetCurrentView,
  ɵɵinject,
  ɵɵlistener,
  ɵɵloadQuery,
  ɵɵnextContext,
  ɵɵpipe,
  ɵɵpipeBind1,
  ɵɵpipeBind2,
  ɵɵprojection,
  ɵɵprojectionDef,
  ɵɵproperty,
  ɵɵpureFunction0,
  ɵɵqueryRefresh,
  ɵɵreference,
  ɵɵrepeater,
  ɵɵrepeaterCreate,
  ɵɵrepeaterTrackByIdentity,
  ɵɵresetView,
  ɵɵresolveDocument,
  ɵɵrestoreView,
  ɵɵtext,
  ɵɵtextInterpolate,
  ɵɵtextInterpolate1,
  ɵɵtextInterpolate2,
  ɵɵviewQuery
} from "./chunk-6MVD4A56.js";

// node_modules/date-fns/constants.js
var daysInYear = 365.2425;
var maxTime = Math.pow(10, 8) * 24 * 60 * 60 * 1e3;
var minTime = -maxTime;
var millisecondsInWeek = 6048e5;
var millisecondsInDay = 864e5;
var secondsInHour = 3600;
var secondsInDay = secondsInHour * 24;
var secondsInWeek = secondsInDay * 7;
var secondsInYear = secondsInDay * daysInYear;
var secondsInMonth = secondsInYear / 12;
var secondsInQuarter = secondsInMonth * 3;
var constructFromSymbol = Symbol.for("constructDateFrom");

// node_modules/date-fns/constructFrom.js
function constructFrom(date, value) {
  if (typeof date === "function") return date(value);
  if (date && typeof date === "object" && constructFromSymbol in date)
    return date[constructFromSymbol](value);
  if (date instanceof Date) return new date.constructor(value);
  return new Date(value);
}

// node_modules/date-fns/toDate.js
function toDate(argument, context) {
  return constructFrom(context || argument, argument);
}

// node_modules/date-fns/addDays.js
function addDays(date, amount, options) {
  const _date = toDate(date, options?.in);
  if (isNaN(amount)) return constructFrom(options?.in || date, NaN);
  if (!amount) return _date;
  _date.setDate(_date.getDate() + amount);
  return _date;
}

// node_modules/date-fns/_lib/defaultOptions.js
var defaultOptions = {};
function getDefaultOptions() {
  return defaultOptions;
}

// node_modules/date-fns/startOfWeek.js
function startOfWeek(date, options) {
  const defaultOptions2 = getDefaultOptions();
  const weekStartsOn = options?.weekStartsOn ?? options?.locale?.options?.weekStartsOn ?? defaultOptions2.weekStartsOn ?? defaultOptions2.locale?.options?.weekStartsOn ?? 0;
  const _date = toDate(date, options?.in);
  const day = _date.getDay();
  const diff = (day < weekStartsOn ? 7 : 0) + day - weekStartsOn;
  _date.setDate(_date.getDate() - diff);
  _date.setHours(0, 0, 0, 0);
  return _date;
}

// node_modules/date-fns/startOfISOWeek.js
function startOfISOWeek(date, options) {
  return startOfWeek(date, __spreadProps(__spreadValues({}, options), { weekStartsOn: 1 }));
}

// node_modules/date-fns/getISOWeekYear.js
function getISOWeekYear(date, options) {
  const _date = toDate(date, options?.in);
  const year = _date.getFullYear();
  const fourthOfJanuaryOfNextYear = constructFrom(_date, 0);
  fourthOfJanuaryOfNextYear.setFullYear(year + 1, 0, 4);
  fourthOfJanuaryOfNextYear.setHours(0, 0, 0, 0);
  const startOfNextYear = startOfISOWeek(fourthOfJanuaryOfNextYear);
  const fourthOfJanuaryOfThisYear = constructFrom(_date, 0);
  fourthOfJanuaryOfThisYear.setFullYear(year, 0, 4);
  fourthOfJanuaryOfThisYear.setHours(0, 0, 0, 0);
  const startOfThisYear = startOfISOWeek(fourthOfJanuaryOfThisYear);
  if (_date.getTime() >= startOfNextYear.getTime()) {
    return year + 1;
  } else if (_date.getTime() >= startOfThisYear.getTime()) {
    return year;
  } else {
    return year - 1;
  }
}

// node_modules/date-fns/_lib/getTimezoneOffsetInMilliseconds.js
function getTimezoneOffsetInMilliseconds(date) {
  const _date = toDate(date);
  const utcDate = new Date(
    Date.UTC(
      _date.getFullYear(),
      _date.getMonth(),
      _date.getDate(),
      _date.getHours(),
      _date.getMinutes(),
      _date.getSeconds(),
      _date.getMilliseconds()
    )
  );
  utcDate.setUTCFullYear(_date.getFullYear());
  return +date - +utcDate;
}

// node_modules/date-fns/_lib/normalizeDates.js
function normalizeDates(context, ...dates) {
  const normalize = constructFrom.bind(
    null,
    context || dates.find((date) => typeof date === "object")
  );
  return dates.map(normalize);
}

// node_modules/date-fns/startOfDay.js
function startOfDay(date, options) {
  const _date = toDate(date, options?.in);
  _date.setHours(0, 0, 0, 0);
  return _date;
}

// node_modules/date-fns/differenceInCalendarDays.js
function differenceInCalendarDays(laterDate, earlierDate, options) {
  const [laterDate_, earlierDate_] = normalizeDates(
    options?.in,
    laterDate,
    earlierDate
  );
  const laterStartOfDay = startOfDay(laterDate_);
  const earlierStartOfDay = startOfDay(earlierDate_);
  const laterTimestamp = +laterStartOfDay - getTimezoneOffsetInMilliseconds(laterStartOfDay);
  const earlierTimestamp = +earlierStartOfDay - getTimezoneOffsetInMilliseconds(earlierStartOfDay);
  return Math.round((laterTimestamp - earlierTimestamp) / millisecondsInDay);
}

// node_modules/date-fns/startOfISOWeekYear.js
function startOfISOWeekYear(date, options) {
  const year = getISOWeekYear(date, options);
  const fourthOfJanuary = constructFrom(options?.in || date, 0);
  fourthOfJanuary.setFullYear(year, 0, 4);
  fourthOfJanuary.setHours(0, 0, 0, 0);
  return startOfISOWeek(fourthOfJanuary);
}

// node_modules/date-fns/addWeeks.js
function addWeeks(date, amount, options) {
  return addDays(date, amount * 7, options);
}

// node_modules/date-fns/isDate.js
function isDate(value) {
  return value instanceof Date || typeof value === "object" && Object.prototype.toString.call(value) === "[object Date]";
}

// node_modules/date-fns/isValid.js
function isValid(date) {
  return !(!isDate(date) && typeof date !== "number" || isNaN(+toDate(date)));
}

// node_modules/date-fns/startOfYear.js
function startOfYear(date, options) {
  const date_ = toDate(date, options?.in);
  date_.setFullYear(date_.getFullYear(), 0, 1);
  date_.setHours(0, 0, 0, 0);
  return date_;
}

// node_modules/date-fns/locale/en-US/_lib/formatDistance.js
var formatDistanceLocale = {
  lessThanXSeconds: {
    one: "less than a second",
    other: "less than {{count}} seconds"
  },
  xSeconds: {
    one: "1 second",
    other: "{{count}} seconds"
  },
  halfAMinute: "half a minute",
  lessThanXMinutes: {
    one: "less than a minute",
    other: "less than {{count}} minutes"
  },
  xMinutes: {
    one: "1 minute",
    other: "{{count}} minutes"
  },
  aboutXHours: {
    one: "about 1 hour",
    other: "about {{count}} hours"
  },
  xHours: {
    one: "1 hour",
    other: "{{count}} hours"
  },
  xDays: {
    one: "1 day",
    other: "{{count}} days"
  },
  aboutXWeeks: {
    one: "about 1 week",
    other: "about {{count}} weeks"
  },
  xWeeks: {
    one: "1 week",
    other: "{{count}} weeks"
  },
  aboutXMonths: {
    one: "about 1 month",
    other: "about {{count}} months"
  },
  xMonths: {
    one: "1 month",
    other: "{{count}} months"
  },
  aboutXYears: {
    one: "about 1 year",
    other: "about {{count}} years"
  },
  xYears: {
    one: "1 year",
    other: "{{count}} years"
  },
  overXYears: {
    one: "over 1 year",
    other: "over {{count}} years"
  },
  almostXYears: {
    one: "almost 1 year",
    other: "almost {{count}} years"
  }
};
var formatDistance = (token, count, options) => {
  let result;
  const tokenValue = formatDistanceLocale[token];
  if (typeof tokenValue === "string") {
    result = tokenValue;
  } else if (count === 1) {
    result = tokenValue.one;
  } else {
    result = tokenValue.other.replace("{{count}}", count.toString());
  }
  if (options?.addSuffix) {
    if (options.comparison && options.comparison > 0) {
      return "in " + result;
    } else {
      return result + " ago";
    }
  }
  return result;
};

// node_modules/date-fns/locale/_lib/buildFormatLongFn.js
function buildFormatLongFn(args) {
  return (options = {}) => {
    const width = options.width ? String(options.width) : args.defaultWidth;
    const format2 = args.formats[width] || args.formats[args.defaultWidth];
    return format2;
  };
}

// node_modules/date-fns/locale/en-US/_lib/formatLong.js
var dateFormats = {
  full: "EEEE, MMMM do, y",
  long: "MMMM do, y",
  medium: "MMM d, y",
  short: "MM/dd/yyyy"
};
var timeFormats = {
  full: "h:mm:ss a zzzz",
  long: "h:mm:ss a z",
  medium: "h:mm:ss a",
  short: "h:mm a"
};
var dateTimeFormats = {
  full: "{{date}} 'at' {{time}}",
  long: "{{date}} 'at' {{time}}",
  medium: "{{date}}, {{time}}",
  short: "{{date}}, {{time}}"
};
var formatLong = {
  date: buildFormatLongFn({
    formats: dateFormats,
    defaultWidth: "full"
  }),
  time: buildFormatLongFn({
    formats: timeFormats,
    defaultWidth: "full"
  }),
  dateTime: buildFormatLongFn({
    formats: dateTimeFormats,
    defaultWidth: "full"
  })
};

// node_modules/date-fns/locale/en-US/_lib/formatRelative.js
var formatRelativeLocale = {
  lastWeek: "'last' eeee 'at' p",
  yesterday: "'yesterday at' p",
  today: "'today at' p",
  tomorrow: "'tomorrow at' p",
  nextWeek: "eeee 'at' p",
  other: "P"
};
var formatRelative = (token, _date, _baseDate, _options) => formatRelativeLocale[token];

// node_modules/date-fns/locale/_lib/buildLocalizeFn.js
function buildLocalizeFn(args) {
  return (value, options) => {
    const context = options?.context ? String(options.context) : "standalone";
    let valuesArray;
    if (context === "formatting" && args.formattingValues) {
      const defaultWidth = args.defaultFormattingWidth || args.defaultWidth;
      const width = options?.width ? String(options.width) : defaultWidth;
      valuesArray = args.formattingValues[width] || args.formattingValues[defaultWidth];
    } else {
      const defaultWidth = args.defaultWidth;
      const width = options?.width ? String(options.width) : args.defaultWidth;
      valuesArray = args.values[width] || args.values[defaultWidth];
    }
    const index = args.argumentCallback ? args.argumentCallback(value) : value;
    return valuesArray[index];
  };
}

// node_modules/date-fns/locale/en-US/_lib/localize.js
var eraValues = {
  narrow: ["B", "A"],
  abbreviated: ["BC", "AD"],
  wide: ["Before Christ", "Anno Domini"]
};
var quarterValues = {
  narrow: ["1", "2", "3", "4"],
  abbreviated: ["Q1", "Q2", "Q3", "Q4"],
  wide: ["1st quarter", "2nd quarter", "3rd quarter", "4th quarter"]
};
var monthValues = {
  narrow: ["J", "F", "M", "A", "M", "J", "J", "A", "S", "O", "N", "D"],
  abbreviated: [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec"
  ],
  wide: [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December"
  ]
};
var dayValues = {
  narrow: ["S", "M", "T", "W", "T", "F", "S"],
  short: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
  abbreviated: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
  wide: [
    "Sunday",
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday"
  ]
};
var dayPeriodValues = {
  narrow: {
    am: "a",
    pm: "p",
    midnight: "mi",
    noon: "n",
    morning: "morning",
    afternoon: "afternoon",
    evening: "evening",
    night: "night"
  },
  abbreviated: {
    am: "AM",
    pm: "PM",
    midnight: "midnight",
    noon: "noon",
    morning: "morning",
    afternoon: "afternoon",
    evening: "evening",
    night: "night"
  },
  wide: {
    am: "a.m.",
    pm: "p.m.",
    midnight: "midnight",
    noon: "noon",
    morning: "morning",
    afternoon: "afternoon",
    evening: "evening",
    night: "night"
  }
};
var formattingDayPeriodValues = {
  narrow: {
    am: "a",
    pm: "p",
    midnight: "mi",
    noon: "n",
    morning: "in the morning",
    afternoon: "in the afternoon",
    evening: "in the evening",
    night: "at night"
  },
  abbreviated: {
    am: "AM",
    pm: "PM",
    midnight: "midnight",
    noon: "noon",
    morning: "in the morning",
    afternoon: "in the afternoon",
    evening: "in the evening",
    night: "at night"
  },
  wide: {
    am: "a.m.",
    pm: "p.m.",
    midnight: "midnight",
    noon: "noon",
    morning: "in the morning",
    afternoon: "in the afternoon",
    evening: "in the evening",
    night: "at night"
  }
};
var ordinalNumber = (dirtyNumber, _options) => {
  const number = Number(dirtyNumber);
  const rem100 = number % 100;
  if (rem100 > 20 || rem100 < 10) {
    switch (rem100 % 10) {
      case 1:
        return number + "st";
      case 2:
        return number + "nd";
      case 3:
        return number + "rd";
    }
  }
  return number + "th";
};
var localize = {
  ordinalNumber,
  era: buildLocalizeFn({
    values: eraValues,
    defaultWidth: "wide"
  }),
  quarter: buildLocalizeFn({
    values: quarterValues,
    defaultWidth: "wide",
    argumentCallback: (quarter) => quarter - 1
  }),
  month: buildLocalizeFn({
    values: monthValues,
    defaultWidth: "wide"
  }),
  day: buildLocalizeFn({
    values: dayValues,
    defaultWidth: "wide"
  }),
  dayPeriod: buildLocalizeFn({
    values: dayPeriodValues,
    defaultWidth: "wide",
    formattingValues: formattingDayPeriodValues,
    defaultFormattingWidth: "wide"
  })
};

// node_modules/date-fns/locale/_lib/buildMatchFn.js
function buildMatchFn(args) {
  return (string, options = {}) => {
    const width = options.width;
    const matchPattern = width && args.matchPatterns[width] || args.matchPatterns[args.defaultMatchWidth];
    const matchResult = string.match(matchPattern);
    if (!matchResult) {
      return null;
    }
    const matchedString = matchResult[0];
    const parsePatterns = width && args.parsePatterns[width] || args.parsePatterns[args.defaultParseWidth];
    const key = Array.isArray(parsePatterns) ? findIndex(parsePatterns, (pattern) => pattern.test(matchedString)) : (
      // [TODO] -- I challenge you to fix the type
      findKey(parsePatterns, (pattern) => pattern.test(matchedString))
    );
    let value;
    value = args.valueCallback ? args.valueCallback(key) : key;
    value = options.valueCallback ? (
      // [TODO] -- I challenge you to fix the type
      options.valueCallback(value)
    ) : value;
    const rest = string.slice(matchedString.length);
    return { value, rest };
  };
}
function findKey(object, predicate) {
  for (const key in object) {
    if (Object.prototype.hasOwnProperty.call(object, key) && predicate(object[key])) {
      return key;
    }
  }
  return void 0;
}
function findIndex(array, predicate) {
  for (let key = 0; key < array.length; key++) {
    if (predicate(array[key])) {
      return key;
    }
  }
  return void 0;
}

// node_modules/date-fns/locale/_lib/buildMatchPatternFn.js
function buildMatchPatternFn(args) {
  return (string, options = {}) => {
    const matchResult = string.match(args.matchPattern);
    if (!matchResult) return null;
    const matchedString = matchResult[0];
    const parseResult = string.match(args.parsePattern);
    if (!parseResult) return null;
    let value = args.valueCallback ? args.valueCallback(parseResult[0]) : parseResult[0];
    value = options.valueCallback ? options.valueCallback(value) : value;
    const rest = string.slice(matchedString.length);
    return { value, rest };
  };
}

// node_modules/date-fns/locale/en-US/_lib/match.js
var matchOrdinalNumberPattern = /^(\d+)(th|st|nd|rd)?/i;
var parseOrdinalNumberPattern = /\d+/i;
var matchEraPatterns = {
  narrow: /^(b|a)/i,
  abbreviated: /^(b\.?\s?c\.?|b\.?\s?c\.?\s?e\.?|a\.?\s?d\.?|c\.?\s?e\.?)/i,
  wide: /^(before christ|before common era|anno domini|common era)/i
};
var parseEraPatterns = {
  any: [/^b/i, /^(a|c)/i]
};
var matchQuarterPatterns = {
  narrow: /^[1234]/i,
  abbreviated: /^q[1234]/i,
  wide: /^[1234](th|st|nd|rd)? quarter/i
};
var parseQuarterPatterns = {
  any: [/1/i, /2/i, /3/i, /4/i]
};
var matchMonthPatterns = {
  narrow: /^[jfmasond]/i,
  abbreviated: /^(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)/i,
  wide: /^(january|february|march|april|may|june|july|august|september|october|november|december)/i
};
var parseMonthPatterns = {
  narrow: [
    /^j/i,
    /^f/i,
    /^m/i,
    /^a/i,
    /^m/i,
    /^j/i,
    /^j/i,
    /^a/i,
    /^s/i,
    /^o/i,
    /^n/i,
    /^d/i
  ],
  any: [
    /^ja/i,
    /^f/i,
    /^mar/i,
    /^ap/i,
    /^may/i,
    /^jun/i,
    /^jul/i,
    /^au/i,
    /^s/i,
    /^o/i,
    /^n/i,
    /^d/i
  ]
};
var matchDayPatterns = {
  narrow: /^[smtwf]/i,
  short: /^(su|mo|tu|we|th|fr|sa)/i,
  abbreviated: /^(sun|mon|tue|wed|thu|fri|sat)/i,
  wide: /^(sunday|monday|tuesday|wednesday|thursday|friday|saturday)/i
};
var parseDayPatterns = {
  narrow: [/^s/i, /^m/i, /^t/i, /^w/i, /^t/i, /^f/i, /^s/i],
  any: [/^su/i, /^m/i, /^tu/i, /^w/i, /^th/i, /^f/i, /^sa/i]
};
var matchDayPeriodPatterns = {
  narrow: /^(a|p|mi|n|(in the|at) (morning|afternoon|evening|night))/i,
  any: /^([ap]\.?\s?m\.?|midnight|noon|(in the|at) (morning|afternoon|evening|night))/i
};
var parseDayPeriodPatterns = {
  any: {
    am: /^a/i,
    pm: /^p/i,
    midnight: /^mi/i,
    noon: /^no/i,
    morning: /morning/i,
    afternoon: /afternoon/i,
    evening: /evening/i,
    night: /night/i
  }
};
var match = {
  ordinalNumber: buildMatchPatternFn({
    matchPattern: matchOrdinalNumberPattern,
    parsePattern: parseOrdinalNumberPattern,
    valueCallback: (value) => parseInt(value, 10)
  }),
  era: buildMatchFn({
    matchPatterns: matchEraPatterns,
    defaultMatchWidth: "wide",
    parsePatterns: parseEraPatterns,
    defaultParseWidth: "any"
  }),
  quarter: buildMatchFn({
    matchPatterns: matchQuarterPatterns,
    defaultMatchWidth: "wide",
    parsePatterns: parseQuarterPatterns,
    defaultParseWidth: "any",
    valueCallback: (index) => index + 1
  }),
  month: buildMatchFn({
    matchPatterns: matchMonthPatterns,
    defaultMatchWidth: "wide",
    parsePatterns: parseMonthPatterns,
    defaultParseWidth: "any"
  }),
  day: buildMatchFn({
    matchPatterns: matchDayPatterns,
    defaultMatchWidth: "wide",
    parsePatterns: parseDayPatterns,
    defaultParseWidth: "any"
  }),
  dayPeriod: buildMatchFn({
    matchPatterns: matchDayPeriodPatterns,
    defaultMatchWidth: "any",
    parsePatterns: parseDayPeriodPatterns,
    defaultParseWidth: "any"
  })
};

// node_modules/date-fns/locale/en-US.js
var enUS = {
  code: "en-US",
  formatDistance,
  formatLong,
  formatRelative,
  localize,
  match,
  options: {
    weekStartsOn: 0,
    firstWeekContainsDate: 1
  }
};

// node_modules/date-fns/getDayOfYear.js
function getDayOfYear(date, options) {
  const _date = toDate(date, options?.in);
  const diff = differenceInCalendarDays(_date, startOfYear(_date));
  const dayOfYear = diff + 1;
  return dayOfYear;
}

// node_modules/date-fns/getISOWeek.js
function getISOWeek(date, options) {
  const _date = toDate(date, options?.in);
  const diff = +startOfISOWeek(_date) - +startOfISOWeekYear(_date);
  return Math.round(diff / millisecondsInWeek) + 1;
}

// node_modules/date-fns/getWeekYear.js
function getWeekYear(date, options) {
  const _date = toDate(date, options?.in);
  const year = _date.getFullYear();
  const defaultOptions2 = getDefaultOptions();
  const firstWeekContainsDate = options?.firstWeekContainsDate ?? options?.locale?.options?.firstWeekContainsDate ?? defaultOptions2.firstWeekContainsDate ?? defaultOptions2.locale?.options?.firstWeekContainsDate ?? 1;
  const firstWeekOfNextYear = constructFrom(options?.in || date, 0);
  firstWeekOfNextYear.setFullYear(year + 1, 0, firstWeekContainsDate);
  firstWeekOfNextYear.setHours(0, 0, 0, 0);
  const startOfNextYear = startOfWeek(firstWeekOfNextYear, options);
  const firstWeekOfThisYear = constructFrom(options?.in || date, 0);
  firstWeekOfThisYear.setFullYear(year, 0, firstWeekContainsDate);
  firstWeekOfThisYear.setHours(0, 0, 0, 0);
  const startOfThisYear = startOfWeek(firstWeekOfThisYear, options);
  if (+_date >= +startOfNextYear) {
    return year + 1;
  } else if (+_date >= +startOfThisYear) {
    return year;
  } else {
    return year - 1;
  }
}

// node_modules/date-fns/startOfWeekYear.js
function startOfWeekYear(date, options) {
  const defaultOptions2 = getDefaultOptions();
  const firstWeekContainsDate = options?.firstWeekContainsDate ?? options?.locale?.options?.firstWeekContainsDate ?? defaultOptions2.firstWeekContainsDate ?? defaultOptions2.locale?.options?.firstWeekContainsDate ?? 1;
  const year = getWeekYear(date, options);
  const firstWeek = constructFrom(options?.in || date, 0);
  firstWeek.setFullYear(year, 0, firstWeekContainsDate);
  firstWeek.setHours(0, 0, 0, 0);
  const _date = startOfWeek(firstWeek, options);
  return _date;
}

// node_modules/date-fns/getWeek.js
function getWeek(date, options) {
  const _date = toDate(date, options?.in);
  const diff = +startOfWeek(_date, options) - +startOfWeekYear(_date, options);
  return Math.round(diff / millisecondsInWeek) + 1;
}

// node_modules/date-fns/_lib/addLeadingZeros.js
function addLeadingZeros(number, targetLength) {
  const sign = number < 0 ? "-" : "";
  const output = Math.abs(number).toString().padStart(targetLength, "0");
  return sign + output;
}

// node_modules/date-fns/_lib/format/lightFormatters.js
var lightFormatters = {
  // Year
  y(date, token) {
    const signedYear = date.getFullYear();
    const year = signedYear > 0 ? signedYear : 1 - signedYear;
    return addLeadingZeros(token === "yy" ? year % 100 : year, token.length);
  },
  // Month
  M(date, token) {
    const month = date.getMonth();
    return token === "M" ? String(month + 1) : addLeadingZeros(month + 1, 2);
  },
  // Day of the month
  d(date, token) {
    return addLeadingZeros(date.getDate(), token.length);
  },
  // AM or PM
  a(date, token) {
    const dayPeriodEnumValue = date.getHours() / 12 >= 1 ? "pm" : "am";
    switch (token) {
      case "a":
      case "aa":
        return dayPeriodEnumValue.toUpperCase();
      case "aaa":
        return dayPeriodEnumValue;
      case "aaaaa":
        return dayPeriodEnumValue[0];
      case "aaaa":
      default:
        return dayPeriodEnumValue === "am" ? "a.m." : "p.m.";
    }
  },
  // Hour [1-12]
  h(date, token) {
    return addLeadingZeros(date.getHours() % 12 || 12, token.length);
  },
  // Hour [0-23]
  H(date, token) {
    return addLeadingZeros(date.getHours(), token.length);
  },
  // Minute
  m(date, token) {
    return addLeadingZeros(date.getMinutes(), token.length);
  },
  // Second
  s(date, token) {
    return addLeadingZeros(date.getSeconds(), token.length);
  },
  // Fraction of second
  S(date, token) {
    const numberOfDigits = token.length;
    const milliseconds = date.getMilliseconds();
    const fractionalSeconds = Math.trunc(
      milliseconds * Math.pow(10, numberOfDigits - 3)
    );
    return addLeadingZeros(fractionalSeconds, token.length);
  }
};

// node_modules/date-fns/_lib/format/formatters.js
var dayPeriodEnum = {
  am: "am",
  pm: "pm",
  midnight: "midnight",
  noon: "noon",
  morning: "morning",
  afternoon: "afternoon",
  evening: "evening",
  night: "night"
};
var formatters = {
  // Era
  G: function(date, token, localize2) {
    const era = date.getFullYear() > 0 ? 1 : 0;
    switch (token) {
      // AD, BC
      case "G":
      case "GG":
      case "GGG":
        return localize2.era(era, { width: "abbreviated" });
      // A, B
      case "GGGGG":
        return localize2.era(era, { width: "narrow" });
      // Anno Domini, Before Christ
      case "GGGG":
      default:
        return localize2.era(era, { width: "wide" });
    }
  },
  // Year
  y: function(date, token, localize2) {
    if (token === "yo") {
      const signedYear = date.getFullYear();
      const year = signedYear > 0 ? signedYear : 1 - signedYear;
      return localize2.ordinalNumber(year, { unit: "year" });
    }
    return lightFormatters.y(date, token);
  },
  // Local week-numbering year
  Y: function(date, token, localize2, options) {
    const signedWeekYear = getWeekYear(date, options);
    const weekYear = signedWeekYear > 0 ? signedWeekYear : 1 - signedWeekYear;
    if (token === "YY") {
      const twoDigitYear = weekYear % 100;
      return addLeadingZeros(twoDigitYear, 2);
    }
    if (token === "Yo") {
      return localize2.ordinalNumber(weekYear, { unit: "year" });
    }
    return addLeadingZeros(weekYear, token.length);
  },
  // ISO week-numbering year
  R: function(date, token) {
    const isoWeekYear = getISOWeekYear(date);
    return addLeadingZeros(isoWeekYear, token.length);
  },
  // Extended year. This is a single number designating the year of this calendar system.
  // The main difference between `y` and `u` localizers are B.C. years:
  // | Year | `y` | `u` |
  // |------|-----|-----|
  // | AC 1 |   1 |   1 |
  // | BC 1 |   1 |   0 |
  // | BC 2 |   2 |  -1 |
  // Also `yy` always returns the last two digits of a year,
  // while `uu` pads single digit years to 2 characters and returns other years unchanged.
  u: function(date, token) {
    const year = date.getFullYear();
    return addLeadingZeros(year, token.length);
  },
  // Quarter
  Q: function(date, token, localize2) {
    const quarter = Math.ceil((date.getMonth() + 1) / 3);
    switch (token) {
      // 1, 2, 3, 4
      case "Q":
        return String(quarter);
      // 01, 02, 03, 04
      case "QQ":
        return addLeadingZeros(quarter, 2);
      // 1st, 2nd, 3rd, 4th
      case "Qo":
        return localize2.ordinalNumber(quarter, { unit: "quarter" });
      // Q1, Q2, Q3, Q4
      case "QQQ":
        return localize2.quarter(quarter, {
          width: "abbreviated",
          context: "formatting"
        });
      // 1, 2, 3, 4 (narrow quarter; could be not numerical)
      case "QQQQQ":
        return localize2.quarter(quarter, {
          width: "narrow",
          context: "formatting"
        });
      // 1st quarter, 2nd quarter, ...
      case "QQQQ":
      default:
        return localize2.quarter(quarter, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // Stand-alone quarter
  q: function(date, token, localize2) {
    const quarter = Math.ceil((date.getMonth() + 1) / 3);
    switch (token) {
      // 1, 2, 3, 4
      case "q":
        return String(quarter);
      // 01, 02, 03, 04
      case "qq":
        return addLeadingZeros(quarter, 2);
      // 1st, 2nd, 3rd, 4th
      case "qo":
        return localize2.ordinalNumber(quarter, { unit: "quarter" });
      // Q1, Q2, Q3, Q4
      case "qqq":
        return localize2.quarter(quarter, {
          width: "abbreviated",
          context: "standalone"
        });
      // 1, 2, 3, 4 (narrow quarter; could be not numerical)
      case "qqqqq":
        return localize2.quarter(quarter, {
          width: "narrow",
          context: "standalone"
        });
      // 1st quarter, 2nd quarter, ...
      case "qqqq":
      default:
        return localize2.quarter(quarter, {
          width: "wide",
          context: "standalone"
        });
    }
  },
  // Month
  M: function(date, token, localize2) {
    const month = date.getMonth();
    switch (token) {
      case "M":
      case "MM":
        return lightFormatters.M(date, token);
      // 1st, 2nd, ..., 12th
      case "Mo":
        return localize2.ordinalNumber(month + 1, { unit: "month" });
      // Jan, Feb, ..., Dec
      case "MMM":
        return localize2.month(month, {
          width: "abbreviated",
          context: "formatting"
        });
      // J, F, ..., D
      case "MMMMM":
        return localize2.month(month, {
          width: "narrow",
          context: "formatting"
        });
      // January, February, ..., December
      case "MMMM":
      default:
        return localize2.month(month, { width: "wide", context: "formatting" });
    }
  },
  // Stand-alone month
  L: function(date, token, localize2) {
    const month = date.getMonth();
    switch (token) {
      // 1, 2, ..., 12
      case "L":
        return String(month + 1);
      // 01, 02, ..., 12
      case "LL":
        return addLeadingZeros(month + 1, 2);
      // 1st, 2nd, ..., 12th
      case "Lo":
        return localize2.ordinalNumber(month + 1, { unit: "month" });
      // Jan, Feb, ..., Dec
      case "LLL":
        return localize2.month(month, {
          width: "abbreviated",
          context: "standalone"
        });
      // J, F, ..., D
      case "LLLLL":
        return localize2.month(month, {
          width: "narrow",
          context: "standalone"
        });
      // January, February, ..., December
      case "LLLL":
      default:
        return localize2.month(month, { width: "wide", context: "standalone" });
    }
  },
  // Local week of year
  w: function(date, token, localize2, options) {
    const week = getWeek(date, options);
    if (token === "wo") {
      return localize2.ordinalNumber(week, { unit: "week" });
    }
    return addLeadingZeros(week, token.length);
  },
  // ISO week of year
  I: function(date, token, localize2) {
    const isoWeek = getISOWeek(date);
    if (token === "Io") {
      return localize2.ordinalNumber(isoWeek, { unit: "week" });
    }
    return addLeadingZeros(isoWeek, token.length);
  },
  // Day of the month
  d: function(date, token, localize2) {
    if (token === "do") {
      return localize2.ordinalNumber(date.getDate(), { unit: "date" });
    }
    return lightFormatters.d(date, token);
  },
  // Day of year
  D: function(date, token, localize2) {
    const dayOfYear = getDayOfYear(date);
    if (token === "Do") {
      return localize2.ordinalNumber(dayOfYear, { unit: "dayOfYear" });
    }
    return addLeadingZeros(dayOfYear, token.length);
  },
  // Day of week
  E: function(date, token, localize2) {
    const dayOfWeek = date.getDay();
    switch (token) {
      // Tue
      case "E":
      case "EE":
      case "EEE":
        return localize2.day(dayOfWeek, {
          width: "abbreviated",
          context: "formatting"
        });
      // T
      case "EEEEE":
        return localize2.day(dayOfWeek, {
          width: "narrow",
          context: "formatting"
        });
      // Tu
      case "EEEEEE":
        return localize2.day(dayOfWeek, {
          width: "short",
          context: "formatting"
        });
      // Tuesday
      case "EEEE":
      default:
        return localize2.day(dayOfWeek, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // Local day of week
  e: function(date, token, localize2, options) {
    const dayOfWeek = date.getDay();
    const localDayOfWeek = (dayOfWeek - options.weekStartsOn + 8) % 7 || 7;
    switch (token) {
      // Numerical value (Nth day of week with current locale or weekStartsOn)
      case "e":
        return String(localDayOfWeek);
      // Padded numerical value
      case "ee":
        return addLeadingZeros(localDayOfWeek, 2);
      // 1st, 2nd, ..., 7th
      case "eo":
        return localize2.ordinalNumber(localDayOfWeek, { unit: "day" });
      case "eee":
        return localize2.day(dayOfWeek, {
          width: "abbreviated",
          context: "formatting"
        });
      // T
      case "eeeee":
        return localize2.day(dayOfWeek, {
          width: "narrow",
          context: "formatting"
        });
      // Tu
      case "eeeeee":
        return localize2.day(dayOfWeek, {
          width: "short",
          context: "formatting"
        });
      // Tuesday
      case "eeee":
      default:
        return localize2.day(dayOfWeek, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // Stand-alone local day of week
  c: function(date, token, localize2, options) {
    const dayOfWeek = date.getDay();
    const localDayOfWeek = (dayOfWeek - options.weekStartsOn + 8) % 7 || 7;
    switch (token) {
      // Numerical value (same as in `e`)
      case "c":
        return String(localDayOfWeek);
      // Padded numerical value
      case "cc":
        return addLeadingZeros(localDayOfWeek, token.length);
      // 1st, 2nd, ..., 7th
      case "co":
        return localize2.ordinalNumber(localDayOfWeek, { unit: "day" });
      case "ccc":
        return localize2.day(dayOfWeek, {
          width: "abbreviated",
          context: "standalone"
        });
      // T
      case "ccccc":
        return localize2.day(dayOfWeek, {
          width: "narrow",
          context: "standalone"
        });
      // Tu
      case "cccccc":
        return localize2.day(dayOfWeek, {
          width: "short",
          context: "standalone"
        });
      // Tuesday
      case "cccc":
      default:
        return localize2.day(dayOfWeek, {
          width: "wide",
          context: "standalone"
        });
    }
  },
  // ISO day of week
  i: function(date, token, localize2) {
    const dayOfWeek = date.getDay();
    const isoDayOfWeek = dayOfWeek === 0 ? 7 : dayOfWeek;
    switch (token) {
      // 2
      case "i":
        return String(isoDayOfWeek);
      // 02
      case "ii":
        return addLeadingZeros(isoDayOfWeek, token.length);
      // 2nd
      case "io":
        return localize2.ordinalNumber(isoDayOfWeek, { unit: "day" });
      // Tue
      case "iii":
        return localize2.day(dayOfWeek, {
          width: "abbreviated",
          context: "formatting"
        });
      // T
      case "iiiii":
        return localize2.day(dayOfWeek, {
          width: "narrow",
          context: "formatting"
        });
      // Tu
      case "iiiiii":
        return localize2.day(dayOfWeek, {
          width: "short",
          context: "formatting"
        });
      // Tuesday
      case "iiii":
      default:
        return localize2.day(dayOfWeek, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // AM or PM
  a: function(date, token, localize2) {
    const hours = date.getHours();
    const dayPeriodEnumValue = hours / 12 >= 1 ? "pm" : "am";
    switch (token) {
      case "a":
      case "aa":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "abbreviated",
          context: "formatting"
        });
      case "aaa":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "abbreviated",
          context: "formatting"
        }).toLowerCase();
      case "aaaaa":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "narrow",
          context: "formatting"
        });
      case "aaaa":
      default:
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // AM, PM, midnight, noon
  b: function(date, token, localize2) {
    const hours = date.getHours();
    let dayPeriodEnumValue;
    if (hours === 12) {
      dayPeriodEnumValue = dayPeriodEnum.noon;
    } else if (hours === 0) {
      dayPeriodEnumValue = dayPeriodEnum.midnight;
    } else {
      dayPeriodEnumValue = hours / 12 >= 1 ? "pm" : "am";
    }
    switch (token) {
      case "b":
      case "bb":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "abbreviated",
          context: "formatting"
        });
      case "bbb":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "abbreviated",
          context: "formatting"
        }).toLowerCase();
      case "bbbbb":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "narrow",
          context: "formatting"
        });
      case "bbbb":
      default:
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // in the morning, in the afternoon, in the evening, at night
  B: function(date, token, localize2) {
    const hours = date.getHours();
    let dayPeriodEnumValue;
    if (hours >= 17) {
      dayPeriodEnumValue = dayPeriodEnum.evening;
    } else if (hours >= 12) {
      dayPeriodEnumValue = dayPeriodEnum.afternoon;
    } else if (hours >= 4) {
      dayPeriodEnumValue = dayPeriodEnum.morning;
    } else {
      dayPeriodEnumValue = dayPeriodEnum.night;
    }
    switch (token) {
      case "B":
      case "BB":
      case "BBB":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "abbreviated",
          context: "formatting"
        });
      case "BBBBB":
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "narrow",
          context: "formatting"
        });
      case "BBBB":
      default:
        return localize2.dayPeriod(dayPeriodEnumValue, {
          width: "wide",
          context: "formatting"
        });
    }
  },
  // Hour [1-12]
  h: function(date, token, localize2) {
    if (token === "ho") {
      let hours = date.getHours() % 12;
      if (hours === 0) hours = 12;
      return localize2.ordinalNumber(hours, { unit: "hour" });
    }
    return lightFormatters.h(date, token);
  },
  // Hour [0-23]
  H: function(date, token, localize2) {
    if (token === "Ho") {
      return localize2.ordinalNumber(date.getHours(), { unit: "hour" });
    }
    return lightFormatters.H(date, token);
  },
  // Hour [0-11]
  K: function(date, token, localize2) {
    const hours = date.getHours() % 12;
    if (token === "Ko") {
      return localize2.ordinalNumber(hours, { unit: "hour" });
    }
    return addLeadingZeros(hours, token.length);
  },
  // Hour [1-24]
  k: function(date, token, localize2) {
    let hours = date.getHours();
    if (hours === 0) hours = 24;
    if (token === "ko") {
      return localize2.ordinalNumber(hours, { unit: "hour" });
    }
    return addLeadingZeros(hours, token.length);
  },
  // Minute
  m: function(date, token, localize2) {
    if (token === "mo") {
      return localize2.ordinalNumber(date.getMinutes(), { unit: "minute" });
    }
    return lightFormatters.m(date, token);
  },
  // Second
  s: function(date, token, localize2) {
    if (token === "so") {
      return localize2.ordinalNumber(date.getSeconds(), { unit: "second" });
    }
    return lightFormatters.s(date, token);
  },
  // Fraction of second
  S: function(date, token) {
    return lightFormatters.S(date, token);
  },
  // Timezone (ISO-8601. If offset is 0, output is always `'Z'`)
  X: function(date, token, _localize) {
    const timezoneOffset = date.getTimezoneOffset();
    if (timezoneOffset === 0) {
      return "Z";
    }
    switch (token) {
      // Hours and optional minutes
      case "X":
        return formatTimezoneWithOptionalMinutes(timezoneOffset);
      // Hours, minutes and optional seconds without `:` delimiter
      // Note: neither ISO-8601 nor JavaScript supports seconds in timezone offsets
      // so this token always has the same output as `XX`
      case "XXXX":
      case "XX":
        return formatTimezone(timezoneOffset);
      // Hours, minutes and optional seconds with `:` delimiter
      // Note: neither ISO-8601 nor JavaScript supports seconds in timezone offsets
      // so this token always has the same output as `XXX`
      case "XXXXX":
      case "XXX":
      // Hours and minutes with `:` delimiter
      default:
        return formatTimezone(timezoneOffset, ":");
    }
  },
  // Timezone (ISO-8601. If offset is 0, output is `'+00:00'` or equivalent)
  x: function(date, token, _localize) {
    const timezoneOffset = date.getTimezoneOffset();
    switch (token) {
      // Hours and optional minutes
      case "x":
        return formatTimezoneWithOptionalMinutes(timezoneOffset);
      // Hours, minutes and optional seconds without `:` delimiter
      // Note: neither ISO-8601 nor JavaScript supports seconds in timezone offsets
      // so this token always has the same output as `xx`
      case "xxxx":
      case "xx":
        return formatTimezone(timezoneOffset);
      // Hours, minutes and optional seconds with `:` delimiter
      // Note: neither ISO-8601 nor JavaScript supports seconds in timezone offsets
      // so this token always has the same output as `xxx`
      case "xxxxx":
      case "xxx":
      // Hours and minutes with `:` delimiter
      default:
        return formatTimezone(timezoneOffset, ":");
    }
  },
  // Timezone (GMT)
  O: function(date, token, _localize) {
    const timezoneOffset = date.getTimezoneOffset();
    switch (token) {
      // Short
      case "O":
      case "OO":
      case "OOO":
        return "GMT" + formatTimezoneShort(timezoneOffset, ":");
      // Long
      case "OOOO":
      default:
        return "GMT" + formatTimezone(timezoneOffset, ":");
    }
  },
  // Timezone (specific non-location)
  z: function(date, token, _localize) {
    const timezoneOffset = date.getTimezoneOffset();
    switch (token) {
      // Short
      case "z":
      case "zz":
      case "zzz":
        return "GMT" + formatTimezoneShort(timezoneOffset, ":");
      // Long
      case "zzzz":
      default:
        return "GMT" + formatTimezone(timezoneOffset, ":");
    }
  },
  // Seconds timestamp
  t: function(date, token, _localize) {
    const timestamp = Math.trunc(+date / 1e3);
    return addLeadingZeros(timestamp, token.length);
  },
  // Milliseconds timestamp
  T: function(date, token, _localize) {
    return addLeadingZeros(+date, token.length);
  }
};
function formatTimezoneShort(offset, delimiter = "") {
  const sign = offset > 0 ? "-" : "+";
  const absOffset = Math.abs(offset);
  const hours = Math.trunc(absOffset / 60);
  const minutes = absOffset % 60;
  if (minutes === 0) {
    return sign + String(hours);
  }
  return sign + String(hours) + delimiter + addLeadingZeros(minutes, 2);
}
function formatTimezoneWithOptionalMinutes(offset, delimiter) {
  if (offset % 60 === 0) {
    const sign = offset > 0 ? "-" : "+";
    return sign + addLeadingZeros(Math.abs(offset) / 60, 2);
  }
  return formatTimezone(offset, delimiter);
}
function formatTimezone(offset, delimiter = "") {
  const sign = offset > 0 ? "-" : "+";
  const absOffset = Math.abs(offset);
  const hours = addLeadingZeros(Math.trunc(absOffset / 60), 2);
  const minutes = addLeadingZeros(absOffset % 60, 2);
  return sign + hours + delimiter + minutes;
}

// node_modules/date-fns/_lib/format/longFormatters.js
var dateLongFormatter = (pattern, formatLong2) => {
  switch (pattern) {
    case "P":
      return formatLong2.date({ width: "short" });
    case "PP":
      return formatLong2.date({ width: "medium" });
    case "PPP":
      return formatLong2.date({ width: "long" });
    case "PPPP":
    default:
      return formatLong2.date({ width: "full" });
  }
};
var timeLongFormatter = (pattern, formatLong2) => {
  switch (pattern) {
    case "p":
      return formatLong2.time({ width: "short" });
    case "pp":
      return formatLong2.time({ width: "medium" });
    case "ppp":
      return formatLong2.time({ width: "long" });
    case "pppp":
    default:
      return formatLong2.time({ width: "full" });
  }
};
var dateTimeLongFormatter = (pattern, formatLong2) => {
  const matchResult = pattern.match(/(P+)(p+)?/) || [];
  const datePattern = matchResult[1];
  const timePattern = matchResult[2];
  if (!timePattern) {
    return dateLongFormatter(pattern, formatLong2);
  }
  let dateTimeFormat;
  switch (datePattern) {
    case "P":
      dateTimeFormat = formatLong2.dateTime({ width: "short" });
      break;
    case "PP":
      dateTimeFormat = formatLong2.dateTime({ width: "medium" });
      break;
    case "PPP":
      dateTimeFormat = formatLong2.dateTime({ width: "long" });
      break;
    case "PPPP":
    default:
      dateTimeFormat = formatLong2.dateTime({ width: "full" });
      break;
  }
  return dateTimeFormat.replace("{{date}}", dateLongFormatter(datePattern, formatLong2)).replace("{{time}}", timeLongFormatter(timePattern, formatLong2));
};
var longFormatters = {
  p: timeLongFormatter,
  P: dateTimeLongFormatter
};

// node_modules/date-fns/_lib/protectedTokens.js
var dayOfYearTokenRE = /^D+$/;
var weekYearTokenRE = /^Y+$/;
var throwTokens = ["D", "DD", "YY", "YYYY"];
function isProtectedDayOfYearToken(token) {
  return dayOfYearTokenRE.test(token);
}
function isProtectedWeekYearToken(token) {
  return weekYearTokenRE.test(token);
}
function warnOrThrowProtectedError(token, format2, input) {
  const _message = message(token, format2, input);
  console.warn(_message);
  if (throwTokens.includes(token)) throw new RangeError(_message);
}
function message(token, format2, input) {
  const subject = token[0] === "Y" ? "years" : "days of the month";
  return `Use \`${token.toLowerCase()}\` instead of \`${token}\` (in \`${format2}\`) for formatting ${subject} to the input \`${input}\`; see: https://github.com/date-fns/date-fns/blob/master/docs/unicodeTokens.md`;
}

// node_modules/date-fns/format.js
var formattingTokensRegExp = /[yYQqMLwIdDecihHKkms]o|(\w)\1*|''|'(''|[^'])+('|$)|./g;
var longFormattingTokensRegExp = /P+p+|P+|p+|''|'(''|[^'])+('|$)|./g;
var escapedStringRegExp = /^'([^]*?)'?$/;
var doubleQuoteRegExp = /''/g;
var unescapedLatinCharacterRegExp = /[a-zA-Z]/;
function format(date, formatStr, options) {
  const defaultOptions2 = getDefaultOptions();
  const locale = options?.locale ?? defaultOptions2.locale ?? enUS;
  const firstWeekContainsDate = options?.firstWeekContainsDate ?? options?.locale?.options?.firstWeekContainsDate ?? defaultOptions2.firstWeekContainsDate ?? defaultOptions2.locale?.options?.firstWeekContainsDate ?? 1;
  const weekStartsOn = options?.weekStartsOn ?? options?.locale?.options?.weekStartsOn ?? defaultOptions2.weekStartsOn ?? defaultOptions2.locale?.options?.weekStartsOn ?? 0;
  const originalDate = toDate(date, options?.in);
  if (!isValid(originalDate)) {
    throw new RangeError("Invalid time value");
  }
  let parts = formatStr.match(longFormattingTokensRegExp).map((substring) => {
    const firstCharacter = substring[0];
    if (firstCharacter === "p" || firstCharacter === "P") {
      const longFormatter = longFormatters[firstCharacter];
      return longFormatter(substring, locale.formatLong);
    }
    return substring;
  }).join("").match(formattingTokensRegExp).map((substring) => {
    if (substring === "''") {
      return { isToken: false, value: "'" };
    }
    const firstCharacter = substring[0];
    if (firstCharacter === "'") {
      return { isToken: false, value: cleanEscapedString(substring) };
    }
    if (formatters[firstCharacter]) {
      return { isToken: true, value: substring };
    }
    if (firstCharacter.match(unescapedLatinCharacterRegExp)) {
      throw new RangeError(
        "Format string contains an unescaped latin alphabet character `" + firstCharacter + "`"
      );
    }
    return { isToken: false, value: substring };
  });
  if (locale.localize.preprocessor) {
    parts = locale.localize.preprocessor(originalDate, parts);
  }
  const formatterOptions = {
    firstWeekContainsDate,
    weekStartsOn,
    locale
  };
  return parts.map((part) => {
    if (!part.isToken) return part.value;
    const token = part.value;
    if (!options?.useAdditionalWeekYearTokens && isProtectedWeekYearToken(token) || !options?.useAdditionalDayOfYearTokens && isProtectedDayOfYearToken(token)) {
      warnOrThrowProtectedError(token, formatStr, String(date));
    }
    const formatter = formatters[token[0]];
    return formatter(originalDate, token, locale.localize, formatterOptions);
  }).join("");
}
function cleanEscapedString(input) {
  const matched = input.match(escapedStringRegExp);
  if (!matched) {
    return input;
  }
  return matched[1].replace(doubleQuoteRegExp, "'");
}

// node_modules/date-fns/setHours.js
function setHours(date, hours, options) {
  const _date = toDate(date, options?.in);
  _date.setHours(hours);
  return _date;
}

// node_modules/date-fns/setMinutes.js
function setMinutes(date, minutes, options) {
  const date_ = toDate(date, options?.in);
  date_.setMinutes(minutes);
  return date_;
}

// node_modules/date-fns/subWeeks.js
function subWeeks(date, amount, options) {
  return addWeeks(date, -amount, options);
}

// src/app/operators/withDelayedLoading.ts
function withDelayedLoading(source$, delay = 200) {
  return source$.pipe(distinctUntilChanged(), switchMap((isLoading) => {
    if (!isLoading) {
      return of(false);
    }
    return timer(delay).pipe(map(() => isLoading));
  }));
}

// src/app/components/scheduler/scheduler.ts
var _c0 = ["scrollContainer"];
var _forTrack0 = ($index, $item) => $item.id;
function Scheduler_Conditional_12_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "span", 5);
    \u0275\u0275text(1, "Loading...");
    \u0275\u0275elementEnd();
  }
}
function Scheduler_For_20_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "div", 13);
    \u0275\u0275text(1);
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const time_r2 = ctx.$implicit;
    const ctx_r2 = \u0275\u0275nextContext();
    \u0275\u0275classProp("work-hour", ctx_r2.isWorkHour(time_r2));
    \u0275\u0275advance();
    \u0275\u0275textInterpolate1(" ", time_r2, " ");
  }
}
function Scheduler_For_23_For_7_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "div", 16);
    \u0275\u0275text(1);
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const appointment_r7 = ctx.$implicit;
    const ctx_r2 = \u0275\u0275nextContext(2);
    \u0275\u0275property("ngStyle", ctx_r2.getAppointmentStyle(appointment_r7));
    \u0275\u0275advance();
    \u0275\u0275textInterpolate2(" ", appointment_r7.title, " (", appointment_r7.end, "min) ");
  }
}
function Scheduler_For_23_Template(rf, ctx) {
  if (rf & 1) {
    const _r4 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "div", 12)(1, "div", 14);
    \u0275\u0275text(2);
    \u0275\u0275elementStart(3, "small");
    \u0275\u0275text(4);
    \u0275\u0275elementEnd()();
    \u0275\u0275elementStart(5, "div", 15);
    \u0275\u0275listener("click", function Scheduler_For_23_Template_div_click_5_listener($event) {
      const day_r5 = \u0275\u0275restoreView(_r4).$implicit;
      const ctx_r2 = \u0275\u0275nextContext();
      const scrollContainer_r6 = \u0275\u0275reference(16);
      return \u0275\u0275resetView(ctx_r2.onColumnClick($event, day_r5, scrollContainer_r6));
    });
    \u0275\u0275repeaterCreate(6, Scheduler_For_23_For_7_Template, 2, 3, "div", 16, _forTrack0);
    \u0275\u0275elementEnd()();
  }
  if (rf & 2) {
    const day_r5 = ctx.$implicit;
    const ctx_r2 = \u0275\u0275nextContext();
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate1(" ", ctx_r2.format(day_r5, "EEE"), " ");
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(ctx_r2.format(day_r5, "d"));
    \u0275\u0275advance(2);
    \u0275\u0275repeater(ctx_r2.getAppointmentsForDay(day_r5));
  }
}
var Scheduler = class _Scheduler {
  scrollContainer;
  appointments = [];
  loadingSubject = new BehaviorSubject(false);
  destroy$ = new Subject();
  set loading(value) {
    this.loadingSubject.next(value);
  }
  showLoading$ = withDelayedLoading(this.loadingSubject);
  appointmentCreate = new EventEmitter();
  appointmentUpdate = new EventEmitter();
  loadAppointments = new EventEmitter();
  currentWeek = /* @__PURE__ */ new Date();
  weekDays = [];
  timeSlots = [];
  ngOnInit() {
    this.generateTimeSlots();
    this.updateWeekDays();
  }
  workHourStart = 8;
  workHourEnd = 19;
  pixelsPerHour = 60;
  // Increased from 30 to 60 for better precision
  generateTimeSlots() {
    this.timeSlots = Array.from({ length: 24 }, (_, i) => format((/* @__PURE__ */ new Date()).setHours(i, 0), "HH:mm"));
  }
  updateWeekDays() {
    const start = startOfWeek(this.currentWeek, { weekStartsOn: 1 });
    this.loadAppointments.emit({ start, end: addWeeks(start, 1) });
    this.weekDays = Array.from({ length: 7 }, (_, i) => addDays(start, i));
  }
  previousWeek() {
    this.currentWeek = subWeeks(this.currentWeek, 1);
    this.updateWeekDays();
  }
  nextWeek() {
    this.currentWeek = addWeeks(this.currentWeek, 1);
    this.updateWeekDays();
  }
  getAppointmentStyle(appointment) {
    const startHour = appointment.start.getHours();
    const startMinute = appointment.start.getMinutes();
    const top = (startHour * 60 + startMinute) * (this.pixelsPerHour / 60);
    const duration = appointment.end.getHours() * 60 + appointment.end.getMinutes() - (appointment.start.getHours() * 60 + appointment.start.getMinutes());
    return {
      top: `${top}px`,
      height: `${duration * (this.pixelsPerHour / 60)}px`
    };
  }
  getAppointmentsForDay(day) {
    return this.appointments.filter((apt) => format(apt.start, "yyyy-MM-dd") === format(day, "yyyy-MM-dd"));
  }
  isWorkHour(time) {
    const hour = parseInt(time.split(":")[0], 10);
    return hour >= this.workHourStart && hour <= this.workHourEnd;
  }
  getWeekStart() {
    return startOfWeek(this.currentWeek, { weekStartsOn: 1 });
  }
  getWeekEnd() {
    return addDays(this.getWeekStart(), 6);
  }
  format = format;
  ngAfterViewInit() {
    const scrollOffset = this.workHourStart * this.pixelsPerHour;
    this.scrollContainer.nativeElement.scrollTop = scrollOffset;
  }
  onColumnClick(event, day, container) {
    const eventElement = event.target.closest(".event");
    if (eventElement) {
      const appointments = this.getAppointmentsForDay(day);
      const rect2 = eventElement.getBoundingClientRect();
      const existingAppointment = appointments.find((apt) => {
        const eventTop = rect2.top;
        const eventBottom = rect2.bottom;
        const clickY = event.clientY;
        return clickY >= eventTop && clickY <= eventBottom;
      });
      if (existingAppointment) {
        this.appointmentUpdate.emit({
          id: existingAppointment.id,
          start: existingAppointment.start,
          end: existingAppointment.end
        });
        return;
      }
    }
    const rect = container.getBoundingClientRect();
    const y = event.clientY - rect.top + container.scrollTop;
    const totalMinutes = Math.floor(y * 60 / this.pixelsPerHour);
    const hours = Math.floor(totalMinutes / 60);
    const minutes = Math.round(totalMinutes % 60 / 15) * 15;
    const startDate = setMinutes(setHours(day, hours - 1), minutes);
    const endDate = setMinutes(setHours(day, hours), minutes);
    this.appointmentCreate.emit({ start: startDate, end: endDate });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  static \u0275fac = function Scheduler_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Scheduler)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Scheduler, selectors: [["app-scheduler"]], viewQuery: function Scheduler_Query(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275viewQuery(_c0, 5);
    }
    if (rf & 2) {
      let _t;
      \u0275\u0275queryRefresh(_t = \u0275\u0275loadQuery()) && (ctx.scrollContainer = _t.first);
    }
  }, inputs: { appointments: "appointments", loading: "loading" }, outputs: { appointmentCreate: "appointmentCreate", appointmentUpdate: "appointmentUpdate", loadAppointments: "loadAppointments" }, decls: 24, vars: 11, consts: [["scrollContainer", ""], [1, "scheduler"], [1, "controls-container"], [1, "controls"], [1, "outline", 3, "click"], ["aria-busy", "true"], [1, "calendar-container"], [1, "scroll-container"], [1, "time-column"], [1, "time-header"], [1, "time-label", 3, "work-hour"], [1, "day-columns"], [1, "day-column"], [1, "time-label"], [1, "day-header"], [1, "events-container", 3, "click"], [1, "event", 3, "ngStyle"]], template: function Scheduler_Template(rf, ctx) {
    if (rf & 1) {
      const _r1 = \u0275\u0275getCurrentView();
      \u0275\u0275elementStart(0, "article", 1)(1, "header", 2)(2, "div", 3)(3, "button", 4);
      \u0275\u0275listener("click", function Scheduler_Template_button_click_3_listener() {
        \u0275\u0275restoreView(_r1);
        return \u0275\u0275resetView(ctx.previousWeek());
      });
      \u0275\u0275text(4, "Previous");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(5, "h3");
      \u0275\u0275text(6);
      \u0275\u0275pipe(7, "date");
      \u0275\u0275pipe(8, "date");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(9, "button", 4);
      \u0275\u0275listener("click", function Scheduler_Template_button_click_9_listener() {
        \u0275\u0275restoreView(_r1);
        return \u0275\u0275resetView(ctx.nextWeek());
      });
      \u0275\u0275text(10, "Next");
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(11, "div");
      \u0275\u0275conditionalCreate(12, Scheduler_Conditional_12_Template, 2, 0, "span", 5);
      \u0275\u0275pipe(13, "async");
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(14, "div", 6)(15, "div", 7, 0)(17, "div", 8);
      \u0275\u0275element(18, "div", 9);
      \u0275\u0275repeaterCreate(19, Scheduler_For_20_Template, 2, 3, "div", 10, \u0275\u0275repeaterTrackByIdentity);
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(21, "div", 11);
      \u0275\u0275repeaterCreate(22, Scheduler_For_23_Template, 8, 2, "div", 12, \u0275\u0275repeaterTrackByIdentity);
      \u0275\u0275elementEnd()()()();
    }
    if (rf & 2) {
      \u0275\u0275advance(6);
      \u0275\u0275textInterpolate2("", \u0275\u0275pipeBind2(7, 3, ctx.getWeekStart(), "mediumDate"), " - ", \u0275\u0275pipeBind2(8, 6, ctx.getWeekEnd(), "mediumDate"));
      \u0275\u0275advance(6);
      \u0275\u0275conditional(\u0275\u0275pipeBind1(13, 9, ctx.showLoading$) ? 12 : -1);
      \u0275\u0275advance(7);
      \u0275\u0275repeater(ctx.timeSlots);
      \u0275\u0275advance(3);
      \u0275\u0275repeater(ctx.weekDays);
    }
  }, dependencies: [CommonModule, NgStyle, AsyncPipe, DatePipe], styles: ["\n\n.scheduler[_ngcontent-%COMP%] {\n  margin: 1rem;\n  padding: 1rem;\n}\n.scheduler[_ngcontent-%COMP%]   .controls[_ngcontent-%COMP%] {\n  display: flex;\n  align-items: center;\n  gap: 1rem;\n  margin-bottom: 1rem;\n}\n.scheduler[_ngcontent-%COMP%]   .controls[_ngcontent-%COMP%]   h3[_ngcontent-%COMP%] {\n  margin: 0;\n}\n.scheduler[_ngcontent-%COMP%]   .controls-container[_ngcontent-%COMP%] {\n  display: flex;\n  justify-content: space-between;\n  align-items: center;\n  color: var(--pico-muted-color);\n  gap: 1rem;\n}\n.scheduler[_ngcontent-%COMP%]   .calendar-container[_ngcontent-%COMP%] {\n  border: 1px solid var(--pico-border-color);\n  border-radius: var(--pico-border-radius);\n  overflow: hidden;\n  height: 660px;\n}\n.scheduler[_ngcontent-%COMP%]   .scroll-container[_ngcontent-%COMP%] {\n  display: flex;\n  height: 100%;\n  overflow-y: auto;\n  background: var(--pico-background-color);\n}\n.scheduler[_ngcontent-%COMP%]   .scroll-container[_ngcontent-%COMP%]::-webkit-scrollbar {\n  width: 8px;\n}\n.scheduler[_ngcontent-%COMP%]   .scroll-container[_ngcontent-%COMP%]::-webkit-scrollbar-track {\n  background: var(--pico-background-color);\n}\n.scheduler[_ngcontent-%COMP%]   .scroll-container[_ngcontent-%COMP%]::-webkit-scrollbar-thumb {\n  background: var(--pico-muted-border-color);\n  border-radius: 4px;\n}\n.scheduler[_ngcontent-%COMP%]   .time-column[_ngcontent-%COMP%] {\n  width: 60px;\n  border-right: 1px solid var(--pico-border-color);\n  flex-shrink: 0;\n}\n.scheduler[_ngcontent-%COMP%]   .time-column[_ngcontent-%COMP%]   .time-header[_ngcontent-%COMP%] {\n  height: 40px;\n  border-bottom: 1px solid var(--pico-border-color);\n  position: sticky;\n  top: 0;\n  z-index: 2;\n  background: var(--pico-background-color);\n}\n.scheduler[_ngcontent-%COMP%]   .time-column[_ngcontent-%COMP%]   .time-label[_ngcontent-%COMP%] {\n  height: 60px;\n  line-height: 60px;\n  text-align: right;\n  padding-right: 0.5rem;\n  font-size: 0.75rem;\n  color: var(--pico-muted-color);\n}\n.scheduler[_ngcontent-%COMP%]   .time-column[_ngcontent-%COMP%]   .time-label.work-hour[_ngcontent-%COMP%] {\n  font-weight: 500;\n  color: var(--pico-color);\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%] {\n  display: flex;\n  flex: 1;\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%]   .day-column[_ngcontent-%COMP%] {\n  flex: 1;\n  border-left: 1px solid var(--pico-border-color);\n  min-height: 1440px;\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%]   .day-column[_ngcontent-%COMP%]:first-child {\n  border-left: none;\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%]   .day-column[_ngcontent-%COMP%]   .day-header[_ngcontent-%COMP%] {\n  height: 40px;\n  display: flex;\n  flex-direction: column;\n  align-items: center;\n  justify-content: center;\n  background: var(--pico-background-color);\n  border-bottom: 1px solid var(--pico-border-color);\n  font-size: 0.9rem;\n  position: sticky;\n  top: 0;\n  z-index: 1;\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%]   .day-column[_ngcontent-%COMP%]   .day-header[_ngcontent-%COMP%]   small[_ngcontent-%COMP%] {\n  color: var(--pico-muted-color);\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%]   .day-column[_ngcontent-%COMP%]   .events-container[_ngcontent-%COMP%] {\n  position: relative;\n  height: 1440px;\n  background-image:\n    repeating-linear-gradient(\n      to bottom,\n      transparent,\n      transparent 59px,\n      var(--pico-stripe-color) 59px,\n      var(--pico-stripe-color) 60px);\n}\n.scheduler[_ngcontent-%COMP%]   .day-columns[_ngcontent-%COMP%]   .day-column[_ngcontent-%COMP%]   .event[_ngcontent-%COMP%] {\n  position: absolute;\n  left: 4px;\n  right: 4px;\n  background: var(--pico-primary);\n  color: var(--pico-primary-inverse);\n  padding: 0.25rem;\n  font-size: 0.75rem;\n  border-radius: var(--pico-border-radius);\n  overflow: hidden;\n  white-space: nowrap;\n  text-overflow: ellipsis;\n  z-index: 2;\n}\n/*# sourceMappingURL=scheduler.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Scheduler, [{
    type: Component,
    args: [{ selector: "app-scheduler", imports: [CommonModule], standalone: true, template: `<article class="scheduler">
  <header class="controls-container">
    <div class="controls">
      <button class="outline" (click)="previousWeek()">Previous</button>
      <h3>{{ getWeekStart() | date: 'mediumDate' }} - {{ getWeekEnd() | date: 'mediumDate' }}</h3>
      <button class="outline" (click)="nextWeek()">Next</button>
    </div>
    <div>
      @if (showLoading$ | async) {
        <span aria-busy="true">Loading...</span>
      }
    </div>
  </header>

  <div class="calendar-container">
    <div class="scroll-container" #scrollContainer>
      <div class="time-column">
        <div class="time-header"></div>
        @for (time of timeSlots; track time) {
          <div class="time-label" [class.work-hour]="isWorkHour(time)">
            {{ time }}
          </div>
        }
      </div>

      <div class="day-columns">
        @for (day of weekDays; track day) {
          <div class="day-column">
            <div class="day-header">
              {{ format(day, 'EEE') }}
              <small>{{ format(day, 'd') }}</small>
            </div>
            <div class="events-container" (click)="onColumnClick($event, day, scrollContainer)">
              @for (appointment of getAppointmentsForDay(day); track appointment.id) {
                <div class="event" [ngStyle]="getAppointmentStyle(appointment)">
                  {{ appointment.title }} ({{ appointment.end }}min)
                </div>
              }
            </div>
          </div>
        }
      </div>
    </div>
  </div>
</article>
`, styles: ["/* src/app/components/scheduler/scheduler.scss */\n.scheduler {\n  margin: 1rem;\n  padding: 1rem;\n}\n.scheduler .controls {\n  display: flex;\n  align-items: center;\n  gap: 1rem;\n  margin-bottom: 1rem;\n}\n.scheduler .controls h3 {\n  margin: 0;\n}\n.scheduler .controls-container {\n  display: flex;\n  justify-content: space-between;\n  align-items: center;\n  color: var(--pico-muted-color);\n  gap: 1rem;\n}\n.scheduler .calendar-container {\n  border: 1px solid var(--pico-border-color);\n  border-radius: var(--pico-border-radius);\n  overflow: hidden;\n  height: 660px;\n}\n.scheduler .scroll-container {\n  display: flex;\n  height: 100%;\n  overflow-y: auto;\n  background: var(--pico-background-color);\n}\n.scheduler .scroll-container::-webkit-scrollbar {\n  width: 8px;\n}\n.scheduler .scroll-container::-webkit-scrollbar-track {\n  background: var(--pico-background-color);\n}\n.scheduler .scroll-container::-webkit-scrollbar-thumb {\n  background: var(--pico-muted-border-color);\n  border-radius: 4px;\n}\n.scheduler .time-column {\n  width: 60px;\n  border-right: 1px solid var(--pico-border-color);\n  flex-shrink: 0;\n}\n.scheduler .time-column .time-header {\n  height: 40px;\n  border-bottom: 1px solid var(--pico-border-color);\n  position: sticky;\n  top: 0;\n  z-index: 2;\n  background: var(--pico-background-color);\n}\n.scheduler .time-column .time-label {\n  height: 60px;\n  line-height: 60px;\n  text-align: right;\n  padding-right: 0.5rem;\n  font-size: 0.75rem;\n  color: var(--pico-muted-color);\n}\n.scheduler .time-column .time-label.work-hour {\n  font-weight: 500;\n  color: var(--pico-color);\n}\n.scheduler .day-columns {\n  display: flex;\n  flex: 1;\n}\n.scheduler .day-columns .day-column {\n  flex: 1;\n  border-left: 1px solid var(--pico-border-color);\n  min-height: 1440px;\n}\n.scheduler .day-columns .day-column:first-child {\n  border-left: none;\n}\n.scheduler .day-columns .day-column .day-header {\n  height: 40px;\n  display: flex;\n  flex-direction: column;\n  align-items: center;\n  justify-content: center;\n  background: var(--pico-background-color);\n  border-bottom: 1px solid var(--pico-border-color);\n  font-size: 0.9rem;\n  position: sticky;\n  top: 0;\n  z-index: 1;\n}\n.scheduler .day-columns .day-column .day-header small {\n  color: var(--pico-muted-color);\n}\n.scheduler .day-columns .day-column .events-container {\n  position: relative;\n  height: 1440px;\n  background-image:\n    repeating-linear-gradient(\n      to bottom,\n      transparent,\n      transparent 59px,\n      var(--pico-stripe-color) 59px,\n      var(--pico-stripe-color) 60px);\n}\n.scheduler .day-columns .day-column .event {\n  position: absolute;\n  left: 4px;\n  right: 4px;\n  background: var(--pico-primary);\n  color: var(--pico-primary-inverse);\n  padding: 0.25rem;\n  font-size: 0.75rem;\n  border-radius: var(--pico-border-radius);\n  overflow: hidden;\n  white-space: nowrap;\n  text-overflow: ellipsis;\n  z-index: 2;\n}\n/*# sourceMappingURL=scheduler.css.map */\n"] }]
  }], null, { scrollContainer: [{
    type: ViewChild,
    args: ["scrollContainer"]
  }], appointments: [{
    type: Input
  }], loading: [{
    type: Input
  }], appointmentCreate: [{
    type: Output
  }], appointmentUpdate: [{
    type: Output
  }], loadAppointments: [{
    type: Output
  }] });
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Scheduler, { className: "Scheduler", filePath: "src/app/components/scheduler/scheduler.ts", lineNumber: 31 });
})();

// node_modules/@angular/animations/fesm2022/animations.mjs
var AnimationBuilder = class _AnimationBuilder {
  static \u0275fac = function AnimationBuilder_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _AnimationBuilder)();
  };
  static \u0275prov = /* @__PURE__ */ \u0275\u0275defineInjectable({
    token: _AnimationBuilder,
    factory: () => (() => inject(BrowserAnimationBuilder))(),
    providedIn: "root"
  });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(AnimationBuilder, [{
    type: Injectable,
    args: [{
      providedIn: "root",
      useFactory: () => inject(BrowserAnimationBuilder)
    }]
  }], null, null);
})();
var AnimationFactory = class {
};
var BrowserAnimationBuilder = class _BrowserAnimationBuilder extends AnimationBuilder {
  animationModuleType = inject(ANIMATION_MODULE_TYPE, {
    optional: true
  });
  _nextAnimationId = 0;
  _renderer;
  constructor(rootRenderer, doc) {
    super();
    const typeData = {
      id: "0",
      encapsulation: ViewEncapsulation.None,
      styles: [],
      data: {
        animation: []
      }
    };
    this._renderer = rootRenderer.createRenderer(doc.body, typeData);
    if (this.animationModuleType === null && !isAnimationRenderer(this._renderer)) {
      throw new RuntimeError(3600, (typeof ngDevMode === "undefined" || ngDevMode) && "Angular detected that the `AnimationBuilder` was injected, but animation support was not enabled. Please make sure that you enable animations in your application by calling `provideAnimations()` or `provideAnimationsAsync()` function.");
    }
  }
  build(animation2) {
    const id = this._nextAnimationId;
    this._nextAnimationId++;
    const entry = Array.isArray(animation2) ? sequence(animation2) : animation2;
    issueAnimationCommand(this._renderer, null, id, "register", [entry]);
    return new BrowserAnimationFactory(id, this._renderer);
  }
  static \u0275fac = function BrowserAnimationBuilder_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _BrowserAnimationBuilder)(\u0275\u0275inject(RendererFactory2), \u0275\u0275inject(DOCUMENT));
  };
  static \u0275prov = /* @__PURE__ */ \u0275\u0275defineInjectable({
    token: _BrowserAnimationBuilder,
    factory: _BrowserAnimationBuilder.\u0275fac,
    providedIn: "root"
  });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(BrowserAnimationBuilder, [{
    type: Injectable,
    args: [{
      providedIn: "root"
    }]
  }], () => [{
    type: RendererFactory2
  }, {
    type: Document,
    decorators: [{
      type: Inject,
      args: [DOCUMENT]
    }]
  }], null);
})();
var BrowserAnimationFactory = class extends AnimationFactory {
  _id;
  _renderer;
  constructor(_id, _renderer) {
    super();
    this._id = _id;
    this._renderer = _renderer;
  }
  create(element, options) {
    return new RendererAnimationPlayer(this._id, element, options || {}, this._renderer);
  }
};
var RendererAnimationPlayer = class {
  id;
  element;
  _renderer;
  parentPlayer = null;
  _started = false;
  constructor(id, element, options, _renderer) {
    this.id = id;
    this.element = element;
    this._renderer = _renderer;
    this._command("create", options);
  }
  _listen(eventName, callback) {
    return this._renderer.listen(this.element, `@@${this.id}:${eventName}`, callback);
  }
  _command(command, ...args) {
    issueAnimationCommand(this._renderer, this.element, this.id, command, args);
  }
  onDone(fn) {
    this._listen("done", fn);
  }
  onStart(fn) {
    this._listen("start", fn);
  }
  onDestroy(fn) {
    this._listen("destroy", fn);
  }
  init() {
    this._command("init");
  }
  hasStarted() {
    return this._started;
  }
  play() {
    this._command("play");
    this._started = true;
  }
  pause() {
    this._command("pause");
  }
  restart() {
    this._command("restart");
  }
  finish() {
    this._command("finish");
  }
  destroy() {
    this._command("destroy");
  }
  reset() {
    this._command("reset");
    this._started = false;
  }
  setPosition(p) {
    this._command("setPosition", p);
  }
  getPosition() {
    return unwrapAnimationRenderer(this._renderer)?.engine?.players[this.id]?.getPosition() ?? 0;
  }
  totalTime = 0;
};
function issueAnimationCommand(renderer, element, id, command, args) {
  renderer.setProperty(element, `@@${id}:${command}`, args);
}
function unwrapAnimationRenderer(renderer) {
  const type = renderer.\u0275type;
  if (type === 0) {
    return renderer;
  } else if (type === 1) {
    return renderer.animationRenderer;
  }
  return null;
}
function isAnimationRenderer(renderer) {
  const type = renderer.\u0275type;
  return type === 0 || type === 1;
}

// src/app/components/modal/modal.ts
var _c02 = [[["modal-header"]], [["modal-content"]], [["modal-footer"]]];
var _c1 = ["modal-header", "modal-content", "modal-footer"];
function Modal_Conditional_0_Template(rf, ctx) {
  if (rf & 1) {
    const _r1 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "dialog", 1);
    \u0275\u0275listener("click", function Modal_Conditional_0_Template_dialog_click_0_listener($event) {
      \u0275\u0275restoreView(_r1);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.handleOverlayClick($event));
    });
    \u0275\u0275elementStart(1, "article")(2, "header")(3, "button", 2);
    \u0275\u0275listener("click", function Modal_Conditional_0_Template_button_click_3_listener() {
      \u0275\u0275restoreView(_r1);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.handleClose());
    });
    \u0275\u0275elementEnd();
    \u0275\u0275projection(4);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(5, "div", 3);
    \u0275\u0275projection(6, 1);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(7, "footer");
    \u0275\u0275projection(8, 2);
    \u0275\u0275elementEnd()()();
  }
  if (rf & 2) {
    \u0275\u0275property("open", true)("@modalAnimation", void 0);
    \u0275\u0275advance();
    \u0275\u0275property("@modalContentAnimation", void 0);
  }
}
var _c2 = ["*"];
var Modal = class _Modal {
  isOpen = false;
  close = new EventEmitter();
  handleOverlayClick(event) {
    const target = event.target;
    if (target.tagName.toLowerCase() === "dialog") {
      this.handleClose();
    }
  }
  handleClose() {
    this.close.emit();
  }
  documentKeydown(event) {
    if (event.key === "Escape") {
      this.handleClose();
    }
  }
  static \u0275fac = function Modal_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Modal)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Modal, selectors: [["app-modal"]], hostBindings: function Modal_HostBindings(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275listener("keydown", function Modal_keydown_HostBindingHandler($event) {
        return ctx.documentKeydown($event);
      }, \u0275\u0275resolveDocument);
    }
  }, inputs: { isOpen: "isOpen" }, outputs: { close: "close" }, ngContentSelectors: _c1, decls: 1, vars: 1, consts: [[1, "modal-overlay", 3, "open"], [1, "modal-overlay", 3, "click", "open"], ["aria-label", "Close", "rel", "prev", 3, "click"], [1, "content"]], template: function Modal_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275projectionDef(_c02);
      \u0275\u0275conditionalCreate(0, Modal_Conditional_0_Template, 9, 3, "dialog", 0);
    }
    if (rf & 2) {
      \u0275\u0275conditional(ctx.isOpen ? 0 : -1);
    }
  }, dependencies: [CommonModule], styles: ["\n\n[_nghost-%COMP%]   dialog[_ngcontent-%COMP%] {\n  padding: 0;\n  border: none;\n  background: transparent;\n}\n[_nghost-%COMP%]   dialog[open][_ngcontent-%COMP%] {\n  opacity: 1;\n  visibility: visible;\n}\n[_nghost-%COMP%]   dialog[_ngcontent-%COMP%]::backdrop {\n  background: rgba(0, 0, 0, 0.5);\n}\n[_nghost-%COMP%]   dialog[_ngcontent-%COMP%]   article[_ngcontent-%COMP%] {\n  max-height: 90vh;\n  display: flex;\n  flex-direction: column;\n  margin-top: 0;\n  padding-top: 0;\n  padding-bottom: 0;\n  margin-bottom: 0;\n}\n[_nghost-%COMP%]   dialog[_ngcontent-%COMP%]   article[_ngcontent-%COMP%]   header[_ngcontent-%COMP%] {\n  position: sticky;\n  top: 0;\n  padding-top: 1rem;\n  padding-bottom: 1rem;\n}\n[_nghost-%COMP%]   dialog[_ngcontent-%COMP%]   article[_ngcontent-%COMP%]   content[_ngcontent-%COMP%] {\n  flex: 1;\n  overflow-y: auto;\n  min-height: 0;\n}\n[_nghost-%COMP%]   dialog[_ngcontent-%COMP%]   article[_ngcontent-%COMP%]   footer[_ngcontent-%COMP%] {\n  position: sticky;\n  bottom: 0;\n  padding-top: 1rem;\n}\n/*# sourceMappingURL=modal.css.map */"], data: { animation: [
    trigger("modalAnimation", [
      transition(":enter", [style({ opacity: 0 }), animate("150ms ease-out", style({ opacity: 1 }))]),
      transition(":leave", [animate("150ms ease-in", style({ opacity: 0 }))])
    ]),
    trigger("modalContentAnimation", [
      transition(":enter", [
        style({ opacity: 0, transform: "scale(0.95)" }),
        animate("150ms ease-out", style({ opacity: 1, transform: "scale(1)" }))
      ]),
      transition(":leave", [animate("150ms ease-in", style({ opacity: 0, transform: "scale(0.95)" }))])
    ])
  ] } });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Modal, [{
    type: Component,
    args: [{ selector: "app-modal", standalone: true, imports: [CommonModule], animations: [
      trigger("modalAnimation", [
        transition(":enter", [style({ opacity: 0 }), animate("150ms ease-out", style({ opacity: 1 }))]),
        transition(":leave", [animate("150ms ease-in", style({ opacity: 0 }))])
      ]),
      trigger("modalContentAnimation", [
        transition(":enter", [
          style({ opacity: 0, transform: "scale(0.95)" }),
          animate("150ms ease-out", style({ opacity: 1, transform: "scale(1)" }))
        ]),
        transition(":leave", [animate("150ms ease-in", style({ opacity: 0, transform: "scale(0.95)" }))])
      ])
    ], template: '@if (isOpen) {\n  <dialog class="modal-overlay" [open]="true" (click)="handleOverlayClick($event)" [@modalAnimation]>\n    <article [@modalContentAnimation]>\n      <header>\n        <button aria-label="Close" rel="prev" (click)="handleClose()"></button>\n        <ng-content select="modal-header"></ng-content>\n      </header>\n      <div class="content">\n        <ng-content select="modal-content"></ng-content>\n      </div>\n      <footer>\n        <ng-content select="modal-footer"></ng-content>\n      </footer>\n    </article>\n  </dialog>\n}\n', styles: ["/* src/app/components/modal/modal.scss */\n:host dialog {\n  padding: 0;\n  border: none;\n  background: transparent;\n}\n:host dialog[open] {\n  opacity: 1;\n  visibility: visible;\n}\n:host dialog::backdrop {\n  background: rgba(0, 0, 0, 0.5);\n}\n:host dialog article {\n  max-height: 90vh;\n  display: flex;\n  flex-direction: column;\n  margin-top: 0;\n  padding-top: 0;\n  padding-bottom: 0;\n  margin-bottom: 0;\n}\n:host dialog article header {\n  position: sticky;\n  top: 0;\n  padding-top: 1rem;\n  padding-bottom: 1rem;\n}\n:host dialog article content {\n  flex: 1;\n  overflow-y: auto;\n  min-height: 0;\n}\n:host dialog article footer {\n  position: sticky;\n  bottom: 0;\n  padding-top: 1rem;\n}\n/*# sourceMappingURL=modal.css.map */\n"] }]
  }], null, { isOpen: [{
    type: Input
  }], close: [{
    type: Output
  }], documentKeydown: [{
    type: HostListener,
    args: ["document:keydown", ["$event"]]
  }] });
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Modal, { className: "Modal", filePath: "src/app/components/modal/modal.ts", lineNumber: 25 });
})();
var ModalHeader = class _ModalHeader {
  static \u0275fac = function ModalHeader_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _ModalHeader)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _ModalHeader, selectors: [["modal-header"]], ngContentSelectors: _c2, decls: 1, vars: 0, template: function ModalHeader_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275projectionDef();
      \u0275\u0275projection(0);
    }
  }, encapsulation: 2 });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(ModalHeader, [{
    type: Component,
    args: [{
      selector: "modal-header",
      standalone: true,
      template: `<ng-content></ng-content>`
    }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(ModalHeader, { className: "ModalHeader", filePath: "src/app/components/modal/modal.ts", lineNumber: 54 });
})();
var ModalContent = class _ModalContent {
  static \u0275fac = function ModalContent_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _ModalContent)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _ModalContent, selectors: [["modal-content"]], ngContentSelectors: _c2, decls: 1, vars: 0, template: function ModalContent_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275projectionDef();
      \u0275\u0275projection(0);
    }
  }, encapsulation: 2 });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(ModalContent, [{
    type: Component,
    args: [{
      selector: "modal-content",
      standalone: true,
      template: `<ng-content></ng-content>`
    }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(ModalContent, { className: "ModalContent", filePath: "src/app/components/modal/modal.ts", lineNumber: 61 });
})();
var ModalFooter = class _ModalFooter {
  static \u0275fac = function ModalFooter_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _ModalFooter)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _ModalFooter, selectors: [["modal-footer"]], ngContentSelectors: _c2, decls: 1, vars: 0, template: function ModalFooter_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275projectionDef();
      \u0275\u0275projection(0);
    }
  }, encapsulation: 2 });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(ModalFooter, [{
    type: Component,
    args: [{
      selector: "modal-footer",
      standalone: true,
      template: `<ng-content></ng-content>`
    }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(ModalFooter, { className: "ModalFooter", filePath: "src/app/components/modal/modal.ts", lineNumber: 68 });
})();

// src/app/services/session.service.ts
var SessionService = class _SessionService {
  client = inject(HttpClient);
  authService = inject(AuthService);
  createSession(session) {
    const { body, url } = {
      body: session,
      url: "/api/sessions"
    };
    return this.client.post(clientOptions.baseUrl + url, body, {
      headers: { Authorization: "Bearer " + this.authService.token },
      withCredentials: true
    });
  }
  getSessions(range) {
    return range.pipe(mergeMap((range2) => this.client.get(`${clientOptions.baseUrl}/api/sessions?from=${range2.start.toISOString()}&until=${range2.end.toISOString()}`, { headers: { Authorization: "Bearer " + this.authService.token } }).pipe(map((response) => ({ data: response, loading: false })), startWith({ loading: true }), catchError((error) => {
      if (error.status === 500) {
        return of({
          error: "Internal server error",
          loading: false
        });
      }
      return of({
        error: error?.message ?? "Unknown error",
        loading: false
      });
    }))));
  }
  updateSession(id, session) {
    return this.client.put(clientOptions.baseUrl + `/api/sessions/${id}`, session);
  }
  getSession(id) {
    return this.client.get(`${clientOptions.baseUrl}/api/sessions/${id}`);
  }
  static \u0275fac = function SessionService_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _SessionService)();
  };
  static \u0275prov = /* @__PURE__ */ \u0275\u0275defineInjectable({ token: _SessionService, factory: _SessionService.\u0275fac, providedIn: "root" });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(SessionService, [{
    type: Injectable,
    args: [{
      providedIn: "root"
    }]
  }], null, null);
})();

// src/app/util/dateutils.ts
var toFormDate = (date) => {
  return format(date, "yyyy-MM-dd'T'HH:mm:ss");
};

// src/app/pages/scheduler/scheduler.ts
var _c03 = () => [];
function Scheduler_Conditional_28_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "app-input", 16);
    \u0275\u0275element(1, "input", 18);
    \u0275\u0275pipe(2, "invalid");
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const ctx_r0 = \u0275\u0275nextContext();
    \u0275\u0275advance();
    \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(2, 1, ctx_r0.capacity));
  }
}
function Scheduler_Conditional_32_Conditional_0_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275element(0, "app-alert", 19);
  }
  if (rf & 2) {
    const appointments_r3 = \u0275\u0275nextContext();
    \u0275\u0275property("dismissible", true)("message", appointments_r3.error);
  }
}
function Scheduler_Conditional_32_Template(rf, ctx) {
  if (rf & 1) {
    const _r2 = \u0275\u0275getCurrentView();
    \u0275\u0275conditionalCreate(0, Scheduler_Conditional_32_Conditional_0_Template, 1, 2, "app-alert", 19);
    \u0275\u0275elementStart(1, "app-scheduler", 20);
    \u0275\u0275listener("appointmentCreate", function Scheduler_Conditional_32_Template_app_scheduler_appointmentCreate_1_listener($event) {
      \u0275\u0275restoreView(_r2);
      const ctx_r0 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r0.onAppointmentCreate($event));
    })("appointmentUpdate", function Scheduler_Conditional_32_Template_app_scheduler_appointmentUpdate_1_listener($event) {
      \u0275\u0275restoreView(_r2);
      const ctx_r0 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r0.onAppointmentUpdate($event));
    })("loadAppointments", function Scheduler_Conditional_32_Template_app_scheduler_loadAppointments_1_listener($event) {
      \u0275\u0275restoreView(_r2);
      const ctx_r0 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r0.loadAppointments($event));
    });
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const appointments_r3 = ctx;
    \u0275\u0275conditional(appointments_r3.error ? 0 : -1);
    \u0275\u0275advance();
    \u0275\u0275property("loading", appointments_r3.loading)("appointments", appointments_r3.data ?? \u0275\u0275pureFunction0(3, _c03));
  }
}
var Scheduler2 = class _Scheduler {
  sessionService = inject(SessionService);
  savingSession = false;
  showModal = false;
  currentWeek = new BehaviorSubject({ start: /* @__PURE__ */ new Date(), end: /* @__PURE__ */ new Date() });
  refreshTrigger = new BehaviorSubject(void 0);
  authService = inject(AuthService);
  appointments$ = this.refreshTrigger.pipe(switchMap(() => this.sessionService.getSessions(this.currentWeek)), map((sessions) => {
    return __spreadProps(__spreadValues({}, sessions), {
      data: (sessions.data?.items ?? []).map((session) => ({
        id: session.id,
        start: new Date(session.start),
        end: new Date(session.end),
        title: session.title,
        description: session.description
      }))
    });
  }));
  formBuilder = new FormBuilder().nonNullable;
  appointmentForm = this.formBuilder.group({
    id: [void 0],
    title: ["", buildValidations("title", CreateSessionCommandSchema)],
    description: ["", buildValidations("description", CreateSessionCommandSchema)],
    location: ["", buildValidations("location", CreateSessionCommandSchema)],
    start: ["", buildValidations("start", CreateSessionCommandSchema)],
    end: ["", buildValidations("end", CreateSessionCommandSchema)],
    type: ["Individual", buildValidations("type", CreateSessionCommandSchema)],
    capacity: [12, buildValidations("capacity", CreateSessionCommandSchema)]
  });
  get title() {
    return this.appointmentForm.get("title");
  }
  get description() {
    return this.appointmentForm.get("description");
  }
  get location() {
    return this.appointmentForm.get("location");
  }
  get start() {
    return this.appointmentForm.get("start");
  }
  get end() {
    return this.appointmentForm.get("end");
  }
  get type() {
    return this.appointmentForm.get("type");
  }
  get capacity() {
    return this.appointmentForm.get("capacity");
  }
  onAppointmentCreate($event) {
    this.appointmentForm.get("start")?.setValue(toFormDate($event.start));
    this.appointmentForm.get("end")?.setValue(toFormDate($event.end));
    this.showModal = true;
  }
  handleSubmit() {
    if (!this.appointmentForm.valid) {
      this.appointmentForm.markAllAsDirty();
      return;
    }
    this.savingSession = true;
    const value = this.appointmentForm.getRawValue();
    if (value.id) {
      this.sessionService.updateSession(value.id, value).subscribe({
        error: () => {
          this.showModal = false;
          this.savingSession = false;
          this.refreshTrigger.next();
        },
        complete: () => {
          this.showModal = false;
          this.savingSession = false;
          this.appointmentForm.reset();
          this.refreshTrigger.next();
        }
      });
    } else {
      this.sessionService.createSession(__spreadProps(__spreadValues({}, value), {
        type: value.type === "Individual" ? "Individual" : "Group",
        tenantId: this.authService.user.tenantId
      })).subscribe({
        error: () => {
          this.showModal = false;
          this.savingSession = false;
          this.refreshTrigger.next();
        },
        complete: () => {
          this.showModal = false;
          this.savingSession = false;
          this.appointmentForm.reset();
          this.refreshTrigger.next();
        }
      });
    }
  }
  onClose() {
    this.showModal = false;
    this.appointmentForm.reset();
  }
  loadAppointments($event) {
    this.currentWeek.next($event);
  }
  onAppointmentUpdate($event) {
    this.appointments$.pipe(take(1)).subscribe((state2) => {
      const session = (state2.data ?? []).find((a) => a.id === $event.id);
      const selected = session ?? {
        id: $event.id,
        title: "",
        description: "",
        start: $event.start,
        end: $event.end
      };
      this.appointmentForm.setValue({
        title: selected.title ?? "",
        description: selected.title ?? "",
        end: toFormDate(new Date(selected.end)),
        start: toFormDate(new Date(selected.start)),
        capacity: selected.capacity ?? 12,
        type: selected.type ?? "Individual",
        location: selected.location ?? "",
        id: selected.id
      });
      this.showModal = true;
    });
  }
  static \u0275fac = function Scheduler_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Scheduler)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Scheduler, selectors: [["app-scheduler-page"]], decls: 34, vars: 25, consts: [[3, "ngSubmit", "formGroup"], [3, "close", "isOpen"], ["formControlName", "title", "label", "Titel"], ["id", "title", "type", "text", "name", "title", "placeholder", "Titel", "formControlName", "title"], ["formControlName", "description", "label", "Beschrijving"], ["id", "description", "placeholder", "Beschrijving", "formControlName", "description"], ["formControlName", "location", "label", "Locatie"], ["id", "location", "type", "text", "placeholder", "Locatie", "formControlName", "location"], ["formControlName", "start", "label", "Start"], ["id", "start", "type", "datetime-local", "placeholder", "Start", "formControlName", "start"], ["formControlName", "end", "label", "Eind"], ["id", "end", "type", "datetime-local", "placeholder", "Eind", "formControlName", "end"], ["formControlName", "type", "label", "Type"], ["name", "type", "id", "type", "formControlName", "type"], ["value", "Individual"], ["value", "Group"], ["formControlName", "capacity", "label", "Capaciteit"], ["type", "submit"], ["id", "capacity", "type", "number", "placeholder", "Capaciteit", "formControlName", "capacity"], ["type", "danger", 3, "dismissible", "message"], [3, "appointmentCreate", "appointmentUpdate", "loadAppointments", "loading", "appointments"]], template: function Scheduler_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "form", 0);
      \u0275\u0275listener("ngSubmit", function Scheduler_Template_form_ngSubmit_0_listener() {
        return ctx.handleSubmit();
      });
      \u0275\u0275elementStart(1, "app-modal", 1);
      \u0275\u0275listener("close", function Scheduler_Template_app_modal_close_1_listener() {
        return ctx.onClose();
      });
      \u0275\u0275elementStart(2, "modal-header")(3, "strong");
      \u0275\u0275text(4, " Nieuwe sessie ");
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(5, "modal-content")(6, "app-input", 2);
      \u0275\u0275element(7, "input", 3);
      \u0275\u0275pipe(8, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(9, "app-input", 4);
      \u0275\u0275element(10, "textarea", 5);
      \u0275\u0275pipe(11, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(12, "app-input", 6);
      \u0275\u0275element(13, "input", 7);
      \u0275\u0275pipe(14, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(15, "app-input", 8);
      \u0275\u0275element(16, "input", 9);
      \u0275\u0275pipe(17, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(18, "app-input", 10);
      \u0275\u0275element(19, "input", 11);
      \u0275\u0275pipe(20, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(21, "app-input", 12);
      \u0275\u0275pipe(22, "invalid");
      \u0275\u0275elementStart(23, "select", 13)(24, "option", 14);
      \u0275\u0275text(25, "Individueel");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(26, "option", 15);
      \u0275\u0275text(27, "Groep");
      \u0275\u0275elementEnd()()();
      \u0275\u0275conditionalCreate(28, Scheduler_Conditional_28_Template, 3, 3, "app-input", 16);
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(29, "modal-footer")(30, "button", 17);
      \u0275\u0275text(31, "Confirm");
      \u0275\u0275elementEnd()()()();
      \u0275\u0275conditionalCreate(32, Scheduler_Conditional_32_Template, 2, 4);
      \u0275\u0275pipe(33, "async");
    }
    if (rf & 2) {
      let tmp_10_0;
      \u0275\u0275property("formGroup", ctx.appointmentForm);
      \u0275\u0275advance();
      \u0275\u0275property("isOpen", ctx.showModal);
      \u0275\u0275advance(6);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(8, 11, ctx.title));
      \u0275\u0275advance(3);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(11, 13, ctx.description));
      \u0275\u0275advance(3);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(14, 15, ctx.location));
      \u0275\u0275advance(3);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(17, 17, ctx.start));
      \u0275\u0275advance(3);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(20, 19, ctx.end));
      \u0275\u0275advance(2);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(22, 21, ctx.type));
      \u0275\u0275advance(7);
      \u0275\u0275conditional((ctx.type == null ? null : ctx.type.getRawValue()) === "Group" ? 28 : -1);
      \u0275\u0275advance(2);
      \u0275\u0275attribute("aria-busy", ctx.savingSession);
      \u0275\u0275advance(2);
      \u0275\u0275conditional((tmp_10_0 = \u0275\u0275pipeBind1(33, 23, ctx.appointments$)) ? 32 : -1, tmp_10_0);
    }
  }, dependencies: [
    Scheduler,
    Modal,
    ModalFooter,
    ModalHeader,
    ModalContent,
    ReactiveFormsModule,
    \u0275NgNoValidate,
    NgSelectOption,
    \u0275NgSelectMultipleOption,
    DefaultValueAccessor,
    NumberValueAccessor,
    SelectControlValueAccessor,
    NgControlStatus,
    NgControlStatusGroup,
    FormGroupDirective,
    FormControlName,
    InvalidPipe,
    FormInput,
    AsyncPipe,
    Alert
  ], encapsulation: 2 });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Scheduler2, [{
    type: Component,
    args: [{ selector: "app-scheduler-page", imports: [
      Scheduler,
      Modal,
      ModalFooter,
      ModalHeader,
      ModalContent,
      ReactiveFormsModule,
      InvalidPipe,
      FormInput,
      AsyncPipe,
      Alert
    ], template: `<form [formGroup]="appointmentForm" (ngSubmit)="handleSubmit()">
  <app-modal [isOpen]="showModal" (close)="onClose()">
    <modal-header>
      <strong> Nieuwe sessie </strong>
    </modal-header>
    <modal-content>
      <app-input formControlName="title" label="Titel">
        <input
          id="title"
          type="text"
          name="title"
          placeholder="Titel"
          formControlName="title"
          [attr.aria-invalid]="title | invalid"
        />
      </app-input>
      <app-input formControlName="description" label="Beschrijving">
        <textarea
          id="description"
          placeholder="Beschrijving"
          formControlName="description"
          [attr.aria-invalid]="description | invalid"
        ></textarea>
      </app-input>
      <app-input formControlName="location" label="Locatie">
        <input
          id="location"
          type="text"
          placeholder="Locatie"
          formControlName="location"
          [attr.aria-invalid]="location | invalid"
        />
      </app-input>
      <app-input formControlName="start" label="Start">
        <input
          id="start"
          type="datetime-local"
          placeholder="Start"
          formControlName="start"
          [attr.aria-invalid]="start | invalid"
        />
      </app-input>
      <app-input formControlName="end" label="Eind">
        <input
          id="end"
          type="datetime-local"
          placeholder="Eind"
          formControlName="end"
          [attr.aria-invalid]="end | invalid"
        />
      </app-input>
      <app-input formControlName="type" label="Type" [attr.aria-invalid]="type | invalid">
        <select name="type" id="type" formControlName="type">
          <option value="Individual">Individueel</option>
          <option value="Group">Groep</option>
        </select>
      </app-input>
      @if (type?.getRawValue() === 'Group') {
        <app-input formControlName="capacity" label="Capaciteit">
          <input
            id="capacity"
            type="number"
            placeholder="Capaciteit"
            formControlName="capacity"
            [attr.aria-invalid]="capacity | invalid"
          />
        </app-input>
      }
    </modal-content>
    <modal-footer>
      <button [attr.aria-busy]="savingSession" type="submit">Confirm</button>
    </modal-footer>
  </app-modal>
</form>
@if (appointments$ | async; as appointments) {
  @if (appointments.error) {
    <app-alert [dismissible]="true" type="danger" [message]="appointments.error"></app-alert>
  }
  <app-scheduler
    [loading]="appointments.loading"
    [appointments]="appointments.data ?? []"
    (appointmentCreate)="onAppointmentCreate($event)"
    (appointmentUpdate)="onAppointmentUpdate($event)"
    (loadAppointments)="loadAppointments($event)"
  ></app-scheduler>
}
` }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Scheduler2, { className: "Scheduler", filePath: "src/app/pages/scheduler/scheduler.ts", lineNumber: 34 });
})();
export {
  Scheduler2 as Scheduler
};
/*! Bundled license information:

@angular/animations/fesm2022/animations.mjs:
  (**
   * @license Angular v20.0.3
   * (c) 2010-2025 Google LLC. https://angular.io/
   * License: MIT
   *)
*/
//# sourceMappingURL=chunk-VA5VCAOP.js.map
