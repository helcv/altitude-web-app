import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";


export class CustomValidators {
    static passwordStrength(control: AbstractControl): ValidationErrors | null {
        const value: string = control.value || '';
        if (!value) {
          return null;
        }
    
        const hasUpperCase = /[A-Z]+/.test(value);
        const hasDigit = /[0-9]+/.test(value);
    
        const errors: ValidationErrors = {};
    
        if (!hasUpperCase) {
          errors['uppercase'] = 'Passwords must have at least one uppercase letter (A-Z).';
        }
    
        if (!hasDigit) {
          errors['digit'] = 'Passwords must have at least one digit (0-9).';
        }
    
        return Object.keys(errors).length > 0 ? errors : null;
    }

    static matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl): ValidationErrors | null => {
        return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
      };
    }
}