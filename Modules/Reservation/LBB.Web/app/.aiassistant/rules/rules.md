---
apply: always
---

# Project rules

- Angular 20 syntax is required. @for, @if.
- PicoCSS is used
  - Prefer PicoCSS default styling
  - Prefer using PicoCSS variables in custom styling
- For tiny components use standalone components, for anything else use separate html, ts and scss files
- Prefer signals to observables
- Validation is done on the backend using form-validation service. No frontend validation.
- Always add i18n attributes or $localize for display text
