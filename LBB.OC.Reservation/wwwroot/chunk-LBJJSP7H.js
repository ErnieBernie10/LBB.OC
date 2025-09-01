import {
  Component,
  setClassMetadata,
  ɵsetClassDebugInfo,
  ɵɵdefineComponent,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵtext
} from "./chunk-6MVD4A56.js";

// src/app/pages/reservations/reservations.ts
var Reservations = class _Reservations {
  static \u0275fac = function Reservations_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Reservations)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Reservations, selectors: [["app-reservations-page"]], decls: 2, vars: 0, template: function Reservations_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "p");
      \u0275\u0275text(1, "reservations works!");
      \u0275\u0275elementEnd();
    }
  }, encapsulation: 2 });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Reservations, [{
    type: Component,
    args: [{ selector: "app-reservations-page", imports: [], template: "<p>reservations works!</p>\n" }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Reservations, { className: "Reservations", filePath: "src/app/pages/reservations/reservations.ts", lineNumber: 9 });
})();
export {
  Reservations
};
//# sourceMappingURL=chunk-LBJJSP7H.js.map
