import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function passwordsMatchValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    let password = control.get('password')?.value;
    let confirmPassword = control.get('confirmPassword')?.value;

    if (password == confirmPassword) {
      return null;
    } else {
      return { 'passwordMathError': true };
    }
  };
}