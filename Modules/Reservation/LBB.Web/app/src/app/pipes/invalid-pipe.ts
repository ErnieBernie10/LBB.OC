import { Pipe, PipeTransform } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Pipe({
  name: 'invalid',
  standalone: true,
  pure: false,
})
export class InvalidPipe implements PipeTransform {
  transform(control: AbstractControl | null, errors?: string[]): boolean | undefined {
    if (!control) return undefined;

    const isInteracted = control.touched;

    if (!isInteracted) return undefined;

    if (errors && errors.length > 0) {
      return errors.some((error) => control.hasError(error)) ? true : undefined;
    }

    return control.invalid ? true : undefined;
  }
}
