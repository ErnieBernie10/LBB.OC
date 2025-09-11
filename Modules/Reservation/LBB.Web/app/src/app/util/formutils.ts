// Normalize potential server field names to Angular form control paths
export function normalizeFieldPath(field: string): string {
  // Server may send PascalCase (e.g., Title); form controls tend to be camelCase (e.g., title)
  // Also support nested properties separated by '.' to '/' used by Angular paths
  const path = field.replace(/\[(\d+)\]/g, '.$1');
  const segments = path.split('.').map((s, i) => (i === 0 ? decapitalize(s) : decapitalizeIfPascal(s)));
  return segments.join('.');
}

function decapitalize(s: string): string {
  return s.length ? s.charAt(0).toLowerCase() + s.slice(1) : s;
}

function decapitalizeIfPascal(s: string): string {
  return /^[A-Z]/.test(s) ? decapitalize(s) : s;
}
