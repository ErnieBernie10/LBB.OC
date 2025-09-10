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

// Normalize/augment server metadata so UI messages can use consistent parameter names
export function mapMetadataForCode(code?: string, metadata?: any): any {
  if (!metadata || typeof metadata !== 'object') return metadata;
  switch (code) {
    case 'GreaterThanValidator': {
      const min =
        metadata.min ??
        metadata.Min ??
        metadata.Minimum ??
        metadata.minimum ??
        metadata.ComparisonValue ??
        metadata.ValueToCompare ??
        metadata.Value ??
        metadata.Expected ??
        undefined;
      return { ...metadata, min };
    }
    case 'GreaterThanOrEqualValidator': {
      const min =
        metadata.min ??
        metadata.Min ??
        metadata.Minimum ??
        metadata.minimum ??
        metadata.ComparisonValue ??
        metadata.ValueToCompare ??
        metadata.Value ??
        metadata.Expected ??
        undefined;
      return { ...metadata, min };
    }
    case 'LessThanValidator': {
      const max =
        metadata.max ??
        metadata.Max ??
        metadata.Maximum ??
        metadata.maximum ??
        metadata.ComparisonValue ??
        metadata.ValueToCompare ??
        metadata.Value ??
        metadata.Expected ??
        undefined;
      return { ...metadata, max };
    }
    case 'LessThanOrEqualValidator': {
      const max =
        metadata.max ??
        metadata.Max ??
        metadata.Maximum ??
        metadata.maximum ??
        metadata.ComparisonValue ??
        metadata.ValueToCompare ??
        metadata.Value ??
        metadata.Expected ??
        undefined;
      return { ...metadata, max };
    }
    case 'MaximumLengthValidator': {
      const max =
        metadata.max ??
        metadata.Max ??
        metadata.Maximum ??
        metadata.maximum ??
        metadata.ComparisonValue ??
        metadata.ValueToCompare ??
        metadata.Value ??
        metadata.Expected ??
        metadata.MaxLength ??
        undefined;
      return { ...metadata, max };
    }
    default:
      return metadata;
  }
}
