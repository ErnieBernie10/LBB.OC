import {
  Component,
  setClassMetadata,
  ɵsetClassDebugInfo,
  ɵɵdefineComponent,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵtext
} from "./chunk-TXVF2NYV.js";

// src/app/pages/home/home.ts
var Home = class _Home {
  modalIsOpen = false;
  toggleModal() {
    this.modalIsOpen = !this.modalIsOpen;
  }
  static \u0275fac = function Home_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Home)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Home, selectors: [["app-home-page"]], decls: 41, vars: 0, consts: [[1, "parent"], [1, "recent"], [1, "next"], [1, "actions"]], template: function Home_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "div", 0)(1, "div", 1)(2, "article")(3, "header")(4, "h2");
      \u0275\u0275text(5, "Recente reservaties");
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(6, "table")(7, "thead")(8, "tr")(9, "th");
      \u0275\u0275text(10, "Datum");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(11, "th");
      \u0275\u0275text(12, "Naam");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(13, "th");
      \u0275\u0275text(14, "Email");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(15, "th");
      \u0275\u0275text(16, "Telefoon");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(17, "th");
      \u0275\u0275text(18, "Aantal personen");
      \u0275\u0275elementEnd()()();
      \u0275\u0275elementStart(19, "tbody")(20, "tr")(21, "td");
      \u0275\u0275text(22, "10/04/99");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(23, "td");
      \u0275\u0275text(24, "Arne Boedt");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(25, "td");
      \u0275\u0275text(26, "Test");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(27, "td");
      \u0275\u0275text(28, "0499176945");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(29, "td");
      \u0275\u0275text(30, "2");
      \u0275\u0275elementEnd()()()()()();
      \u0275\u0275elementStart(31, "div", 2)(32, "article")(33, "header")(34, "h2");
      \u0275\u0275text(35, "Volgende sessie");
      \u0275\u0275elementEnd()()()();
      \u0275\u0275elementStart(36, "div", 3)(37, "article")(38, "header")(39, "h2");
      \u0275\u0275text(40, "Acties");
      \u0275\u0275elementEnd()()()()();
    }
  }, styles: ["\n\n.parent[_ngcontent-%COMP%] {\n  display: grid;\n  grid-template-columns: repeat(2, 1fr);\n  grid-template-rows: repeat(2, 1fr);\n  grid-column-gap: 8px;\n  grid-row-gap: 8px;\n}\n.recent[_ngcontent-%COMP%] {\n  grid-area: 1/1/2/3;\n}\n.next[_ngcontent-%COMP%] {\n  grid-area: 2/1/3/2;\n}\n.actions[_ngcontent-%COMP%] {\n  grid-area: 2/2/3/3;\n}\n@media screen and (max-width: 768px) {\n  .parent[_ngcontent-%COMP%] {\n    grid-template-columns: 1fr;\n    grid-template-rows: auto;\n    grid-row-gap: 16px;\n  }\n  .recent[_ngcontent-%COMP%] {\n    grid-area: 1/1/2/2;\n  }\n  .next[_ngcontent-%COMP%] {\n    grid-area: 2/1/3/2;\n  }\n  .actions[_ngcontent-%COMP%] {\n    grid-area: 3/1/4/2;\n  }\n}\n@media screen and (max-width: 480px) {\n  .parent[_ngcontent-%COMP%] {\n    grid-row-gap: 12px;\n    padding: 8px;\n  }\n  table[_ngcontent-%COMP%] {\n    font-size: 14px;\n  }\n  table[_ngcontent-%COMP%]   th[_ngcontent-%COMP%], \n   table[_ngcontent-%COMP%]   td[_ngcontent-%COMP%] {\n    padding: 8px 4px;\n  }\n}\n/*# sourceMappingURL=home.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Home, [{
    type: Component,
    args: [{ selector: "app-home-page", imports: [], template: '<div class="parent">\n  <div class="recent">\n    <article>\n      <header>\n        <h2>Recente reservaties</h2>\n      </header>\n      <table>\n        <thead>\n          <tr>\n            <th>Datum</th>\n            <th>Naam</th>\n            <th>Email</th>\n            <th>Telefoon</th>\n            <th>Aantal personen</th>\n          </tr>\n        </thead>\n        <tbody>\n          <tr>\n            <td>10/04/99</td>\n            <td>Arne Boedt</td>\n            <td>Test</td>\n            <td>0499176945</td>\n            <td>2</td>\n          </tr>\n        </tbody>\n      </table>\n    </article>\n  </div>\n\n  <div class="next">\n    <article>\n      <header>\n        <h2>Volgende sessie</h2>\n      </header>\n    </article>\n  </div>\n  <div class="actions">\n    <article>\n      <header>\n        <h2>Acties</h2>\n      </header>\n    </article>\n  </div>\n</div>\n', styles: ["/* src/app/pages/home/home.scss */\n.parent {\n  display: grid;\n  grid-template-columns: repeat(2, 1fr);\n  grid-template-rows: repeat(2, 1fr);\n  grid-column-gap: 8px;\n  grid-row-gap: 8px;\n}\n.recent {\n  grid-area: 1/1/2/3;\n}\n.next {\n  grid-area: 2/1/3/2;\n}\n.actions {\n  grid-area: 2/2/3/3;\n}\n@media screen and (max-width: 768px) {\n  .parent {\n    grid-template-columns: 1fr;\n    grid-template-rows: auto;\n    grid-row-gap: 16px;\n  }\n  .recent {\n    grid-area: 1/1/2/2;\n  }\n  .next {\n    grid-area: 2/1/3/2;\n  }\n  .actions {\n    grid-area: 3/1/4/2;\n  }\n}\n@media screen and (max-width: 480px) {\n  .parent {\n    grid-row-gap: 12px;\n    padding: 8px;\n  }\n  table {\n    font-size: 14px;\n  }\n  table th,\n  table td {\n    padding: 8px 4px;\n  }\n}\n/*# sourceMappingURL=home.css.map */\n"] }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Home, { className: "Home", filePath: "src/app/pages/home/home.ts", lineNumber: 9 });
})();
export {
  Home
};
//# sourceMappingURL=chunk-M7GOUGZS.js.map
