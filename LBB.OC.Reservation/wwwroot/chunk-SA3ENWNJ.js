import {
  Router
} from "./chunk-35JJGJVE.js";
import {
  Alert,
  AuthService,
  DefaultValueAccessor,
  FormBuilder,
  FormControlName,
  FormGroupDirective,
  FormInput,
  InvalidPipe,
  LoginRequestSchema,
  NgControlStatus,
  NgControlStatusGroup,
  ReactiveFormsModule,
  buildValidations,
  ɵNgNoValidate
} from "./chunk-67T4FLOB.js";
import "./chunk-IXWTYFU2.js";
import "./chunk-ABNVDUIW.js";
import {
  Component,
  inject,
  setClassMetadata,
  ɵsetClassDebugInfo,
  ɵɵadvance,
  ɵɵattribute,
  ɵɵconditional,
  ɵɵconditionalCreate,
  ɵɵdefineComponent,
  ɵɵelement,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵlistener,
  ɵɵnextContext,
  ɵɵpipe,
  ɵɵpipeBind1,
  ɵɵproperty,
  ɵɵtext
} from "./chunk-6MVD4A56.js";

// src/app/pages/login/login.ts
function Login_Conditional_9_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275element(0, "app-alert", 6);
  }
  if (rf & 2) {
    const ctx_r0 = \u0275\u0275nextContext();
    \u0275\u0275property("dismissible", true)("message", ctx_r0.error);
  }
}
function Login_Conditional_10_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275element(0, "app-alert", 7);
  }
  if (rf & 2) {
    \u0275\u0275property("dismissible", true);
  }
}
var Login = class _Login {
  authService = inject(AuthService);
  formBuilder = new FormBuilder().nonNullable;
  router = inject(Router);
  loginForm = this.formBuilder.group({
    email: ["", buildValidations("email", LoginRequestSchema)],
    password: ["", buildValidations("password", LoginRequestSchema)]
  });
  loading = false;
  error = null;
  success = false;
  onSubmit() {
    if (!this.loginForm.valid) {
      this.loginForm.markAllAsDirty();
      return;
    }
    const value = this.loginForm.getRawValue();
    this.loading = true;
    this.authService.login(value.email, value.password).subscribe({
      error: (e) => {
        this.loading = false;
        this.success = false;
        if (e.status === 401) {
          this.error = "Invalid email or password";
          return;
        }
        this.error = "Something went wrong. Please try again later.";
      },
      next: (response) => {
        this.loading = false;
        this.error = null;
        this.success = true;
        setTimeout(() => this.router.navigateByUrl("/"), 500);
      }
    });
  }
  get email() {
    return this.loginForm.get("email");
  }
  get password() {
    return this.loginForm.get("password");
  }
  static \u0275fac = function Login_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Login)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Login, selectors: [["app-login"]], decls: 11, vars: 11, consts: [[3, "ngSubmit", "formGroup"], ["formControlName", "email", "label", "Email"], ["id", "email", "type", "email", "placeholder", "Email", "formControlName", "email"], ["formControlName", "password", "label", "Password"], ["id", "password", "type", "password", "placeholder", "Password", "formControlName", "password"], ["type", "submit", 3, "disabled"], ["type", "danger", 3, "dismissible", "message"], ["type", "success", "message", "Login success!", 3, "dismissible"]], template: function Login_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "form", 0);
      \u0275\u0275listener("ngSubmit", function Login_Template_form_ngSubmit_0_listener() {
        return ctx.onSubmit();
      });
      \u0275\u0275elementStart(1, "app-input", 1);
      \u0275\u0275element(2, "input", 2);
      \u0275\u0275pipe(3, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(4, "app-input", 3);
      \u0275\u0275element(5, "input", 4);
      \u0275\u0275pipe(6, "invalid");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(7, "button", 5);
      \u0275\u0275text(8, "Login");
      \u0275\u0275elementEnd();
      \u0275\u0275conditionalCreate(9, Login_Conditional_9_Template, 1, 2, "app-alert", 6);
      \u0275\u0275conditionalCreate(10, Login_Conditional_10_Template, 1, 1, "app-alert", 7);
      \u0275\u0275elementEnd();
    }
    if (rf & 2) {
      \u0275\u0275property("formGroup", ctx.loginForm);
      \u0275\u0275advance(2);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(3, 7, ctx.email));
      \u0275\u0275advance(3);
      \u0275\u0275attribute("aria-invalid", \u0275\u0275pipeBind1(6, 9, ctx.password));
      \u0275\u0275advance(2);
      \u0275\u0275property("disabled", ctx.loading);
      \u0275\u0275attribute("aria-busy", ctx.loading);
      \u0275\u0275advance(2);
      \u0275\u0275conditional(ctx.error ? 9 : -1);
      \u0275\u0275advance();
      \u0275\u0275conditional(ctx.success ? 10 : -1);
    }
  }, dependencies: [ReactiveFormsModule, \u0275NgNoValidate, DefaultValueAccessor, NgControlStatus, NgControlStatusGroup, FormGroupDirective, FormControlName, FormInput, Alert, InvalidPipe], encapsulation: 2 });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Login, [{
    type: Component,
    args: [{ selector: "app-login", imports: [ReactiveFormsModule, FormInput, Alert, InvalidPipe], template: '<form [formGroup]="loginForm" (ngSubmit)="onSubmit()">\n  <app-input formControlName="email" label="Email">\n    <input id="email" type="email" placeholder="Email" formControlName="email" [attr.aria-invalid]="email | invalid" />\n  </app-input>\n  <app-input formControlName="password" label="Password">\n    <input\n      id="password"\n      type="password"\n      placeholder="Password"\n      formControlName="password"\n      [attr.aria-invalid]="password | invalid"\n    />\n  </app-input>\n  <button [attr.aria-busy]="loading" type="submit" [disabled]="loading">Login</button>\n  @if (error) {\n    <app-alert [dismissible]="true" type="danger" [message]="error"></app-alert>\n  }\n  @if (success) {\n    <app-alert [dismissible]="true" type="success" message="Login success!"></app-alert>\n  }\n</form>\n' }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Login, { className: "Login", filePath: "src/app/pages/login/login.ts", lineNumber: 18 });
})();
export {
  Login
};
//# sourceMappingURL=chunk-SA3ENWNJ.js.map
