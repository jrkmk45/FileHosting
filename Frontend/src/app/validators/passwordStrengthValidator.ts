import { AbstractControl, ValidatorFn } from "@angular/forms";

export function passwordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const hasLower = /[a-z]/.test(control.value);
    const hasUpper = /[A-Z]/.test(control.value);
    const hasNumber = /\d/.test(control.value);

    if (hasLower && hasUpper && hasNumber) {
      return null;
    } else {
      return { 'passwordStrength': true };
    }
  };
}

