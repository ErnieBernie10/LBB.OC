//@ts-nocheck
import { Validators } from '@angular/forms';

export const buildValidations = (propertyName: string, schema: unknown) => {
  const validations = [];
  if (Array.isArray(schema['required'])) {
    if (schema['required'].includes(propertyName)) {
      validations.push(Validators.required);
    }
  } else {
    if (schema['required'] === propertyName) {
      validations.push(Validators.required);
    }
  }

  const properties = schema.properties;
  for (const [key, value] of Object.entries(properties)) {
    if (value.type === 'string') {
      if (value.maxLength) {
        validations.push(Validators.maxLength(value.maxLength));
      }
    }
    if (value.type === 'object') {
      validations.push(buildValidations(key, value));
    }
    if (value.type === 'array') {
      // TODO
    }
  }

  return validations;
};
